using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Security.Cryptography;
using System.Threading;
using SharedLibrary;

namespace ClientDesktopApp
{
    internal class NetworkingThread
    {
        private Thread _thread;
        private bool _running;
        private int numJobsDone = 0;
        private List<Client> clients;
        private Client serverClient;
        private static Random rnd = new Random();
        private bool isBusy = false;
        private ServerThread _serverThread; // Reference to the server thread
        private IJobBoard _jobBoard; // Reference to IJobBoard

        public NetworkingThread(Client serverClient, ServerThread serverThread, IJobBoard jobBoard)
        {
            this.serverClient = serverClient;
            this._serverThread = serverThread; // Initialize server thread reference
            this._jobBoard = jobBoard; // Initialize IJobBoard reference
            clients = new List<Client>();
        }

        public void Start()
        {
            _running = true;
            _thread = new Thread(Run);
            _thread.Start();
        }

        private void Run()
        {
            while (_running)
            {
                RegisterClientAsync().Wait(); // Register client to the Web API
                LookForNewClientsAsync().Wait(); // Polling for new clients
                CheckClientsForJobs(); // Check if there are jobs from other clients
                Thread.Sleep(20000); // Poll every 20 seconds
            }
        }

        public void Stop()
        {
            _running = false;
            _thread.Join();
        }

        // Register the client to the Web API
        public async Task RegisterClientAsync()
        {
            RestClient restClient = new RestClient("https://localhost:8123/");
            RestRequest request = new RestRequest("api/Client/register", Method.Post);
            request.AddJsonBody(serverClient);

            RestResponse response = await restClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Client registered successfully.");
            }
            else
            {
                Console.WriteLine($"Error registering client: {response.ErrorMessage}");
            }
        }

        private async Task LookForNewClientsAsync()
        {
            RestClient restClient = new RestClient("https://localhost:8123/");
            RestRequest restRequest = new RestRequest("api/Client/list", Method.Get);
            RestResponse restResponse = await restClient.ExecuteAsync(restRequest);

            try
            {
                List<Client> clientsTemp = JsonConvert.DeserializeObject<List<Client>>(restResponse.Content);
                if (clientsTemp != null)
                {
                    clients = clientsTemp;
                }
                Console.WriteLine("NetworkingThread retrieved list of clients: " + clients.Count);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error retrieving clients: " + exc.Message);
            }
        }

        private void CheckClientsForJobs()
        {
            ShuffleList(clients);

            foreach (Client client in clients)
            {
                if (client.IPAddress == serverClient.IPAddress && client.Port == serverClient.Port)
                {
                    continue; // Skip current client
                }

                try
                {
                    var tcp = new NetTcpBinding();
                    var url = $"net.tcp://{client.IPAddress}:{client.Port}";
                    var factory = new ChannelFactory<IJobBoard>(tcp, url);
                    var clientNet = factory.CreateChannel();

                    Console.WriteLine($"Attempting to get job from: {client.IPAddress}:{client.Port}");
                    var availableJobs = clientNet.GetAvailableJobs(); // Use the method from IJobBoard

                    foreach (var job in availableJobs)
                    {
                        if (!job.IsCompleted) // Check if the job is not completed
                        {
                            // Validate the job using SHA256
                            string jobDataBase64 = job.JobData; // Assume JobData is already Base64 encoded
                            string decodedWork = DecodeFromBase64(jobDataBase64);
                            byte[] hash = ComputeSHA256Hash(jobDataBase64);

                            if (VerifyHash(decodedWork, hash))
                            {
                                // Execute the job and get the result
                                string resultString = _serverThread.PerformTask(decodedWork);

                                // Submit the answer back to the client
                                clientNet.SubmitAnswer(job.ID, resultString); // Assuming SubmitAnswer() takes job ID and result string
                                numJobsDone++;

                                // Notify job completion to the web server
                                NotifyJobCompletionAsync(job, resultString).Wait();
                            }
                            else
                            {
                                Console.WriteLine("Hash does not match for job.");
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Exception while checking for jobs from {client.IPAddress}:{client.Port}: {exc.Message}");
                }
            }
        }

        private void ShuffleList(List<Client> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Client value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private byte[] ComputeSHA256Hash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private bool VerifyHash(string data, byte[] originalHash)
        {
            byte[] computedHash = ComputeSHA256Hash(data);
            return Convert.ToBase64String(computedHash) == Convert.ToBase64String(originalHash);
        }

        private string DecodeFromBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

        // New method to notify job completion
        private async Task NotifyJobCompletionAsync(Job job, string result)
        {
            RestClient restClient = new RestClient("https://localhost:8123/");
            RestRequest request = new RestRequest("api/Client/jobcompleted", Method.Post);
            request.AddJsonBody(new { ClientId = job.ClientId, JobData = result }); // Adjust as necessary

            RestResponse response = await restClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine($"Job {job.ID} completed and notified successfully.");
            }
            else
            {
                Console.WriteLine($"Error notifying completion for job {job.ID}: {response.ErrorMessage}");
            }
        }


    }
}
