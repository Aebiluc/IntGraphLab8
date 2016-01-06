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

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for Monitoring.xaml
    /// </summary>
    public partial class Monitoring : UserControl
    {
        private Machine _machine;

        public Machine machine
        {
            set
            {
                _machine = value;
            }
        }
        public Monitoring()
        {
            InitializeComponent();
        }
    }
}
