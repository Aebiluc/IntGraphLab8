using System.Windows;
using System.Windows.Controls;

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
        }

        private void ButtonManager_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser.UserStatus = UserType.Manager;
        }

        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser.UserStatus = UserType.Admin;
        }

        public User SelectedUser { get; set; }
        public ButtonValidate ButtonValidateAction { get; set; }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            ButtonValidateAction(sender, e);
        }
    }
}
