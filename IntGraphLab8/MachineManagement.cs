using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMAchine;
using System.Windows.Threading;

namespace IntGraphLab8
{
    public class MachineManagement
    {
        public Machine machine { set; get; }
        bool exit = false;

        public void Work()
        {
            DispatcherTimer MachineAvaliable;

            machine = new Machine("127.0.0.1", 9999);

            MachineAvaliable = new DispatcherTimer();
            MachineAvaliable.Interval = TimeSpan.FromMilliseconds(20);
            MachineAvaliable.Tick += CheckMachineConn;
            MachineAvaliable.IsEnabled = true;

            while (!exit)
            {

            }

        }

        public void RequestStop()
        {
            exit = true;
        }

        public delegate void MachineConnStateChange();
        public MachineConnStateChange MachineConnState { get; set; }

        private void CheckMachineConn(object sender, EventArgs e)
        {
            try
            {
                bool test = machine.ConveyorOn;
            }
            catch
            {
               
            }
        }

    }
}
