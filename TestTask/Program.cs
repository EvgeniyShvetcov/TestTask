using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestTask
{
    class Program
    {
        static TagStorage TagStrg = new TagStorage("Root");

        static void Main(string[] args)
        {
            Menu();
        }

        //Функция основного меню
        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Выберите требуемый пункт меню:");
            Console.WriteLine("1.Загрузить дерево тегов из XML файла");
            Console.WriteLine("2.Выгрузить дерево тегов в XML файл");
            Console.WriteLine("3.Вывод списка тегов");
            Console.WriteLine("4.Добавление тега");
            Console.WriteLine("5.Удаление тега по полному пути");
            Console.WriteLine("6.Переименование тега");

            char key = Console.ReadKey().KeyChar;

            //Проверка на пункт выбранного меню.
            switch (key)
            {
                //"Нажата клавиша 1".
                case (char)49:
                    {
                        TagStrg.LoadFromXML();
                        Console.WriteLine("Дерево было загружено. Нажмите любую клавишу, чтобы продолжить...");
                        Console.ReadKey();
                        Menu();
                        break;
                    }

                //"Нажата клавиша 2".
                case (char)50:
                    {
                        TagStrg.SaveToXML();
                        Console.WriteLine("Дерево было выгружено в файл text.xml. Нажмите любую клавишу, чтобы продолжить...");
                        Console.ReadKey();
                        Menu();
                        break;
                    }

                //"Нажата клавиша 3".
                case (char)51:
                    {
                        ShowTag();
                        break;
                    }
                //"Нажата клавиша 4".
                case (char)52:
                    {
                        AddTag();
                        break;
                    }

                //"Нажата клавиша 5".
                case (char)53:
                    {
                        DeleteTag();
                        break;
                    }

                //"Нажата клавиша 6".
                case (char)54:
                    {
                        RenameTag();
                        break;
                    }
            }
        }

        //Добавление тега.
        static void AddTag()
        {
            string fullpath, tagname;
            char tagtype;

            Console.Clear();
            Console.WriteLine("Введите полный путь к родительскому тегу(Пустая строка создание в корне):");
            fullpath = Console.ReadLine();

            Console.WriteLine("Введите имя тега:");
            tagname = Console.ReadLine();

            Console.WriteLine("Выберите тип тега:");
            Console.WriteLine("1.Double");
            Console.WriteLine("2.Int");
            Console.WriteLine("3.Bool");
            Console.WriteLine("4.None");

            tagtype = Console.ReadKey().KeyChar;
            dynamic tagvalue = "";

            if (tagtype != 52)
            {
                Console.WriteLine("\nВведите значение тега:");
                tagvalue = Console.ReadLine();
            }
            TagStrg.AddChildTag(fullpath, tagname, tagtype, tagvalue);

            Menu();
        }

        //Вывод списка тегов и их информации.
        static void ShowTag()
        {
            Console.Clear();

            TagStrg.FindAllTagInformation();

            for(var i = 0; i < TagStrg.GetTagsInfo().Count; i++)
            {
                Console.WriteLine(TagStrg.GetTagsInfo()[i]);
            }
            Console.Read();

            Menu();
        }

        //Удаление тега по полному пути.
        static void DeleteTag()
        {
            string path;

            Console.Clear();

            Console.WriteLine("Введите полный путь удаляемого тега:");
            path = Console.ReadLine();

            TagStrg.DeleteChildTag(path);

            Menu();
        }

        //Переименование тега.
        static void RenameTag()
        {
            string fullpath, newtagname;

            Console.Clear();

            Console.WriteLine("Введите полный путь тега, который требуется переименовать:");
            fullpath = Console.ReadLine();

            Console.WriteLine("Новое имя тега:");
            newtagname = Console.ReadLine();

            TagStrg.RenameTag(fullpath, newtagname);

            Menu();
        }

    }
}
