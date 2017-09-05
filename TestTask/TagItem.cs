using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestTask
{
    class TagItem
    {
        //Имя тега.
        private string TagName;

        //Имя родителя тега.
        private string ParentTagName;

        //Тип тега.
        private string TagType;

        //Значение тега.
        private dynamic TagValue;

        //Полный путь к тегу.
        private string FullPath;

        //Уровень вложения тега.
        private int TagLevel;

        //Полная информация о теге
        private string AllTagInformation;

        //Путь до иконки текущего тега
        private string TagIconPath;

        //Список дочерних тегов в родительском.
        public Dictionary<string, TagItem> childList { get; set; }

        //Чтение/запись имени тега.
        public string Tagname
        {
            get
            {
                return TagName;
            }
            set
            {
                TagName = value;
            }
        }

        //Чтение/запись имени родительского тега тега.
        public string ParentTagname{ get; set; }

        //Запись/чтение полного пути к тегу.
        public string Fullpath
        {
            get
            {
                return FullPath;
            }
            set
            {
                if (value == "")
                {
                    TagLevel = 1;
                    FullPath = TagName;
                }
                else
                {
                    value = value + "." + TagName;

                    int count = 0;
                    string[] t = value.Split('.');

                    foreach (var k in t)
                    {
                        count++;
                    }
                    //Расчёт уровня вложенности тега.
                    TagLevel = count;
                    FullPath = value;
                    if (TagLevel >= 2)
                    {
                        ParentTagName = t[TagLevel - 2];
                    }
                }
            }
        }

        //Уровень тега.
        public int Taglevel
        {
            get
            {
                return TagLevel;
            }
            set
            {
                TagLevel = value;
            }
        }

        //Полная информация о теге
        public string Alltaginformation
        {
            get
            {
                return AllTagInformation;
            }
            set
            {
                AllTagInformation = TagName + ", " + TagLevel + ", " + GetTagType() + ", " + FullPath;
            }
        }

        //Запись/чтение пути до иконки соответствующего типа
        public string Tagiconpath
        {
            get
            {
                return TagIconPath;
            }
            set
            {
                switch (TagType)
                {
                    case "Double":
                        {
                            TagIconPath = Directory.GetCurrentDirectory() + "\\Icons\\1.png";
                            break;
                        }
                    case "Int":
                        {
                            TagIconPath = Directory.GetCurrentDirectory() + "\\Icons\\2.png";
                            break;
                        }
                    case "Bool":
                        {
                            TagIconPath = Directory.GetCurrentDirectory() + "\\Icons\\3.png";
                            break;
                        }
                    case "None":
                        {
                            TagIconPath = Directory.GetCurrentDirectory() + "\\Icons\\4.png";
                            break;
                        }
                }
            }
        }

        //Конструктор по-умолчанию.
        public TagItem()
        {
            childList = new Dictionary<string, TagItem>();
        }
        //Конструктор класса.
        public TagItem(string tagname)
        {
            Tagname = tagname;
            childList = new Dictionary<string, TagItem>();
        }

        //Выбор типа тега.
        public void SetTagType(char tagtype)
        {
            switch (tagtype)
            {
                case (char)49:
                    {
                        TagType = "Double";
                        break;
                    }
                case (char)50:
                    {
                        TagType = "Int";
                        break;
                    }
                case (char)51:
                    {
                        TagType = "Bool";
                        break;
                    }
                case (char)52:
                    {
                        TagType = "None";
                        break;
                    }
            }
        }

        //Метод записи типа тега.
        public void SetTagType(string tagtype)
        {
            TagType = tagtype;
        }

        //Получение имени тега.
        public string GetTagType()
        {
            return TagType;
        }

        //Присваивание значения тегу.
        public void SetTagValue(dynamic tagvalue)
        {
            try
            {
                switch (TagType)
                {
                    case "Double":
                        {
                            TagValue = tagvalue;
                            break;
                        }
                    case "Int":
                        {
                            TagValue = tagvalue;
                            break;
                        }
                    case "Bool":
                        {
                            TagValue = tagvalue;
                            break;
                        }
                    case "None":
                        break;
                }
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine(Console.Error);
            }
        }

        //Получение значение тега.
        public dynamic GetTagValue()
        {
            return TagValue;
        }

        //Создание дочернего тега.
        public void AddChildTag(string parentpath, string tagname, dynamic tagtype, dynamic tagtvalue)
        {
            //Разбиение полного пути.
            string[] t = parentpath.Split('.');

            //Проверка на то,откуда был вызван метод.
            if((t.Last() == this.Tagname) || (t.Last() == ""))
            {
                TagItem childTag = new TagItem(tagname);
                childTag.SetTagType(tagtype);
                childTag.Fullpath = parentpath;
                childTag.SetTagValue(tagtvalue);
                childTag.Alltaginformation = "";
                childTag.Tagiconpath = "";
                childList.Add(childTag.Tagname, childTag);
            }
            //Если требуется добавить дочерний тег через уровень.
            else
            {
                string nextTag = t[this.Taglevel];
                childList[nextTag].AddChildTag(parentpath, tagname, tagtype, tagtvalue);
            }
        }

        //Переименование тега.
        public void RenameTag(string fullpath, string newtagname)
        {
            //Разбиение полного пути.
            string[] t = fullpath.Split('.');

            //Проверка на то,откуда был вызван метод.
            if (t.Last() == this.Tagname)
            {
                string parentpath = "";

                for (int i = 0; i < t.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        parentpath = t[i];
                    }
                    else
                    {
                        parentpath = parentpath + "." + t[i];
                    }
                }
                if (childList.Count == 0)
                {   
                    this.Tagname = newtagname;
                    this.Fullpath = parentpath;
                    this.Alltaginformation = "";
                }
                else
                {
                    this.Tagname = newtagname;
                    this.Fullpath = parentpath;
                    this.Alltaginformation = "";
                    foreach (var k in this.childList)
                    {
                        k.Value.Fullpath = this.Fullpath;
                        k.Value.Alltaginformation = this.Alltaginformation;
                        if(k.Value.childList.Count != 0)
                            k.Value.RenameTag(k.Value.Fullpath, k.Value.Tagname);
                    }
                }
            }
            else
            {
                foreach (var k in childList)
                {
                    if (k.Value.Tagname == t[this.Taglevel])
                    {
                        k.Value.RenameTag(fullpath, newtagname);
                    }
                }
            }
        }


        //Удаление тега по полному пути.
        public void DeleteChildTag(string fullpath)
        {
            //Разбиение полного пути.
            string[] t = fullpath.Split('.');

            if (t.Length == this.Taglevel + 1)
            {
                foreach (var k in childList)
                {
                    if (k.Value.Tagname == t.Last())
                    {
                        childList.Remove(t.Last());
                        break;
                    }
                }
            }
            else
            {
                string nextTag = t[this.Taglevel];
                childList[nextTag].DeleteChildTag(fullpath);
            }
        }

        //Получение списка дочерних тегов.
        public Dictionary<string, TagItem> GetChildTag()
        {
            return childList;
        }

    }
}
