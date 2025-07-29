using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary
{
    public class JobBoard : MarshalByRefObject, IJobBoard
    {
        private readonly List<Job> _jobs; // List to store jobs
        private readonly object _lock = new object(); // Synchronization object

        public JobBoard()
        {
            _jobs = new List<Job>(); // Initialize job list
        }

        public List<Job> GetAvailableJobs()
        {
            lock (_lock) // Ensure thread safety
            {
                return _jobs.Where(job => !job.IsCompleted).ToList(); // Return only incomplete jobs
            }
        }

        public void SubmitJob(Job job)
        {
            lock (_lock) // Ensure thread safety
            {
                _jobs.Add(job); // Add new job to the list
                Console.WriteLine($"New job submitted: {job.JobData}");
            }
        }

        public int GetCompletedJobsCount()
        {
            lock (_lock) // Ensure thread safety
            {
                return _jobs.Count(job => job.IsCompleted); // Return count of completed jobs
            }
        }

        public void SubmitAnswer(int jobId, string result)
        {
            lock (_lock) // Ensure thread safety
            {
                var job = _jobs.FirstOrDefault(j => j.ID == jobId);
                if (job != null && !job.IsCompleted)
                {
                    job.IsCompleted = true; // Mark job as completed
                    job.Result = result; // Save the result to the job
                    Console.WriteLine($"Job {jobId} completed with result: {result}");
                }
            }
        }
    }
}
