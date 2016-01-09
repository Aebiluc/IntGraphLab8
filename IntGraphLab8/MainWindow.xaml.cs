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
    public class Global
    {
        public Thread ThreadRecipe { get; set; }
        public Thread ThreadMachine { get; set; }
        public SemaphoreSlim SemaphoreMachine { get; set; }
        public SemaphoreSlim SemaphoreRecipe { get; set; }
        public Machine Machine { get; set; }
        public double Time { get; set; }
        public bool RecipeExecute { get; set; }

        public Global()
        {
            SemaphoreMachine = new SemaphoreSlim(1);
            SemaphoreRecipe = new SemaphoreSlim(0);
        }
    }


    public partial class MainWindow : Window
    {
        User CurrentUser;
        ProgrammeConfig Config;
        Global global;

        private bool _convoyor;
        private ColorTank _color;
        private long _ticksStop;

        public MainWindow()
        {
            InitializeComponent();
            PageManagement(1);
            CurrentUser = new User(UserType.None);
            PageStart.SelectedUser = CurrentUser;
            PageStart.ButtonValidateAction = UserManagement;
            ButtonConfig.IsEnabled = false;
            ButtonStart.IsEnabled = false;

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
            //initialisation pour les variables globales
            global = new Global();
            PageJob.Global = global;
            PageMonitoring.Global = global;

            global.RecipeExecute = false;

            global.ThreadMachine = new Thread(PageMonitoring.MachineExecute);
            global.ThreadRecipe = new Thread(PageJob.RecipeExecute);
            global.Machine = new Machine("127.0.0.1", 9999);

            global.ThreadMachine.Start();
            global.ThreadRecipe.Start();
        }

        private void UserManagement(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.UserStatus == UserType.Admin)
                ButtonConfig.IsEnabled = true;
            else
                ButtonConfig.IsEnabled = false;

            ButtonJobPage_Click(sender, e);
        }

        private void LoadConfigFile()
        {
            using (XmlReader reader = XmlReader.Create("Config.XML"))
            {
                Config.ImportXML(reader);
            }
        }

        private void ButtonStartPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(1);
        }

        private void ButtonJobPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(2);
        }

        private void ButtonMonitorPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(3);
        }

        private void ButtonConfigPage_Click(object sender, RoutedEventArgs e)
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
            global.ThreadRecipe.Abort();
            global.ThreadMachine.Abort();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (global.RecipeExecute)
            {
                _ticksStop = DateTime.Now.Ticks;
                global.ThreadRecipe.Suspend();
                //Suspension du thread machine pour éviter les accent concurrant à la machine
                global.ThreadMachine.Suspend();
                _convoyor = global.Machine.ConveyorOn;
                _color = global.Machine.ColorTank;
                global.Machine.StopConveyor();
                global.Machine.ColorTank = ColorTank.NONE;
                global.ThreadMachine.Resume();
                ButtonStart.IsEnabled = true;
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //Suspension du thread machine pour éviter les accent concurrant à la machine
            global.ThreadMachine.Suspend();
            if (_convoyor)
                global.Machine.StartConveyor();
            global.Machine.ColorTank = _color;
            global.ThreadMachine.Resume();
            _ticksStop = DateTime.Now.Ticks - _ticksStop;
            global.Time += (double)_ticksStop / TimeSpan.TicksPerMillisecond;
            global.ThreadRecipe.Resume();
            ButtonStart.IsEnabled = false;
        }
    }
}
