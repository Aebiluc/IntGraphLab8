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
        private ProgrammeConfig _configFile;
        public User CurrentUser { private get; set; }

        public delegate void SaveConfigFile();
        public SaveConfigFile SaveFile { get; set; }

        public ProgrammeConfig ConfigFile
        {
            set
            {
                _configFile = value;
                ConfigFilePath.Text = value.FilePath;
            }
        }

        public Configuration()
        {
            InitializeComponent();
            
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _configFile.FilePath = ConfigFilePath.Text;
            SaveFile();
        }

        private void ButtonChangeMdp_Click(object sender, RoutedEventArgs e)
        {
            if(PasswordBoxNew.Password == PasswordBoxNew.Password)
            {
                PasswordBoxNew.BorderBrush = Brushes.Green;
                PasswordBoxConfirm.BorderBrush = Brushes.Green;

                CurrentUser.mdp = PasswordBoxNew.Password;

                switch (CurrentUser.UserStatus)
                {
                    case UserType.None:
                        break;
                    case UserType.Operator:
                        _configFile.MdpOperateur = PasswordBoxNew.Password;
                        break;
                    case UserType.Manager:
                        _configFile.MdpManager = PasswordBoxNew.Password;
                        break;
                    case UserType.Admin:
                        _configFile.MdpAdmin = PasswordBoxNew.Password;
                        break;
                    default:
                        break;
                }
                SaveFile();

            }
            else
            {
                PasswordBoxNew.BorderBrush = Brushes.Red;
                PasswordBoxConfirm.BorderBrush = Brushes.Red;
            }
        }
    }
}
