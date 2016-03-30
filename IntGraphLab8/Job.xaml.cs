﻿using APIMAchine;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            ListBoxRecipe.ItemsSource = recipe.items;
        }

        private void ButtonOpenRecipe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Global.Config.FilePath;
            dlg.Filter = "xml/recipe files (*.xml, *.rcp)|*.xml;*.rcp|All files (*.*)|*.*";
            
            if (dlg.ShowDialog() == true)
            {
                recipe.Clear();
                using (XmlReader reader = XmlReader.Create(dlg.FileName))
                {
                    try
                    {
                        recipe.ImportXML(reader);
                    }
                    catch
                    {
                        MessageBox.Show("Impossible d'ouvrir le fichier de recette");
                        return;
                    }
                }
            }
        }

        private void ButtonEditRecipe_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo exeRepiceEdit =
                new System.Diagnostics.ProcessStartInfo();
            exeRepiceEdit.FileName = "Labo6.exe";
            exeRepiceEdit.WorkingDirectory = "";
            try
            {
                System.Diagnostics.Process.Start("Labo6.exe");
            }
            catch
            {
                MessageBox.Show("Impossible de trouver le programme d'édition de recette");
            }
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            //donne l'autorisation d'executer la recette
            if (recipe.NbLot != 0)
            {
                Global.SemaphoreRecipe.Release();
                Global.RecipeExecuted = true;
                ButtonEditRecipe.IsEnabled = false;
                ButtonOpenRecipe.IsEnabled = false;
                ButtonExecute.IsEnabled = false;
                ButtonAbort.IsEnabled = true;
            }
        }


        private void ButtonAbort_Click(object sender, RoutedEventArgs e)
        {
            Global.ThreadRecipe.Abort();
            //Global.ThreadRecipe.Join();
            //Global.ThreadRecipe.Interrupt();
            Global.ThreadRecipe = new Thread(new ThreadStart(RecipeExecute));
            //Global.ThreadMachine.Start();
        }

        /*      Thread avec la machine d'état pour l'execution de la recette      */
        public void RecipeExecute()
        {
            int indexLot = 0, indexBucket = 0, totBucket = 0;
            double totQuantity = 0;
            Global.Timer = new System.Timers.Timer(1000);
            Global.Timer.Elapsed += TimerTicks;

            /* Première initialisation */
            while(!FirstInitRecipeExec());


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
                    /* init */
                    totBucket = 0;
                    totQuantity = 0;
                    foreach (Lot lot in recipe.items)
                    {
                        totBucket += lot.NbBuckets;
                        for (int i = 0; i < lot.Quantity.Length; i++)
                            totQuantity += lot.Quantity[i]*lot.NbBuckets;
                    }

                    TimeRecipe(totQuantity, totBucket);

                    indexLot = 0;
                    indexBucket = 0;

                    /* Execution de la recette */
                    foreach (Lot lot in recipe.items)
                    {
                        indexLot++;
                       
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TextBlockLot.Text = indexLot.ToString() + '/' + recipe.NbLot.ToString();
                            BorderColorLot.Background = ColorToBrush(FindFinalColor(lot));
                            TextBlockBucket.Text = indexBucket.ToString() + '/' + totBucket.ToString();
                        }));

                        for (int i = 0; i < lot.NbBuckets; i++)
                        {
                            long start;
                            Global.Config.TotalBucket++;
                            //attente d'un saut
                            while (true){
                                Global.SemaphoreMachine.Wait();
                                if (Global.Machine.BucketLocked){
                                    Global.SemaphoreMachine.Release();
                                    break;
                                }
                                Global.SemaphoreMachine.Release();
                                Thread.Sleep(100);
                            }
                            //Start le timer du décompte
                            Global.Timer.Start();

                            indexBucket++;
                            Dispatcher.Invoke(new Action(() =>
                            {
                                TextBlockBucket.Text = indexBucket.ToString() + '/' + totBucket.ToString();
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
                }
                else
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Impossible d'executer la recette.\nLa machine ne répond pas", "Aucune connection avec la machine", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                //Finalisation à la fin de la recette
                EndRecipe();
            }
        }

        private void TimerTicks(object sender, EventArgs e)
        {
            string[] tab;
            int seconds, mins, hours;
            DateTime time = new DateTime(0);
            Dispatcher.Invoke(new Action(() =>
            {
                tab = TextBlockRestTime.Text.Split(':');
                seconds = int.Parse(tab[tab.Length - 1]);
                mins = int.Parse(tab[tab.Length - 2]);
                hours = int.Parse(tab[tab.Length - 3]);
                if (seconds == 0)
                {
                    mins = int.Parse(tab[tab.Length - 2]);
                    if (mins == 0)
                    {
                        hours = int.Parse(tab[tab.Length - 3]);
                        if (hours == 0)
                            return;
                        hours--;
                        mins = 59;
                        seconds = 59;
                    }
                    else
                    {
                        mins--;
                        seconds = 59;
                    }
                }
                else
                    seconds--;
                time = time.AddSeconds(seconds);
                time = time.AddMinutes(mins);
                time = time.AddHours(hours);
                TextBlockRestTime.Text = "Temps restant : " + time.ToLongTimeString();
            }));
        }

        private bool FirstInitRecipeExec()
        {
            Global.SemaphoreRecipe.Wait();
            Global.SemaphoreMachine.Wait();
            if (Global.Machine.Connected)
            {

                //active le convoyeur avec l'arrivée des sauts
                Global.Machine.BucketLoadingEnabled = true;
                if (!Global.Machine.BucketLocked)
                    Global.Machine.StartConveyor();

                Global.SemaphoreMachine.Release();
                Global.SemaphoreRecipe.Release();
                return true;
            }
            else
            {
                Global.SemaphoreMachine.Release();
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("Impossible d'executer la recette.\nLa machine ne répond pas", "Aucune connection avec la machine", MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
                EndRecipe();
                return false;
            }

        }

        private void TimeRecipe(double totQuantity, int totBucket)
        {
            DateTime estimateTime = new DateTime(0);

            estimateTime = estimateTime.AddMilliseconds((int)(totQuantity / 10 * 1000));
            estimateTime = estimateTime.AddMilliseconds(2730 * (totBucket - 1)); //temps moyen pour passer d'un saut à un autre

            Dispatcher.Invoke(new Action(() =>
            {
                ProgressBarProgress.Value = 0;
                TextBlockTotTime.Text = "Temps estimé : " + estimateTime.ToLongTimeString();
                TextBlockRestTime.Text = "Temps restant : " + estimateTime.ToLongTimeString();
            }));
        }

        private void EndRecipe()
        {
            Global.RecipeExecuted = false;
            Dispatcher.Invoke(new Action(() =>
            {
                ButtonEditRecipe.IsEnabled = true;
                ButtonOpenRecipe.IsEnabled = true;
                ButtonExecute.IsEnabled = true;
                ButtonAbort.IsEnabled = false;
            }));
            Global.Timer.Stop();
        }

        /*      final color     */
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
