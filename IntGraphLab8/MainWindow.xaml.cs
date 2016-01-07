using APIMAchine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml;

namespace IntGraphLab8
{
    public partial class MainWindow : Window
    {
        User CurrentUser;
        ProgrammeConfig Config;

        Thread threadMachineManagement;
        MachineManagement machineWorker;

        static Mutex mutexMachine = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
            PageManagement(1);
            CurrentUser = new User(UserType.None);
            PageStart.SelectedUser = CurrentUser;
            PageStart.ButtonValidateAction = UserManagement;
            ButtonConfig.IsEnabled = false;

            Config = new ProgrammeConfig();
            try
            {
                LoadConfigFile();
                PageConfig.ConfigFile = Config;
            }
            catch
            {
                MessageBox.Show("Erreur lors du chargement du fichier de configuration");
                SaveConfigFile();
            }

            mutexMachine.WaitOne();

            machineWorker = new MachineManagement();
            threadMachineManagement = new Thread(machineWorker.Work);
            threadMachineManagement.Start();
        }

        private void UserManagement(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.UserStatus == UserType.Admin)
                ButtonConfig.IsEnabled = true;
            else
                ButtonConfig.IsEnabled = false;

            ButtonJob_Click(sender, e);
        }

        private void LoadConfigFile()
        {
            using (XmlReader reader = XmlReader.Create("Config.XML"))
            {
                Config.ImportXML(reader);
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(1);
        }

        private void ButtonJob_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(2);
        }

        private void ButtonMonitor_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(3);
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(4);
        }

        private void PageManagement(int numPage)
        {
            PageStart.Visibility = Visibility.Collapsed;
            PageJob.Visibility = Visibility.Collapsed;
            PageMonitoring.Visibility = Visibility.Collapsed;
            PageConfig.Visibility = Visibility.Collapsed;

            switch(numPage)
            {
                case 1:
                    PageStart.Visibility = Visibility.Visible;
                    break;

                case 2:
                    PageJob.Visibility = Visibility.Visible;
                    break;

                case 3:
                    PageMonitoring.Visibility = Visibility.Visible;
                    break;

                case 4:
                    PageConfig.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void SaveConfigFile()
        {
            using (XmlWriter writer = XmlWriter.Create("Config.XML"))
            {
                Config.ExportXML(writer);
            }
        }

        private void mainWindowsClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfigFile();
            machineWorker.RequestStop();
            threadMachineManagement.Join();
        }     
    }
}
