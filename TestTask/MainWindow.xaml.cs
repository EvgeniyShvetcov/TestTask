using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Reflection;

namespace TestTask
{
    public partial class MainWindow : Window
    {
        private TagStorage tagStorage = new TagStorage();
        //Объявление класса для работы с потоком
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker)this.FindResource("backgroundWoker");

        }
        //Загрузка XML файла.
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            treeView.Items.Clear();

            //Создание объекта для открытия диалогового окна.
            OpenFileDialog LoadFileDialog = new OpenFileDialog();
            //Задание расширения по умолчанию.
            LoadFileDialog.DefaultExt = ".xml";
            string StreamArgument = "";
            //Установка фильтра на файлы.
            LoadFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (LoadFileDialog.ShowDialog() == true)
            {
                //Получение полного пути к файлу.
                StreamArgument = LoadFileDialog.FileName + ",Load" ;
                //Запуск фонового потока для загрузки дерева тегов.
                backgroundWorker.RunWorkerAsync(StreamArgument);
            }
            else
            {
                backgroundWorker.CancelAsync();
            }

        }

        //Событие фонового потока при его запуске.
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Создание временного объекта.
                TagStorage temptagstorage = new TagStorage("Root");
                string fullpath = "";
                string LoadOrSave = "";
                fullpath = ((string)(e.Argument)).Split(',')[0];
                LoadOrSave = ((string)(e.Argument)).Split(',')[1];
                if (LoadOrSave == "Load")
                {
                    temptagstorage.LoadFromXML(fullpath);
                    e.Result = temptagstorage;
                }
                if (LoadOrSave == "Save")
                {
                    tagStorage.SaveToXML(fullpath);
                    e.Result = null;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при работе с XML файлом!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Событие для фонового потока при его завершении.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Ошибка была сгенерирована обработчиком события DoWork.
                MessageBox.Show(e.Error.Message, "Произошла ошибка");
            }
            if (e.Result != null)
            {
                tagStorage = (TagStorage)e.Result;
                treeView.ItemsSource = tagStorage.GetChildTag();
            }
            else
            {
                tagStorage.ischanged = false;
            }
        }

        //Событие при нажатии кнопки "Удалить тег".
        private void deleteTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeView.SelectedItem != null)
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный тег?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (result == MessageBoxResult.Yes)
                    {
                        //Будет работать в случае если дерево корректно отрисовано.
                        tagStorage.DeleteChildTag((
                        (KeyValuePair<string, TagItem>)(treeView.SelectedItem))
                        .Value.Fullpath);

                        //Обновление компоненты.
                        treeView.Items.Refresh();
                        //Был изменено дерево тегов.
                        tagStorage.ischanged = true;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите из списка, который хотите удалить!");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при удалении указанного тега!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Событие при нажатии кнопки "Переименовать тег".
        private void renameTag_Click(object sender, RoutedEventArgs e)
        {
            string newtagname = "";
            if (treeView.SelectedItem != null)
            {
                InformationForTagRename newWindow = new InformationForTagRename();
                newWindow.Owner = this;

                //Открываем окно
                newWindow.ShowDialog();
                newWindow.GetTagInformation(out newtagname);
                tagStorage.RenameTag(((
                        (KeyValuePair<string, TagItem>)(treeView.SelectedItem))
                        .Value.Fullpath), newtagname);

                    //Обновление компоненты.
                    treeView.Items.Refresh();
                    //Был изменено дерево тегов.
                    tagStorage.ischanged = true;
            }
            else
            {
                MessageBox.Show("Необходимо указать тег, который требуется переименовать.", "Уведомление",MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        //Событие при нажатии кнопки "Добавить тег".
        private void addTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tagname, tagtype, tagvalue;
                //Создаём объект нового окна
                InformationForTagAdd newWindow = new InformationForTagAdd();
                //Присваиваем какое окно главное
                newWindow.Owner = this;
                //Вызываем окно
                newWindow.ShowDialog();
                //Берём данные из 2 окна
                newWindow.GetNewTagInformation(out tagname, out tagtype, out tagvalue);
                //Проверяем в каком месте дерева требуется создать тег.
                if (treeView.SelectedItem == null)
                {
                    //Создание в корне дерева.
                    tagStorage.AddChildTag("", tagname, tagtype, tagvalue);
                }
                //Создание "ниже" выбранного тега.
                else
                {
                    string fullpath;
                    //Нахождение полного пути выбранного тега.
                    fullpath = ((KeyValuePair<string, TagItem>)
                        (treeView.SelectedItem)).
                        Value.Fullpath;

                    tagStorage.AddChildTag(fullpath, tagname, tagtype, tagvalue);
                }
                //Обновление дерева тегов.
                treeView.ItemsSource = tagStorage.GetChildTag();
                treeView.Items.Refresh();

                tagStorage.ischanged = true;
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении тега!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Cобытие возникающее при закрытии окна приложения.
        private void OnClosing(object sender, CancelEventArgs e)
        {
            //Были внесены изменения в дерево тегов.
            if (tagStorage.ischanged == true)
            {
                MessageBoxResult result = MessageBox.Show("Сохранить результаты изменений дерева тегов в файл?", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFileDialog SaveFileDialog = new SaveFileDialog();
                    SaveFileDialog.DefaultExt = ".xml";
                    SaveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                    SaveFileDialog.ShowDialog();

                    if (SaveFileDialog.FileName != "")
                    {
                        string StreamArgument = SaveFileDialog.FileName + ",Save";
                        backgroundWorker.RunWorkerAsync(StreamArgument);
                    }
                    else
                    {
                        MessageBox.Show("Не был указан файл для выгрузки дерева.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //Прерывание закрытия окна.
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Изменения не были сохранены ;)", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        //Событие для отмены выделения тега в дереве по нажатию правой кнопки мыши.
        private void CancelSelectedTag(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                //Обходим все теги первого уровня.
                foreach (var item in treeView.Items)
                {
                    TreeViewItem childTreeItem = treeView.ItemContainerGenerator.
                                        ContainerFromItem(item) as TreeViewItem;

                    if (childTreeItem != null)
                    {
                        //Сбрасываем у них выделение.
                        childTreeItem.IsSelected = false;
                        //Спускаемся ниже по дереву.
                        CancelSelectedTag(childTreeItem);
                    }
                }
            }
        }

        //Событие для отмены выделения тега в дереве по нажатию правой кнопки мыши.
        //Для более нижних уровней
        private void CancelSelectedTag(TreeViewItem childTreeItem)
        {
            if (treeView.SelectedItem != null)
            {
                //Обходим все теги 2 и более уровней
                foreach (var item in childTreeItem.Items)
                {
                    TreeViewItem childItem = childTreeItem.ItemContainerGenerator.
                    ContainerFromItem(item) as TreeViewItem;
                    if (childItem != null)
                    {
                        //Сбрасываем выделение.
                        childItem.IsSelected = false;
                        //Спускаемся ниже.
                        CancelSelectedTag(childItem);
                    }
                }
            }
        }

        private void SaveToXML(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.DefaultExt = ".xml";
            SaveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            SaveFileDialog.ShowDialog();

            string StreamArgument = SaveFileDialog.FileName + ",Save";
            backgroundWorker.RunWorkerAsync(StreamArgument);
        }

        //Был нажат пункт меню "Выход"
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AboutProgramm_Click(object sender, RoutedEventArgs e)
        {
            AboutProgramm newWindow = new AboutProgramm();
            newWindow.Owner = this;
            newWindow.ShowDialog();
        }
    }
}
