using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntGraphLab8
{
    public delegate void ButtonUserManagement(object sender, RoutedEventArgs e);
    public partial class Start : UserControl
    {
        UserType tmp = new UserType();
        public Start()
        {
            InitializeComponent();
        }

        private void ButtonOperator_Click(object sender, RoutedEventArgs e)
        {
            tmp = UserType.Operator;
            buttonOperateur.Background = Brushes.DarkMagenta;
            ButtonManager.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
        }

        private void ButtonManager_Click(object sender, RoutedEventArgs e)
        {
            tmp = UserType.Manager;
            ButtonManager.Background = Brushes.DarkMagenta;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
        }

        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            tmp = UserType.Admin;
            ButtonAdmin.Background = Brushes.DarkMagenta;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonManager.Background = Brushes.LightGray;
        }

        public User SelectedUser { get; set; }
        public ButtonUserManagement ButtonUserAction { get; set; }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            ButtonDisconnect.IsEnabled = true;
            SelectedUser.UserStatus = tmp;
            ButtonUserAction(sender, e);
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ButtonDisconnect.IsEnabled = false;
            SelectedUser.UserStatus = UserType.None;
            ButtonUserAction(sender, e);
        }
    }
}
