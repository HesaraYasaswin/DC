using System.IO;
using System.Windows;
using Microsoft.Win32;
using LobbyCommon;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MortalKombatLobbyClient
{
    public partial class LobbyWindow : Window
    {
        private IGamingLobby _proxy;
        private string _username;
        private string _lobbyName;
        private bool _isUpdating = true;

        public LobbyWindow(IGamingLobby proxy, string username, string lobbyName)
        {
            InitializeComponent();
            _proxy = proxy;
            _username = username;
            _lobbyName = lobbyName;
            LoadLobbyData();
            StartUpdating();
        }

        private async void StartUpdating()
        {
            await Task.Run(async () =>
            {
                while (_isUpdating)
                {
                    await Dispatcher.InvokeAsync(() => LoadLobbyData());
                    await Task.Delay(5000); // Poll every 5 seconds
                }
            });
        }

        private void LoadLobbyData()
        {
            // Load messages and display them in the listbox
            var messages = _proxy.GetLobbyMessages(_lobbyName);
            var sharedFiles = _proxy.GetSharedFiles(_lobbyName);

            var allMessages = messages.Select(m => m.ToString()).ToList();
            allMessages.AddRange(sharedFiles.Select(f => f.ToString()));

            MessagesListBox.ItemsSource = allMessages;

        
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(message))
            {
                _proxy.SendMessage(_lobbyName, message, _username);
                MessageTextBox.Clear();
                LoadLobbyData(); // Refresh messages
            }
        }

        private void ShareFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                byte[] fileData = File.ReadAllBytes(fileName);
                _proxy.ShareFile(_lobbyName, fileData, Path.GetFileName(fileName), _username);
                MessageBox.Show("File shared successfully.", "Mortal Kombat Lobby", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadLobbyData(); // Refresh messages to show the shared file
            }
        }

        private void MessagesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MessagesListBox.SelectedItem != null)
            {
                string selectedItem = MessagesListBox.SelectedItem.ToString();
                if (selectedItem.StartsWith("[File Shared]"))
                {
                    var sharedFiles = _proxy.GetSharedFiles(_lobbyName);
                    string fileName = selectedItem.Split(':')[1].Trim();
                    var fileToDownload = sharedFiles.FirstOrDefault(f => f.FileName == fileName);

                    if (fileToDownload != null)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = fileToDownload.FileName;
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            File.WriteAllBytes(saveFileDialog.FileName, fileToDownload.FileData);
                            MessageBox.Show("File downloaded successfully.", "File Download", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadLobbyData();
        }

        private void PrivateMessageButton_Click(object sender, RoutedEventArgs e)
        {
            PrivateMessageWindow privateMessageWindow = new PrivateMessageWindow(_proxy, _username, _lobbyName);
            privateMessageWindow.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _isUpdating = false; // Stop the thread when navigating away
            _proxy.LeaveLobby(_lobbyName, _username);
            LobbySelectionWindow lobbySelection = new LobbySelectionWindow(_proxy, _username);
            lobbySelection.Show();
            this.Close();
        }
    }
}
