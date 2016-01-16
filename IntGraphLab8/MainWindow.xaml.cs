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
        public ProgrammeConfig Config { get; set; }
        public double Time { get; set; }
        public bool RecipeExecuted { get; set; }
        public System.Timers.Timer Timer { get; set; }

        public Global()
        {
            SemaphoreMachine = new SemaphoreSlim(1);
            SemaphoreRecipe = new SemaphoreSlim(0);
            Config = new ProgrammeConfig();
        }
    }


    public partial class MainWindow : Window
    {
        User CurrentUser;
        Global global;
        ProgrammeConfig Config;

        private bool _convoyor;
        private ColorTank _color;
        private long _ticksStop;

        public MainWindow()
        {
            InitializeComponent();
            PageManagement(1);
            PageStart.ButtonUserAction = UserManagement;
            ButtonConfig.IsEnabled = false;
            ButtonRecette.IsEnabled = false;
            ButtonMachine.IsEnabled = false;

            ButtonStop.Tag = false;
            
            //initialisation pour les variables globales
            global = new Global();

            Config = global.Config;
            LoadConfigFile();

            PageJob.Global = global;
            PageMonitoring.Global = global;

            PageStart.Config = Config;

            PageConfig.ConfigFile = Config;
            PageConfig.SaveFile += SaveConfigFile;

            global.RecipeExecuted = false;

            global.ThreadMachine = new Thread(PageMonitoring.MachineExecute);
            global.ThreadRecipe = new Thread(PageJob.RecipeExecute);
            global.Machine = new Machine("127.0.0.1", 9999);

            global.ThreadMachine.Start();
            global.ThreadRecipe.Start();
        }

        private void UserManagement(object sender, RoutedEventArgs e)
        {
            CurrentUser = PageStart.SelectedUser;
            PageConfig.CurrentUser = CurrentUser;
            if (CurrentUser.UserStatus != UserType.None)
            {
                ButtonConfig.IsEnabled = true;
                ButtonMachine.IsEnabled = true;
                ButtonRecette.IsEnabled = true;

                ButtonJobPage_Click(sender, e);
            }

            else {
                ButtonConfig.IsEnabled = false;
                ButtonMachine.IsEnabled = false;
                ButtonRecette.IsEnabled = false;
            }
        }

        private void LoadConfigFile()
        {
            using (XmlReader reader = XmlReader.Create("Config.XML"))
            {
                try
                {
                    Config.ImportXML(reader);
                }
                catch
                {
                    MessageBox.Show("Impossible d'ouvrir le fichier de configuration");
                }
            }
        }

        private void ButtonStartPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(1);

            ButtonConnexion.BorderThickness = new Thickness(2.0);
            ButtonRecette.BorderThickness = new Thickness(0);
            ButtonMachine.BorderThickness = new Thickness(0);
            ButtonConfig.BorderThickness = new Thickness(0);
        }

        private void ButtonJobPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(2);

            ButtonConnexion.BorderThickness = new Thickness(0);
            ButtonRecette.BorderThickness = new Thickness(2.0);
            ButtonMachine.BorderThickness = new Thickness(0);
            ButtonConfig.BorderThickness = new Thickness(0);
        }

        private void ButtonMonitorPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(3);

            ButtonConnexion.BorderThickness = new Thickness(0);
            ButtonRecette.BorderThickness = new Thickness(0);
            ButtonMachine.BorderThickness = new Thickness(2.0);
            ButtonConfig.BorderThickness = new Thickness(0);
        }

        private void ButtonConfigPage_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(4);

            ButtonConnexion.BorderThickness = new Thickness(0);
            ButtonRecette.BorderThickness = new Thickness(0);
            ButtonMachine.BorderThickness = new Thickness(0);
            ButtonConfig.BorderThickness = new Thickness(2.0);
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
            //if (global.ThreadRecipe.ThreadState == ThreadState.Suspended)
            //    global.ThreadRecipe.Resume();
            //if (global.ThreadMachine.ThreadState == ThreadState.Suspended)
            //    global.ThreadMachine.Resume();
            //global.ThreadRecipe.Abort();
            //global.ThreadMachine.Abort();
            global.ThreadRecipe.Interrupt();
            global.ThreadMachine.Interrupt();
            if (global.Machine.Connected)
            {
                global.Machine.StopConveyor();
                global.Machine.ColorTank = ColorTank.NONE;
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if((bool?)ButtonStop.Tag == false)
            {
                ButtonStop.Tag = true;
                global.ThreadRecipe.Suspend();
                //Suspension du thread machine pour éviter les acces concurrant à la machine
                global.ThreadMachine.Suspend();
                if (global.Machine.Connected)
                {
                    _convoyor = global.Machine.ConveyorOn;
                    _color = global.Machine.ColorTank;
                    if (_color != ColorTank.NONE || _convoyor == true || global.RecipeExecuted)
                        ButtonStart.IsEnabled = true;
                    global.Machine.StopConveyor();
                    global.Machine.ColorTank = ColorTank.NONE;
                }
                global.ThreadMachine.Resume();
                if (global.RecipeExecuted)
                {
                    _ticksStop = DateTime.Now.Ticks;
                    global.Timer.Stop();
                }
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //Suspension du thread machine pour éviter les accent concurrant à la machine
            global.ThreadMachine.Suspend();
            if (global.Machine.Connected)
            {
                if (_convoyor)
                    global.Machine.StartConveyor();
                global.Machine.ColorTank = _color;
            }
            global.ThreadMachine.Resume();
            global.Timer.Start();
            _ticksStop = DateTime.Now.Ticks - _ticksStop;
            global.Time += (double)_ticksStop / TimeSpan.TicksPerMillisecond;
            global.ThreadRecipe.Resume();
            ButtonStart.IsEnabled = false;
            ButtonStop.Tag = false;
        }
    }
}
