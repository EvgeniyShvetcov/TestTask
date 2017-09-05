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
using System.Windows.Shapes;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для InformationForTagAdd.xaml
    /// </summary>
    public partial class InformationForTagAdd : Window
    {

        private string TagNameAdded { get; set; }
        private string TagTypeAdded { get; set; }
        private string TagValueAdded { get; set; }

        public InformationForTagAdd()
        {
            InitializeComponent();
        }

        //Добавление тега
        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBlock selecteditem = (TextBlock)comboBox.SelectedItem;
                TagNameAdded = tagName.Text;
                TagTypeAdded = selecteditem.Text;
                TagValueAdded = tagValue.Text;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Данные требуемые для добавления тега не были введены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

        }

        //Проверка выбранного типа тега
        private void CheckSelectedInformation(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 3)
            {
                tagValue.IsEnabled = false;
                tagValue.Background = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                if (!tagValue.IsEnabled)
                {
                    tagValue.IsEnabled = true;
                    tagValue.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        //Метод для пересылки данных в главное окно
        public void GetNewTagInformation(out string tagname, out string tagtype, out string tagvalue)
        {
                tagname = TagNameAdded;
                tagtype = TagTypeAdded;
                tagvalue = TagValueAdded;
        }

    }

}
