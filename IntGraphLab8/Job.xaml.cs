﻿using APIMAchine;
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
            dlg.Filter = "xml files (*.xml)|*.xml";

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

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            //donne l'autorisation d'executer la recette
        }


        public void RecipeExecute()
        {
            while (true)
            {
                Global.MutexRecipe.WaitOne();
                foreach (Lot lot in recipe.items)
                {
                    for (int i = 0; i < lot.NbBuckets; i++)
                    {
                        long start;
                        int[] temps = new int[4];

                        //10ms correspondent à 0.4ml --> temps = 10*Qté/0.4

                        //attente d'un saut
                        while (true)
                        {
                            Global.MutexMachine.WaitOne();
                            if (Global.Machine.BucketLocked)
                            {
                                Global.MutexMachine.ReleaseMutex();
                                break;
                            }
                            Global.MutexMachine.ReleaseMutex();
                            Thread.Sleep(100);
                        }

                        //Tank A
                        Global.MutexMachine.WaitOne();
                        Global.Machine.SetColorTank = ColorTank.A;
                        Global.MutexMachine.ReleaseMutex();
                        start = DateTime.Now.Ticks;
                        while ((DateTime.Now.Ticks - start) < temps[0])
                            Thread.Sleep(10);

                        //Tank B
                        Global.MutexMachine.WaitOne();
                        Global.Machine.SetColorTank = ColorTank.B;
                        Global.MutexMachine.ReleaseMutex();
                        start = DateTime.Now.Ticks;
                        while ((DateTime.Now.Ticks - start) < temps[0])
                            Thread.Sleep(10);

                        //Tank C
                        Global.MutexMachine.WaitOne();
                        Global.Machine.SetColorTank = ColorTank.C;
                        Global.MutexMachine.ReleaseMutex();
                        start = DateTime.Now.Ticks;
                        while ((DateTime.Now.Ticks - start) < temps[0])
                            Thread.Sleep(10);

                        //Tank D
                        Global.MutexMachine.WaitOne();
                        Global.Machine.SetColorTank = ColorTank.D;
                        Global.MutexMachine.ReleaseMutex();
                        start = DateTime.Now.Ticks;
                        while ((DateTime.Now.Ticks - start) < temps[0])
                            Thread.Sleep(10);

                        //Tank None
                        Global.MutexMachine.WaitOne();
                        Global.Machine.SetColorTank = ColorTank.NONE;
                        Global.MutexMachine.ReleaseMutex();
                    }
                }
            }
        }
    }
}
