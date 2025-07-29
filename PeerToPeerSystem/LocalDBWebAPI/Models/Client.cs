namespace LocalDBWebAPI.Models
{
    public class Client
    {
        // Unique identifier for the client
        public int ID { get; set; }

        // Client's IP address
        public string IPAddress { get; set; }

        // Client's port
        public int Port { get; set; }
    }
}

