using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace LobbyCommon
{
    [ServiceContract]
    public interface IGamingLobby
    {
        [OperationContract]
        bool Login(string username);

        [OperationContract]
        bool Logout(string username);

        [OperationContract]
        bool CreateLobby(string lobbyName);

        [OperationContract]
        bool JoinLobby(string lobbyName, string username);

        [OperationContract]
        bool LeaveLobby(string lobbyName, string username);

        [OperationContract]
        void SendMessage(string lobbyName, string message, string username);

        [OperationContract]
        void SendPrivateMessage(string recipient, string message, string sender);

        [OperationContract]
        List<string> GetLobbies();

        [OperationContract]
        List<string> GetUsersInLobby(string lobbyName);

        [OperationContract]
        List<Message> GetLobbyMessages(string lobbyName);

        [OperationContract]
        List<Message> GetPrivateMessages(string recipient);

        [OperationContract]
        void ShareFile(string lobbyName, byte[] fileData, string fileName, string username);

        [OperationContract]
        List<SharedFile> GetSharedFiles(string lobbyName);
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsPrivate { get; set; }
        public string Recipient { get; set; }

        // Override ToString for proper display in UI
        public override string ToString()
        {
            if (IsPrivate)
            {
                return $"[Private] [{Timestamp}] {Sender} to {Recipient}: {Content}";
            }
            else
            {
                return $"[{Timestamp}] {Sender}: {Content}";
            }
        }
    }

    public class Player
    {
        public string Username { get; set; }
        public string CurrentLobby { get; set; }
    }

    public class Lobby
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
    }

    public class SharedFile
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"[File Shared] {Sender}: {FileName}";
        }
    }
}
