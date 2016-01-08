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

        public Monitoring()
        {
            InitializeComponent();
        }


        /*                   Gestion de la connection avec la machine                   */
        public void MachineExecute()
        {
            Global.MutexRecipe.WaitOne(); //blocage pour l'exectuion de la recette
            Global.ThreadRecipe.Start();


            while (true)
            {
                Thread.Sleep(100);
                Global.MutexMachine.WaitOne();
                try
                {
                    bool tmp = Global.Machine.ConveyorOn;
                    ;//changement sur l'interface
                }
                catch
                {
                    ;//changement sur l'interface
                }
                Global.MutexMachine.ReleaseMutex();
            }
        }
        /*                   Gestion de la connection avec la machine                   */
    }
}
