using System.Diagnostics;
using WebsiteView.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SharedLibrary;

namespace WebsiteView.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Starting client data fetch from Web API...");

            // Update the URL to match the correct Web Service API endpoint
            RestClient restClient = new RestClient("http://localhost:8123/");
            RestRequest restRequest = new RestRequest("api/Client/list", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);

            _logger.LogInformation($"API Call Status: {restResponse.StatusCode}, Content: {restResponse.Content}");

            // Check if the response is successful
            if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
            {
                try
                {
                    // Attempt to deserialize the response as a list of clients
                    if (restResponse.Content.Contains("message"))
                    {
                        var responseMessage = JsonConvert.DeserializeObject<dynamic>(restResponse.Content);
                        _logger.LogInformation($"Message from API: {responseMessage.message}");
                        return View(new MyModel { Clients = new List<Client>(), Stats = new List<Work_Stat>() });
                    }
                    else
                    {
                        List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(restResponse.Content);
                        _logger.LogInformation($"Clients fetched successfully: {clients?.Count} client(s) found.");

                        MyModel model = new MyModel
                        {
                            Clients = clients ?? new List<Client>(),
                            Stats = new List<Work_Stat>()
                        };

                        if (clients != null)
                        {
                            foreach (Client client in clients)
                            {
                                _logger.LogInformation($"Fetching completed job count for client IP: {client.IPAddress}");
                                Work_Stat stat = new Work_Stat
                                {
                                    Client = client,
                                    NumCompleted = GetCompletedJobCount(client.IPAddress) // Use IP Address instead of ID
                                };
                                _logger.LogInformation($"Client IP: {client.IPAddress}, Completed Jobs: {stat.NumCompleted}");
                                model.Stats.Add(stat);
                            }
                        }

                        return View(model);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Error deserializing client data: {ex.Message}");
                }
            }
            else
            {
                _logger.LogError($"Error fetching clients: {restResponse.StatusCode}, {restResponse.Content}");
            }

            return View(new MyModel { Clients = new List<Client>(), Stats = new List<Work_Stat>() });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetCompletedJobCount(string ipAddress)
        {
            _logger.LogInformation($"Starting job count fetch for client IP: {ipAddress}");

            // Update this URL to match your Web Service API endpoint
            RestClient restClient = new RestClient("http://localhost:8123/");
            RestRequest restRequest = new RestRequest($"api/client/{ipAddress}/jobcount", Method.Get); // Use the new endpoint
            RestResponse restResponse = restClient.Execute(restRequest);

            _logger.LogInformation($"API Call Status (Jobs): {restResponse.StatusCode}, Content: {restResponse.Content}");

            if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<dynamic>(restResponse.Content);
                    int jobCount = response.JobCount ?? 0;

                    _logger.LogInformation($"Client IP: {ipAddress}, Completed Jobs Count: {jobCount}");
                    return jobCount;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Error deserializing job count data for client {ipAddress}: {ex.Message}");
                }
            }
            else
            {
                _logger.LogError($"Error fetching job count for client {ipAddress}: {restResponse.StatusCode}, {restResponse.Content}");
            }

            return 0; // Return 0 if an error occurs
        }
    }
}
