using Microsoft.Win32;
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
using System.Xml;

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Job.xaml
    /// </summary>
    public partial class Job : UserControl
    {
        private Recipe recipe;

        public Job()
        {
            InitializeComponent();
            recipe = new Recipe();
        }

        private void ButtonOpenRecipe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "Recettes\\";
            dlg.Filter = "xml files (*.xml)|*.xml";

            if (dlg.ShowDialog() == true)
            {
                using (XmlReader reader = XmlReader.Create(dlg.FileName))
                    recipe.ImportXML(reader);
                foreach(Lot lot in recipe.items)
                {
                    ListBoxRecipe.Items.Add(lot.ToString());
                }
            }
        }
    }
}
