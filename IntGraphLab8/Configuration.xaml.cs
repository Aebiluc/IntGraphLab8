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
using Microsoft.Win32;

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    /// 
    public partial class Configuration : UserControl
    {
        private ProgrammeConfig _configFile;
        private User _CurrentUser;

        public User CurrentUser {
            private get {return _CurrentUser; }
            set
            {
                _CurrentUser = value;
                if (_CurrentUser.UserStatus == UserType.Admin)
                    GroupBoxRecipeConfig.Visibility = Visibility.Visible;
                else
                    GroupBoxRecipeConfig.Visibility = Visibility.Collapsed;
            }

        }

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

        private void ButtonChangeFilePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            // Do not allow the user to create new files via the FolderBrowserDialog.
            dlg.ShowNewFolderButton = false;

            // Default to the My Documents folder.
            //dlg.RootFolder = Environment.SpecialFolder.Personal;
            dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ConfigFilePath.Text = dlg.SelectedPath;
            }
        }
    }
}
