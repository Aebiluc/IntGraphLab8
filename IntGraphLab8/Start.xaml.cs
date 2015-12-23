using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntGraphLab8
{
    public delegate void ButtonValidate(object sender, RoutedEventArgs e);
    public partial class Start : UserControl
    {
        public Start()
        {
            InitializeComponent();
        }

        private void ButtonOperator_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser.UserStatus = UserType.Operator;
            buttonOperateur.Background = Brushes.DarkGray;
            ButtonManager.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
        }

        private void ButtonManager_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser.UserStatus = UserType.Manager;
            ButtonManager.Background = Brushes.DarkGray;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
        }

        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser.UserStatus = UserType.Admin;
            ButtonAdmin.Background = Brushes.DarkGray;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonManager.Background = Brushes.LightGray;
        }

        public User SelectedUser { get; set; }
        public ButtonValidate ButtonValidateAction { get; set; }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            ButtonValidateAction(sender, e);
        }
    }
}
