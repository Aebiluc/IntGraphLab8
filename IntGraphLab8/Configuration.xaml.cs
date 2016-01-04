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
using IntGraphLab8;

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    /// 
    public partial class Configuration : UserControl
    {
        public ProgrammeConfig _ConfigFile;

        public ProgrammeConfig ConfigFile
        {
            set
            {
                _ConfigFile = value;
                ConfigFilePath.Text = value.FilePath;
            }

            get
            {
                return _ConfigFile;
            }
        }

        public Configuration()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ConfigFile.FilePath = ConfigFilePath.Text;
        }
    }
}
