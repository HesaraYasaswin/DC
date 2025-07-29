using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using LobbyCommon;

namespace GamingLobbyServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class GamingLobbyServer : IGamingLobby
    {
        private static List<Player> _players = new List<Player>();
        private static List<Lobby> _lobbies = new List<Lobby>();
        private static Dictionary<string, List<Message>> _lobbyMessages = new Dictionary<string, List<Message>>();
        private static Dictionary<string, List<SharedFile>> _lobbyFiles = new Dictionary<string, List<SharedFile>>();

        public bool Login(string username)
        {
            if (_players.Any(p => p.Username == username))
            {
                return false; // Username already taken
            }

            _players.Add(new Player { Username = username });
            Console.WriteLine($"{username} has logged in.");
            return true;
        }

        public bool Logout(string username)
        {
            var player = _players.FirstOrDefault(p => p.Username == username);
            if (player != null)
            {
                if (!string.IsNullOrEmpty(player.CurrentLobby))
                {
                    LeaveLobby(player.CurrentLobby, username);
                }
                _players.Remove(player);
                Console.WriteLine($"{username} has logged out.");
                return true;
            }
            return false;
        }

        public bool CreateLobby(string lobbyName)
        {
            if (_lobbies.Any(l => l.Name == lobbyName))
            {
                return false; // Lobby with the same name already exists
            }

            _lobbies.Add(new Lobby { Name = lobbyName });
            _lobbyMessages[lobbyName] = new List<Message>(); // Initialize message list
            _lobbyFiles[lobbyName] = new List<SharedFile>(); // Initialize file list
            Console.WriteLine($"Lobby '{lobbyName}' created.");
            return true;
        }

        public bool JoinLobby(string lobbyName, string username)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);
            if (lobby == null)
            {
                return false; // Lobby does not exist
            }

            var player = _players.FirstOrDefault(p => p.Username == username);
            if (player != null)
            {
                if (!string.IsNullOrEmpty(player.CurrentLobby))
                {
                    LeaveLobby(player.CurrentLobby, username);
                }
                lobby.Players.Add(player);
                player.CurrentLobby = lobbyName;
                Console.WriteLine($"{username} joined lobby '{lobbyName}'.");
                return true;
            }
            return false;
        }

        public bool LeaveLobby(string lobbyName, string username)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);
            var player = _players.FirstOrDefault(p => p.Username == username);
            if (lobby != null && player != null)
            {
                lobby.Players.Remove(player);
                player.CurrentLobby = null;
                Console.WriteLine($"{username} left lobby '{lobbyName}'.");
                return true;
            }
            return false;
        }

        public void SendMessage(string lobbyName, string message, string username)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);
            if (lobby != null)
            {
                var newMessage = new Message
                {
                    Sender = username,
                    Content = message,
                    Timestamp = DateTime.Now,
                    IsPrivate = false
                };

                _lobbyMessages[lobbyName].Add(newMessage); // Store message in memory
                Console.WriteLine($"[{lobbyName}] {username}: {message}");
            }
        }

        public void SendPrivateMessage(string recipient, string message, string sender)
        {
            var recipientPlayer = _players.FirstOrDefault(p => p.Username == recipient);
            if (recipientPlayer != null)
            {
                var newMessage = new Message
                {
                    Sender = sender,
                    Content = message,
                    Timestamp = DateTime.Now,
                    IsPrivate = true,
                    Recipient = recipient
                };

                _lobbyMessages[recipientPlayer.CurrentLobby].Add(newMessage); // Store private message
                Console.WriteLine($"[Private] {sender} to {recipient}: {message}");
            }
        }

        public List<string> GetLobbies()
        {
            return _lobbies.Select(l => l.Name).ToList();
        }

        public List<string> GetUsersInLobby(string lobbyName)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);
            return lobby != null ? lobby.Players.Select(p => p.Username).ToList() : new List<string>();
        }

        public List<Message> GetLobbyMessages(string lobbyName)
        {
            if (_lobbyMessages.ContainsKey(lobbyName))
            {
                return _lobbyMessages[lobbyName].Where(m => !m.IsPrivate).ToList();
            }
            return new List<Message>();
        }

        public List<Message> GetPrivateMessages(string recipient)
        {
            return _lobbyMessages.Values.SelectMany(m => m)
                .Where(m => m.IsPrivate && m.Recipient == recipient)
                .ToList();
        }

        public void ShareFile(string lobbyName, byte[] fileData, string fileName, string username)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);
            if (lobby != null)
            {
                var sharedFile = new SharedFile
                {
                    FileName = fileName,
                    FileData = fileData,
                    Sender = username,
                    Timestamp = DateTime.Now
                };

                _lobbyFiles[lobbyName].Add(sharedFile); // Store file in memory
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SharedFiles", lobbyName);
                Directory.CreateDirectory(directoryPath); // Ensure directory exists
                string filePath = Path.Combine(directoryPath, fileName);
                File.WriteAllBytes(filePath, fileData);

                Console.WriteLine($"[{lobbyName}] {username} shared a file: {fileName}");
            }
        }

        public List<SharedFile> GetSharedFiles(string lobbyName)
        {
            if (_lobbyFiles.ContainsKey(lobbyName))
            {
                return _lobbyFiles[lobbyName];
            }
            return new List<SharedFile>();
        }
    }
}
