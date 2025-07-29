using LocalDBWebAPI.Models;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LocalDBWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult RegisterClient([FromBody] Client client)
        {
            if (client == null || string.IsNullOrEmpty(client.IPAddress) || client.Port <= 0)
            {
                return BadRequest(new { Message = "Invalid client data." });
            }

            if (DBManager.InsertClient(client))
            {
                return Ok(new { Message = "Client registered successfully." });
            }

            return BadRequest(new { Message = "Error in registering client." });
        }

        [HttpDelete("unregister/{ipAddress}")]
        public IActionResult UnregisterClient(string ipAddress)
        {
            if (DBManager.RemoveClientByIP(ipAddress))
            {
                return Ok(new { Message = "Client unregistered successfully." });
            }

            return BadRequest(new { Message = "Error in unregistering client." });
        }

        [HttpGet("list")]
        public IActionResult GetClients()
        {
            List<Client> clients = DBManager.GetAllClients();
            if (clients == null || clients.Count == 0)
            {
                return Ok(new { Message = "No clients found." }); // Still returning 200 OK for no clients
            }

            return Ok(clients); // Return clients list with 200 OK
        }

        [HttpGet("{ipAddress}/jobcount")]
        public IActionResult GetJobCountForClient(string ipAddress)
        {
            try
            {
                var client = DBManager.GetClientByIPAddress(ipAddress);
                if (client == null)
                {
                    return NotFound(new { Message = "Client not found." });
                }

                int jobCount = DBManager.GetJobCountForClient(client.ID);
                return Ok(new { JobCount = jobCount }); // Returning the job count in a structured format
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error occurred: {ex.Message}" }); // Use 500 for server error
            }
        }

        [HttpPost("jobcompleted")]
        public IActionResult JobCompleted([FromBody] Job job)
        {
            if (job == null || job.ClientId <= 0 || string.IsNullOrEmpty(job.JobData))
            {
                return BadRequest(new { Message = "Invalid job data." });
            }

            if (DBManager.InsertJob(job))
            {
                return Ok(new { Message = "Job data saved successfully." });
            }

            return BadRequest(new { Message = "Error in saving job data." });
        }

        [HttpGet("completedjobs")]
        public IActionResult GetCompletedJobs()
        {
            List<Job> jobs = DBManager.GetAllCompletedJobs();
            if (jobs.Count == 0)
            {
                return Ok(new { Message = "No completed jobs found." }); // Return 200 OK for no completed jobs
            }

            return Ok(jobs); // Return jobs with 200 OK
        }

        [HttpGet("jobs/{clientId}")]
        public IActionResult GetJobsForClient(int clientId)
        {
            List<Job> jobs = DBManager.GetJobsForClient(clientId);
            if (jobs == null || jobs.Count == 0)
            {
                return NotFound(new { Message = "No jobs found for the client." });
            }
            return Ok(jobs); // Return the list of jobs with 200 OK
        }


        [HttpPatch("job/updatestatus/{jobId}")]
        public IActionResult UpdateJobStatus(int jobId, [FromBody] bool isCompleted)
        {
            if (DBManager.UpdateJobStatus(jobId, isCompleted))
            {
                return Ok(new { Message = "Job status updated successfully." });
            }
            return BadRequest(new { Message = "Error in updating job status." });
        }
    }
}
