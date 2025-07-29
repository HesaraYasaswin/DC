using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using LobbyCommon;
using System;

namespace MortalKombatLobbyClient
{
    public partial class LobbySelectionWindow : Window
    {
        private IGamingLobby _proxy;
        private string _username;
        private bool _isUpdating = true;

        public LobbySelectionWindow(IGamingLobby proxy, string username)
        {
            InitializeComponent();
            _proxy = proxy;
            _username = username;
            LoadLobbies();
            StartUpdating();
        }

        private async void StartUpdating()
        {
            await Task.Run(async () =>
            {
                while (_isUpdating)
                {
                    await Dispatcher.InvokeAsync(() => LoadLobbies());
                    await Task.Delay(5000); // Poll every 5 seconds
                }
            });
        }

        private void LoadLobbies()
        {
            LobbiesListBox.ItemsSource = _proxy.GetLobbies();
        }

        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedLobby = LobbiesListBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedLobby) && _proxy.JoinLobby(selectedLobby, _username))
            {
                _isUpdating = false;
                LobbyWindow lobbyWindow = new LobbyWindow(_proxy, _username, selectedLobby);
                lobbyWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to join the lobby. It may no longer exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            string lobbyName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the new lobby:", "Create Lobby", "");
            if (!string.IsNullOrEmpty(lobbyName) && _proxy.CreateLobby(lobbyName))
            {
                LoadLobbies();
                MessageBox.Show("Lobby created successfully.", "Mortal Kombat Lobby", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Lobby creation failed. A lobby with that name may already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadLobbies();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _isUpdating = false; // Stop the thread when closing the window
        }
    }
}
