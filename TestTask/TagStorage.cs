using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestTask
{
    class TagStorage : TagItem
    {
        //Список информации о всех тегах, содержащихся в корневом теге.
        List<string> allInformation = new List<string>();
        //Флаг изменения дерева тегов(0 - по-умолчанию или после загрузки из файла,
        //                            1 - после изменения пустого или загруженного дерева).
        private bool isChanged;

        //Запись/чтение флага изменения дерева тегов.
        public bool ischanged
        {
            get
            {
                return isChanged;
            }
            set
            {
                isChanged = value;
            }
        }
        //Конструктор по-умолчанию
        public TagStorage()
        {
            this.Tagname = "Root";
            isChanged = false;
        }

        //Конструктор класса.
        public TagStorage(string tagname)
        {
            this.Tagname = tagname;
            isChanged = false;
        }

        //Сохранение дерева тегов в файл.
        public void SaveToXML(string filepath)
        {
            XmlTextWriter textWritter = new XmlTextWriter(filepath, Encoding.UTF8);

            //Создание шапки.
            textWritter.WriteStartDocument();
            textWritter.WriteStartElement("Root");

            //Перебор всего списка тегов.
            foreach (var keyValue in childList)
            {
                //Создание узла.
                textWritter.WriteStartElement(childList[keyValue.Key].Tagname);

                //Создание аттрибута узла.
                textWritter.WriteAttributeString(childList[keyValue.Key].GetTagType(), childList[keyValue.Key].GetTagValue());

                //Проверка на наличие дочерних тегов.
                if (keyValue.Value.GetChildTag().Count == 0)
                    {
                        //Если их нет закрываем узел.
                        textWritter.WriteEndElement();
                    }
                    //Иначе переходим ниже по дереву.
                    else
                    {
                        //Вызов метода для дочерних тегов уровня ниже.
                        SaveToXML(keyValue.Value.GetChildTag(), textWritter);
                        textWritter.WriteEndElement();
                    }
            }
            //Закрытие всех "незакрытых тегов".
            textWritter.WriteEndDocument();

            //Закрытие и сохранение файла.
            textWritter.Close();
        }

        //Сохранение для тегов нижних уровней 2 и более.
        public void SaveToXML(Dictionary<string, TagItem> list, XmlTextWriter textwritter)
        {
            //Перебираем все теги.
            foreach (var keyValue in list)
            {
                //Создаём узел.
                textwritter.WriteStartElement(keyValue.Value.Tagname);

                //Создаём аттрибут.
                textwritter.WriteAttributeString(keyValue.Value.GetTagType(), keyValue.Value.GetTagValue());

                //Проверяем если ли дочерние теги.
                if (keyValue.Value.GetChildTag().Count == 0)
                {
                    //Закрываем узел.
                    textwritter.WriteEndElement();
                }
                //Иначе переходим ниже по дереву.
                else
                {
                    SaveToXML(keyValue.Value.GetChildTag(), textwritter);
                }
            }
        }

        //Загрузка из XML файла.
        public void LoadFromXML(string filepath)
        {
            XmlDocument document = new XmlDocument();

            //Загрузка файла.
            document.Load(filepath);
            GetChildInformation(document.ChildNodes, "");
        }

        //Метод для "вытаскивания" нужной информации из XML файла.
        public void GetChildInformation(XmlNodeList nodeList, string parentpath)
        {
            //пробежимся по всем сыновьям.
            foreach (XmlNode node in nodeList)
            {
                string tagname = "";
                string tagtype = "";
                string tagvalue = "";

                if (node.NodeType == XmlNodeType.Element)
                {
                     if (node.Name != "Root")
                     {
                        tagname = node.Name;
                     }
                }
                //Если имеются аттрибуты в узле.
                if (node.Attributes != null)
                {   //пробежимся по аттрибутам.
                    foreach (XmlAttribute atr in node.Attributes)
                    {
                        tagtype = atr.Name;
                        tagvalue = atr.Value;
                    }
                }
                //Если данные введены верно добавляем тег.
                if ((tagname != "") && (tagtype != ""))
                {
                    AddChildTag(parentpath, tagname, tagtype, tagvalue);
                }
                //если есть дети, то рекурсивно добавим их.
                if (node.HasChildNodes)
                {
                    //Формирование полного пути для добавления тега.
                    if (node.Name != "Root")
                    {
                        if (parentpath == "")
                        {
                            parentpath = node.Name;
                        }
                        else
                        {
                            parentpath = parentpath + "." + node.Name;
                        }
                    }
                    GetChildInformation(node.ChildNodes, parentpath);
                    parentpath = "";
                }
                //Когда кончились потомки ветви переходим обратно в корень.
            }
        }

        //Заполнение массива для построчечного выведения.
        public void FindAllTagInformation()
        {
            FindAllTagInformation(childList);
        }

        //Заполнение массива для построчечного выведения.
        public void FindAllTagInformation(Dictionary<string, TagItem> childlist)
        {
            foreach(var k in childlist)
            {
                allInformation.Add(k.Value.Fullpath + "," + k.Value.Taglevel + "," + k.Value.GetTagType() + "," + k.Value.GetTagValue());

                //Если есть дочерние теги вызываем рекурсивно метод для уровня ниже.
                if (childlist.Count != 0)
                {
                    FindAllTagInformation(k.Value.GetChildTag());
                }
            }
        }

        //Получение списка дочерних тегов для корневого тега.
        public Dictionary<string, TagItem> GetTagStorage()
        {
            return childList;
        }

        //Полная информация о тегах и потомков расположенных в корне.
        public List<string> GetTagsInfo()
        {
            return allInformation;
        }
    }
}
