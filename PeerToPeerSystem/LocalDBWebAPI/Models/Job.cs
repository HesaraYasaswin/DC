namespace LocalDBWebAPI.Models
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
    }
}
