using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMAchine;
using System.Windows.Threading;
using System.Threading;

namespace IntGraphLab8
{
    public delegate void MachineConnStateChange(bool status);

    public class MachineManagement
    {
        public Machine machine { set; get; }

        public Mutex MutexMachine { set; get; }

        private bool exit = false;
        private bool _isConnected;

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

        public MachineConnStateChange MachineConnState { get; set; }

        private void CheckMachineConn(object sender, EventArgs e)
        {
            try
            {
                bool test = machine.ConveyorOn;
                _isConnected = true;
                MachineConnState(true);
            }
            catch
            {
                _isConnected = false;
                MachineConnState(false);
            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

    }
}
