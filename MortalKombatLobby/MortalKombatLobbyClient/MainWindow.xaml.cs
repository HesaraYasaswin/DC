using System.ServiceModel;
using System.Windows;
using LobbyCommon;

namespace MortalKombatLobbyClient
{
    public partial class MainWindow : Window
    {
        private IGamingLobby _proxy;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize connection to server
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = 65536 * 1024; // Increase the message size to 64 MB
            ChannelFactory<IGamingLobby> factory = new ChannelFactory<IGamingLobby>(binding, "net.tcp://localhost:8100/GamingLobbyService");
            _proxy = factory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            if (!string.IsNullOrEmpty(username) && _proxy.Login(username))
            {
                MessageBox.Show("Login Successful", "Mortal Kombat Lobby", MessageBoxButton.OK, MessageBoxImage.Information);
                LobbySelectionWindow lobbySelection = new LobbySelectionWindow(_proxy, username);
                lobbySelection.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Username is already taken or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
