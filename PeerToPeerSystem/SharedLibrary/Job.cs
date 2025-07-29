using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SharedLibrary
{
    public class Job
    {
        // Unique identifier for the job
        public int ID { get; set; }

        // Foreign key - ClientID (the client that owns this job)
        public int ClientId { get; set; }

        // Data associated with the job (job description, task details, etc.)
        public string JobData { get; set; }

        // Status of the job (true = completed, false = in progress)
        public bool IsCompleted { get; set; }
        public string Result { get; set; }

        // Method to get the job's status
        public string GetStatus()
        {
            return IsCompleted ? "Completed" : "In Progress";
        }
    }
}
