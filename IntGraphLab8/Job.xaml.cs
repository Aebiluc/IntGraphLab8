using APIMAchine;
using Microsoft.Win32;
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
using System.Xml;

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Job.xaml
    /// </summary>
    public partial class Job : UserControl
    {
        public Global Global { get; set; }

        private Recipe recipe;

        public Job()
        {
            InitializeComponent();
            recipe = new Recipe();
        }

        private void ButtonOpenRecipe_Click(object sender, RoutedEventArgs e)
        {
            ListBoxRecipe.Items.Clear();
            recipe.Clear();

            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "Recettes\\";
            dlg.Filter = "xml files (*.xml)|*.xml|recipe files (*.rcp)|*.rcp|All files (*.*)|*.*";

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


        private void ButtonEditRecipe_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo exeRepiceEdit =
                new System.Diagnostics.ProcessStartInfo();
            exeRepiceEdit.FileName = "Labo6.exe";
            exeRepiceEdit.WorkingDirectory = "";
            System.Diagnostics.Process.Start("Labo6.exe");
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            //donne l'autorisation d'executer la recette
            if (recipe.NbLot != 0)
                Global.SemaphoreRecipe.Release();
        }


        /*      Thread avec la machine d'état pour l'execution de la recette      */
        public void RecipeExecute()
        {
            int index = 0, totBucket = 0;

            Global.SemaphoreRecipe.Wait();
            if (Global.Machine.Connected)
            {
                Global.SemaphoreMachine.Wait();

                //active le convoyeur avec l'arrivée des sauts
                Global.Machine.BucketLoadingEnabled = true;
                Global.Machine.StartConveyor();


                Global.SemaphoreMachine.Release();
            }
            Global.SemaphoreRecipe.Release();



            while (true)
            {
                //attente du signal entre deux execution de recette
                Global.SemaphoreRecipe.Wait();

                //Check que la machine soit connectée
                Global.SemaphoreMachine.Wait();
                bool connect = Global.Machine.Connected;
                Global.SemaphoreMachine.Release();

                if (connect)
                {
                    totBucket = 0;
                    foreach (Lot lot in recipe.items)
                        totBucket += lot.NbBuckets;
                    index = 0;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        ProgressBarProgress.Value = 0;
                    }));

                    foreach (Lot lot in recipe.items)
                    {
                        index++;
                        /*Actualisation du visuel*/
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TextBlockLot.Text = index.ToString() + '/' + recipe.NbLot.ToString();
                            BorderColorLot.Background = ColorToBrush(FindFinalColor(lot));
                        }));
                        /*Actualisation du visuel*/

                        for (int i = 0; i < lot.NbBuckets; i++)
                        {
                            long start;
                            //double temps;

                            //attente d'un saut
                            while (true)
                            {
                                Global.SemaphoreMachine.Wait();
                                if (Global.Machine.BucketLocked)
                                {
                                    Global.SemaphoreMachine.Release();
                                    break;
                                }
                                Global.SemaphoreMachine.Release();
                                Thread.Sleep(100);
                            }

                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProgressBarProgress.Value = ProgressBarProgress.Value + 0.2 / totBucket * 100;
                            }));

                            /*    10ml/s --> temps = Qté/10 *1000 [ms]    */

                            //Tank A
                            Global.Time = 100 * lot.Quantity[0];
                            Global.SemaphoreMachine.Wait();
                            Global.Machine.ColorTank = ColorTank.A;
                            Global.SemaphoreMachine.Release();
                            start = DateTime.Now.Ticks;
                            while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < Global.Time)
                                Thread.Sleep(10);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProgressBarProgress.Value = ProgressBarProgress.Value + 0.2 / totBucket * 100;
                            }));

                            //Tank B
                            Global.Time = 100 * lot.Quantity[1];
                            Global.SemaphoreMachine.Wait();
                            Global.Machine.ColorTank = ColorTank.B;
                            Global.SemaphoreMachine.Release();
                            start = DateTime.Now.Ticks;
                            while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < Global.Time)
                                Thread.Sleep(10);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProgressBarProgress.Value = ProgressBarProgress.Value + 0.2 / totBucket * 100;
                            }));

                            //Tank C
                            Global.Time = 100 * lot.Quantity[2];
                            Global.SemaphoreMachine.Wait();
                            Global.Machine.ColorTank = ColorTank.C;
                            Global.SemaphoreMachine.Release();
                            start = DateTime.Now.Ticks;
                            while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < Global.Time)
                                Thread.Sleep(10);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProgressBarProgress.Value = ProgressBarProgress.Value + 0.2 / totBucket * 100;
                            }));

                            //Tank D
                            Global.Time = 100 * lot.Quantity[3];
                            Global.SemaphoreMachine.Wait();
                            Global.Machine.ColorTank = ColorTank.D;
                            Global.SemaphoreMachine.Release();
                            start = DateTime.Now.Ticks;
                            while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < Global.Time)
                                Thread.Sleep(10);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProgressBarProgress.Value = ProgressBarProgress.Value + 0.2 / totBucket * 100;
                            }));

                            //Tank None et relancement du convoyeur
                            Global.SemaphoreMachine.Wait();
                            Global.Machine.ColorTank = ColorTank.NONE;
                            Global.Machine.StartConveyor();
                            Global.SemaphoreMachine.Release();
                        }
                    }
                }else
                    MessageBox.Show("Impossible d'executer la recette.\nLa machine ne répond pas", "Aucune connection avec la machine", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        Color PaintingColorBlue = Color.FromArgb(255, 69, 134, 191);
        Color PaintingColorGreen = Color.FromArgb(255, 125, 185, 105);
        Color PaintingColorYellow = Color.FromArgb(255, 253, 240, 2);
        Color PaintingColorOrange = Color.FromArgb(255, 242, 146, 5);
        const double MixCoefficient = 0.95;
        const double debitColor = 10; //    ml/s

        public Brush ColorToBrush(Color c)
        {
            return new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
        }

        public Color MixColors(Color colorA, Color colorB)
        {
            return Color.FromArgb(
                255,
                (byte)(MixCoefficient * colorA.R + (1 - MixCoefficient) * colorB.R),
                (byte)(MixCoefficient * colorA.G + (1 - MixCoefficient) * colorB.G),
                (byte)(MixCoefficient * colorA.B + (1 - MixCoefficient) * colorB.B));
        }

        public Color FindFinalColor(Lot lot)
        {
            int n, i;
            Color color = Color.FromArgb(255, 255, 255, 255);
            n = (int)(lot.Quantity[0] / (debitColor * 0.25));
            for (i = 0; i < n; i++)
                color = MixColors(color, PaintingColorBlue);

            n = (int)(lot.Quantity[1] / (debitColor * 0.25));
            for (i = 0; i < n; i++)
                color = MixColors(color, PaintingColorGreen);

            n = (int)(lot.Quantity[2] / (debitColor * 0.25));
            for (i = 0; i < n; i++)
                color = MixColors(color, PaintingColorOrange);

            n = (int)(lot.Quantity[3] / (debitColor * 0.25));
            for (i = 0; i < n; i++)
                color = MixColors(color, PaintingColorYellow);

            return color;
        }
    }
}
