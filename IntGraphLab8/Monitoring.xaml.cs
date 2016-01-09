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
            while (true)
            {
                Thread.Sleep(100);
                Global.SemaphoreMachine.Wait();
                try
                {
                    bool tmp = Global.Machine.ConveyorOn;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        TextBlockConnection.Text = "Connecté";

                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        if (tmp)
                            bi3.UriSource = new Uri("Image/gear_on.png", UriKind.Relative);
                        else
                            bi3.UriSource = new Uri("Image/gear_off.png", UriKind.Relative);
                        bi3.EndInit();

                        ConveyorGear.Source = bi3;
                    }));
                    

                }
                catch
                {
                    Dispatcher.Invoke(new Action(() => { TextBlockConnection.Text = "Non-Connecté"; }));
                }
                Global.SemaphoreMachine.Release();
            }
        }
        /*                   Gestion de la connection avec la machine                   */
    }
}
