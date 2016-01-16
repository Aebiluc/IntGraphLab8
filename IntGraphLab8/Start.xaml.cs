using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntGraphLab8
{
    public delegate void ButtonUserManagement(object sender, RoutedEventArgs e);

    public partial class Start : UserControl
    {
        User _SelectedUser = new User();

        public ProgrammeConfig Config { get; set; }

        public Start()
        {
            InitializeComponent();
            SelectedUser = _SelectedUser;
        }

        private void ButtonOperator_Click(object sender, RoutedEventArgs e)
        {
            _SelectedUser.UserStatus = UserType.Operator;
            _SelectedUser.mdp = Config.MdpOperateur;
            
            buttonOperateur.Background = Brushes.DarkCyan;
            ButtonManager.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
            PassWord.Focus();
        }

        private void ButtonManager_Click(object sender, RoutedEventArgs e)
        {
            _SelectedUser.UserStatus = UserType.Manager;
            _SelectedUser.mdp = Config.MdpManager;

            ButtonManager.Background = Brushes.DarkCyan;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonAdmin.Background = Brushes.LightGray;
            PassWord.Focus();
        }

        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            _SelectedUser.UserStatus = UserType.Admin;
            _SelectedUser.mdp = Config.MdpAdmin;

            ButtonAdmin.Background = Brushes.DarkCyan;
            buttonOperateur.Background = Brushes.LightGray;
            ButtonManager.Background = Brushes.LightGray;
            PassWord.Focus();
        }

        public User SelectedUser { get; set; }
        public ButtonUserManagement ButtonUserAction { get; set; }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            if(_SelectedUser.UserStatus != UserType.None)
            {
                if (PassWord.Password == _SelectedUser.mdp)
                {
                    ButtonDisconnect.IsEnabled = true;
                    SelectedUser.UserStatus = _SelectedUser.UserStatus;
                    ButtonUserAction(sender, e);
                    SelectedUser = _SelectedUser;

                    ButtonValider.IsEnabled = false;
                    PassWord.IsEnabled = false;
                    switch (_SelectedUser.UserStatus)
                    {
                        case UserType.None:
                            break;
                        case UserType.Operator:
                            ButtonManager.Visibility = Visibility.Collapsed;
                            ButtonAdmin.Visibility = Visibility.Collapsed;
                            break;
                        case UserType.Manager:
                            ButtonAdmin.Visibility = Visibility.Collapsed;
                            buttonOperateur.Visibility = Visibility.Collapsed;
                            break;
                        case UserType.Admin:
                            buttonOperateur.Visibility = Visibility.Collapsed;
                            ButtonManager.Visibility = Visibility.Collapsed;
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    MessageBox.Show("Mot de passe incorrect", "Waring", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }  
            else
                MessageBox.Show("Aucun Utilisateur Sélectionné!", "Waring", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ButtonDisconnect.IsEnabled = false;
            _SelectedUser.UserStatus = UserType.None;
            _SelectedUser.mdp = "";
            SelectedUser = _SelectedUser;

            buttonOperateur.Visibility = Visibility.Visible;
            ButtonManager.Visibility = Visibility.Visible;
            ButtonAdmin.Visibility = Visibility.Visible;

            ButtonValider.IsEnabled = true;
            PassWord.IsEnabled = true;

            ButtonUserAction(sender, e);
        }
    }
}
