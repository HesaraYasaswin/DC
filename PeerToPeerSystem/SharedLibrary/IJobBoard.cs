using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharedLibrary
{
    public interface IJobBoard
    {
        List<Job> GetAvailableJobs(); // Method to get available jobs
        void SubmitJob(Job job); // Method to submit a job
        void SubmitAnswer(int jobId, string result); // Method to submit a job result
        int GetCompletedJobsCount(); // Method to get the count of completed jobs
    }
}
