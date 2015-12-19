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

namespace IntGraphLab8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(1);
        }

        private void ButtonJob_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(2);
        }

        private void ButtonMonitor_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(3);
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            PageManagement(4);
        }

        private void PageManagement(int numPage)
        {
            PageStart.Visibility = Visibility.Collapsed;
            PageJob.Visibility = Visibility.Collapsed;
            PageMonitoring.Visibility = Visibility.Collapsed;
            PageConfig.Visibility = Visibility.Collapsed;

            switch(numPage)
            {
                case 1:
                    PageStart.Visibility = Visibility.Visible;
                    break;

                case 2:
                    PageJob.Visibility = Visibility.Visible;
                    break;

                case 3:
                    PageMonitoring.Visibility = Visibility.Visible;
                    break;

                case 4:
                    PageConfig.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
