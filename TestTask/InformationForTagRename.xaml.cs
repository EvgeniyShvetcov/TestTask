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
    /// Логика взаимодействия для InformationForTagRename.xaml
    /// </summary>
    public partial class InformationForTagRename : Window
    {
        private string NewTagName { get; set; }
        public InformationForTagRename()
        {
            InitializeComponent();
        }

        private void RenameTag_Click(object sender, RoutedEventArgs e)
        {
            NewTagName = newTagName.Text;
            this.Close();
        }

        public void GetTagInformation(out string newtagname)
        {
            newtagname = NewTagName;
        }
    }
}
