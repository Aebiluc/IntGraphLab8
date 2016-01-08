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
            if (recipe.NbLot != 0)
                Global.SemaphoreRecipe.Release();
        }


        public void RecipeExecute()
        {
            Global.SemaphoreRecipe.Wait();
            Global.SemaphoreMachine.Wait();
            Global.Machine.BucketLoadingEnabled = true;
            Global.Machine.StartConveyor();
            Global.SemaphoreMachine.Release();
            Global.SemaphoreRecipe.Release();
            while (true)
            {
                Global.SemaphoreRecipe.Wait();
                //active le convoyeur avec l'arrivée des sauts

                foreach (Lot lot in recipe.items)
                {
                    for (int i = 0; i < lot.NbBuckets; i++)
                    {
                        long start;
                        double temps;

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

                        /*    10ml/s --> temps = Qté/10 *1000 [ms]    */

                        //Tank A
                        temps = 100 * lot.Quantity[0];
                        Global.SemaphoreMachine.Wait();
                        Global.Machine.SetColorTank = ColorTank.A;
                        Global.SemaphoreMachine.Release();
                        start = DateTime.Now.Ticks;
                        while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < temps)
                            Thread.Sleep(10);

                        //Tank B
                        temps = 100 * lot.Quantity[1];
                        Global.SemaphoreMachine.Wait();
                        Global.Machine.SetColorTank = ColorTank.B;
                        Global.SemaphoreMachine.Release();
                        start = DateTime.Now.Ticks;
                        while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < temps)
                            Thread.Sleep(10);

                        //Tank C
                        temps = 100 * lot.Quantity[2];
                        Global.SemaphoreMachine.Wait();
                        Global.Machine.SetColorTank = ColorTank.C;
                        Global.SemaphoreMachine.Release();
                        start = DateTime.Now.Ticks;
                        while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < temps)
                            Thread.Sleep(10);

                        //Tank D
                        temps = 100 * lot.Quantity[3];
                        Global.SemaphoreMachine.Wait();
                        Global.Machine.SetColorTank = ColorTank.D;
                        Global.SemaphoreMachine.Release();
                        start = DateTime.Now.Ticks;
                        while ((double)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond < temps)
                            Thread.Sleep(10);

                        //Tank None et relancement du convoyeur
                        Global.SemaphoreMachine.Wait();
                        Global.Machine.SetColorTank = ColorTank.NONE;
                        Global.Machine.StartConveyor();
                        Global.SemaphoreMachine.Release();
                    }
                }
            }
        }
    }
}
