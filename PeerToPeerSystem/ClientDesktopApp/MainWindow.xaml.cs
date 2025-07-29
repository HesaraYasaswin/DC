using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using SharedLibrary;
using RestSharp;

namespace ClientDesktopApp
{
    public partial class MainWindow : Window
    {
        private static ServerThread _serverThread; // Static ServerThread object for job submission
        private NetworkingThread _networkingThread;
        private Client _serverClient;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize client instance (will be set in Register)
            _serverClient = new Client();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Get IP address and port from user input
            string ipAddress = IPAddressTextBox.Text;
            string portText = PortTextBox.Text;
            int port;

            // Validate IP address and port
            if (string.IsNullOrWhiteSpace(ipAddress) || !int.TryParse(portText, out port))
            {
                StatusLabel.Text = "Status: Invalid IP address or port.";
                return;
            }

            // Set the client IP and port
            _serverClient.IPAddress = ipAddress;
            _serverClient.Port = port;

            try
            {
                // Create the networking thread instance
                using (var restClient = new RestClient("http://localhost:8123/")) // Use HTTP for local development
                {
                    var request = new RestRequest("api/Client/register", Method.Post);
                    request.AddJsonBody(_serverClient); // Add the client data

                    // Send the request asynchronously
                    var response = await restClient.ExecuteAsync(request);

                    if(response.IsSuccessful)
                    {
                        StatusLabel.Text = "Status: Registered successfully!";

                        // Instantiate the ServerThread after successful registration
                        if (_serverThread == null) // Check if it hasn't been instantiated
                        {
                            _serverThread = new ServerThread();
                            _serverThread.Start(); // Start the server thread
                        }
                    }
                    else
                    {
                        // Log detailed error information
                        StatusLabel.Text = $"Error registering client: {response.ErrorMessage} (Status Code: {response.StatusCode})";
                        // Additional information for debugging
                        if (response.StatusCode == 0)
                        {
                            StatusLabel.Text += " - Possible connection issue. Check server status and network connectivity.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Status: Registration failed - {ex.Message}";
            }
        }




        private void SubmitJob_Click(object sender, RoutedEventArgs e)
        {
            string pythonCode = CodeInputTextBox.Text;
            if (!string.IsNullOrEmpty(pythonCode))
            {
                _serverThread.SubmitJob(pythonCode); // Submit the job via static ServerThread object
                StatusLabel.Text = "Status: Job submitted!";
            }
            else
            {
                StatusLabel.Text = "Status: Job submission failed. Empty input.";
            }
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to load Python code from a file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Python Files (*.py)|*.py|All Files (*.*)|*.*",
                Title = "Load Python Code File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Read the file content and set it to the TextBox
                string filePath = openFileDialog.FileName;
                try
                {
                    string code = File.ReadAllText(filePath);
                    CodeInputTextBox.Text = code;
                    StatusLabel.Text = "Status: File loaded successfully!";
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = $"Status: Error loading file - {ex.Message}";
                }
            }
        }

        private void JobStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a specific job ID, for example, the last submitted job's ID.
            var job = _serverThread.GetLastSubmittedJob(); // Add this method in ServerThread
            if (job != null)
            {
                string serverJobStatus = _serverThread.GetJobStatus(job.ID); // Get job status from ServerThread

                // Update the UI with the job status
                JobCountLabel.Text = $"Job Status: {serverJobStatus}";
                StatusLabel.Text = "Status: Job status checked!";
            }
            else
            {
                StatusLabel.Text = "Status: No jobs submitted.";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _networkingThread.Stop(); // Stop the networking thread when closing the application
        }
    }
}
