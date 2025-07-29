using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Http.Json;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using SharedLibrary;

namespace ClientDesktopApp
{
    public class ServerThread
    {
        private TcpChannel _channel;
        private const int Port = 8080;
        private readonly JobBoard _jobBoard;
        private int _completedJobsCount; // Add counter for completed jobs

        public ServerThread()
        {
            _jobBoard = new JobBoard(); // Initialize JobBoard
            _completedJobsCount = 0; // Initialize completed jobs counter
        }

        public void Start()
        {
            _channel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(_channel, true);
            RemotingServices.Marshal(_jobBoard, "JobBoard");
            Console.WriteLine("Server is running and waiting for connections...");
        }

        public void SubmitJob(string pythonCode)
        {
            string jobResult = PerformTask(pythonCode);

            Job job = new Job
            {
                JobData = jobResult,
                IsCompleted = true,
                ClientId = 6 
            };

            _jobBoard.SubmitJob(job);

            // Increment completed jobs counter when the job is completed
            _completedJobsCount++;

            SaveJobToDatabase(job);
        }

        public int GetCompletedJobsCount()
        {
            return _completedJobsCount; // Expose the completed jobs count
        }

        private void SaveJobToDatabase(Job job)
        {
            // Create an HTTP client to call the JobCompleted endpoint
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8123/"); // Your API base URL
                var response = client.PostAsJsonAsync("api/client/jobcompleted", job).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Job saved to the database successfully.");
                }
                else
                {
                    Console.WriteLine("Error saving job to the database: " + response.ReasonPhrase);
                }
            }
        }

        // Method to get the status of a specific job
        public string GetJobStatus(int jobId)
        {
            var job = _jobBoard.GetAvailableJobs().FirstOrDefault(j => j.ID == jobId);
            return job?.GetStatus() ?? "Job not found"; // Use the method in Job class
        }

        // Move IronPython logic here
        public string PerformTask(string jobCode)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.Execute(jobCode, scope);
                dynamic runFunction = scope.GetVariable("run_func");
                var result = runFunction();

                Console.WriteLine(result);
                return result.ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error executing Python code: " + exc.Message);
                return "failed";
            }
        }

        // Method to encode data to Base64
        private string EncodeToBase64(string data)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(textBytes);
        }

        public Job GetLastSubmittedJob()
        {
            // Get the available jobs and return the last submitted job based on ID
            var availableJobs = _jobBoard.GetAvailableJobs();
            return availableJobs.OrderByDescending(j => j.ID).FirstOrDefault(); // Return the last submitted job
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(_channel);
        }
    }
}
