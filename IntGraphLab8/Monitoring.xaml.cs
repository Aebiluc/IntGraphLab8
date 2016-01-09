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

        public Global Global { get; set; }

        public bool IsConveyorOn { get; private set; }
        public bool IsBucketLocked { get; private set; }
        public ColorTank CurrentDeliveredColor { get; private set; }
       

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
                    IsConveyorOn = Global.Machine.ConveyorOn;
                    IsBucketLocked = Global.Machine.BucketLocked;
                    CurrentDeliveredColor = Global.Machine.ColorTank;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        TextBlockConnection.Text = "Connecté";
                        Global.Machine.Connected = true;

                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        if (IsConveyorOn)
                            bi3.UriSource = new Uri("Image/gear_on.png", UriKind.Relative);
                        else
                            bi3.UriSource = new Uri("Image/gear_off.png", UriKind.Relative);
                        bi3.EndInit();

                        ConveyorGear.Source = bi3;

                        if (IsBucketLocked)
                            ImageBucketLocked.Opacity = 1;
                        else
                            ImageBucketLocked.Opacity = 0.2;


                        BitmapImage bi2 = new BitmapImage();
                        bi2.BeginInit();

                        switch (CurrentDeliveredColor)
                        {
                            case ColorTank.A:
                                bi2.UriSource = new Uri("Image/BlueBucket.png", UriKind.Relative);
                                ImageDeliveryBucket.Visibility = Visibility.Visible;
                                break;

                            case ColorTank.B:
                                bi2.UriSource = new Uri("Image/GreenBucket.png", UriKind.Relative);
                                ImageDeliveryBucket.Visibility = Visibility.Visible;
                                break;

                            case ColorTank.C:
                                bi2.UriSource = new Uri("Image/YellowBucket.png", UriKind.Relative);
                                ImageDeliveryBucket.Visibility = Visibility.Visible;
                                break;

                            case ColorTank.D:
                                bi2.UriSource = new Uri("Image/OrangeBucket.png", UriKind.Relative);
                                ImageDeliveryBucket.Visibility = Visibility.Visible;
                                break;

                            case ColorTank.NONE:
                                bi2.UriSource = new Uri("Image/OrangeBucket.png", UriKind.Relative);
                                ImageDeliveryBucket.Visibility = Visibility.Hidden;
                                break;
                        }

                        bi2.EndInit();
                        ImageDeliveryBucket.Source = bi2;

                        TextBlockTotalBucket.Text = Global.Config.TotalBucket.ToString();

                        if (Global.RecipeExecute)
                            ButtonNewBucket.IsEnabled = false;
                        else
                            ButtonNewBucket.IsEnabled = true;

                    }));
                }
                catch
                {
                    Dispatcher.Invoke(new Action(() => 
                    {
                        TextBlockConnection.Text = "Non-Connecté";
                        Global.Machine.Connected = false;
                    }));
                }
                Global.SemaphoreMachine.Release();
            }
        }

        private void ButtonNewBucket_Click(object sender, RoutedEventArgs e)
        {
            Global.ThreadMachine.Suspend();
            Global.Machine.StartConveyor();
            Global.ThreadMachine.Resume();
        }
    }
}
