using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LobbyCommon;

namespace MortalKombatLobbyClient
{
    public partial class PrivateMessageWindow : Window
    {
        private IGamingLobby _proxy;
        private string _sender;
        private string _currentLobby;

        private bool _isUpdating = true;

        public PrivateMessageWindow(IGamingLobby proxy, string sender, string lobbyName)
        {
            InitializeComponent();
            _proxy = proxy;
            _sender = sender;
            _currentLobby = lobbyName;
            LoadUsers();
            LoadPrivateMessages();
            StartUpdating();
        }

        private async void StartUpdating()
        {
            await Task.Run(async () =>
            {
                while (_isUpdating)
                {
                    await Dispatcher.InvokeAsync(() => LoadPrivateMessages());
                    await Task.Delay(5000); // Poll every 5 seconds
                }
            });
        }

        private void LoadUsers()
        {
            // Load online users from the current lobby except the sender
            List<string> users = _proxy.GetUsersInLobby(_currentLobby).Where(u => u != _sender).ToList();
            UsersComboBox.ItemsSource = users;
            if (users.Count > 0)
                UsersComboBox.SelectedIndex = 0;
        }

        private void LoadPrivateMessages()
        {
            // Retrieve private messages where the current user is the recipient
            var privateMessagesReceived = _proxy.GetPrivateMessages(_sender);
            PrivateMessagesListBox.ItemsSource = privateMessagesReceived.Select(m => m.ToString());
        }

        private void SendPrivateMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string recipient = UsersComboBox.SelectedItem as string;
            string message = PrivateMessageTextBox.Text;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(recipient))
            {
                _proxy.SendPrivateMessage(recipient, message, _sender);
                PrivateMessageTextBox.Clear();
                LoadPrivateMessages(); // Refresh messages
            }
            else
            {
                MessageBox.Show("Please select a user and enter a message.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void RefreshMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPrivateMessages();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _isUpdating = false; // Stop the thread when navigating away
            this.Close();
        }
    }
}
