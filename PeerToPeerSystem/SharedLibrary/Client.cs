using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
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