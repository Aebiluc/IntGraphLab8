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
using APIMAchine;
using System.Threading;
using System.Windows.Threading;

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Monitoring.xaml
    /// </summary>
    public partial class Monitoring : UserControl
    {

        private Machine _machine;
        private bool ConveyorLoading = false;

        public Global Global { get; set; }


        public Monitoring()
        {
            InitializeComponent();

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Image/gear_off.png", UriKind.Relative);
            bi3.EndInit();

            ConveyorGear.Source = bi3;

        }


        /*                   Gestion de la connection avec la machine                   */
        public void MachineExecute()
        {
            Global.MutexRecipe.WaitOne(); //blocage pour l'exectuion de la recette
            Global.ThreadRecipe.Start();

            DispatcherTimer machineConnectionCheck = new DispatcherTimer();
            machineConnectionCheck.Tick += new EventHandler(MachineConnectionCheck_Tick);
            machineConnectionCheck.Interval = new TimeSpan(0, 0, 0, 100);
            machineConnectionCheck.Start();
            while (!Global.Exit)
                Thread.Sleep(10);
        }

        public void MachineConnectionCheck_Tick(object sender, EventArgs e)
        {
            Global.MutexMachine.WaitOne();
            try {
                bool tmp = Global.Machine.ConveyorOn;
                ;//changement sur l'interface
            } catch {
                ;//changement sur l'interface
            }
            Global.MutexMachine.ReleaseMutex();
        }
        /*                   Gestion de la connection avec la machine                   */
    }
}
