using LocalDBWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace LocalDBWebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=mydatabase.db;Version=3;";

        // Initialize the database with required tables (Clients and Jobs)
        public static bool InitializeDatabase()
        {
            try
            {
                ClearDatabase(); // Clear the database before initializing

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create Clients table
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Clients (
                           ID INTEGER PRIMARY KEY AUTOINCREMENT,
                           IPAddress TEXT NOT NULL,
                           Port INTEGER NOT NULL
                        )";
                        command.ExecuteNonQuery();
                    }

                    // Create Jobs table
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Jobs (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ClientID INTEGER NOT NULL,
                            JobData TEXT NOT NULL,
                            IsCompleted INTEGER NOT NULL DEFAULT 0,
                            FOREIGN KEY(ClientID) REFERENCES Clients(ID)
                        )";
                        command.ExecuteNonQuery();
                    }

                    // Seed data - Insert clients and jobs
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Insert 5 clients
                        command.CommandText = @"
                        INSERT INTO Clients (IPAddress, Port) VALUES ('192.168.1.1', 8080);
                        INSERT INTO Clients (IPAddress, Port) VALUES ('192.168.1.2', 8081);
                        INSERT INTO Clients (IPAddress, Port) VALUES ('192.168.1.3', 8082);
                        INSERT INTO Clients (IPAddress, Port) VALUES ('192.168.1.4', 8083);
                        INSERT INTO Clients (IPAddress, Port) VALUES ('192.168.1.5', 8084);";
                        command.ExecuteNonQuery();

                        // Insert jobs for each client
                        command.CommandText = @"
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted) VALUES (1, 'Job data for Client 1', 0);
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted) VALUES (2, 'Job data for Client 2', 1);
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted) VALUES (3, 'Job data for Client 3', 0);
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted) VALUES (4, 'Job data for Client 4', 1);
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted) VALUES (5, 'Job data for Client 5', 0);";
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                Console.WriteLine("Tables created and seed data inserted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }


        // Method to clear all data from the database
        // Method to clear all data from the database and reset IDs
        private static void ClearDatabase()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Delete all records from Jobs and Clients tables
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Jobs"; // Clear jobs first to avoid foreign key constraint
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Clients"; // Now clear clients
                        command.ExecuteNonQuery();
                    }

                    // Reset the auto-increment IDs
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM sqlite_sequence WHERE name = 'Jobs'";
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM sqlite_sequence WHERE name = 'Clients'";
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                Console.WriteLine("Database cleared and IDs reset successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing database: " + ex.Message);
            }
        }


        // Insert a new client into the Clients table
        public static bool InsertClient(Client client)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO Clients (IPAddress, Port)
                        VALUES (@IPAddress, @Port)";
                        command.Parameters.AddWithValue("@IPAddress", client.IPAddress);
                        command.Parameters.AddWithValue("@Port", client.Port);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // Remove a client by IP address from the Clients table
        public static bool RemoveClientByIP(string ipAddress)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Clients WHERE IPAddress = @IPAddress";
                        command.Parameters.AddWithValue("@IPAddress", ipAddress);

                        int rowsDeleted = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsDeleted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // Get all clients from the Clients table
        public static List<Client> GetAllClients()
        {
            List<Client> clients = new List<Client>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Clients";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Client client = new Client();
                                client.ID = Convert.ToInt32(reader["ID"]);
                                client.IPAddress = reader["IPAddress"].ToString();
                                client.Port = Convert.ToInt32(reader["Port"]);

                                clients.Add(client);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return clients;
        }

        // Get client by IP address
        public static Client GetClientByIPAddress(string ipAddress)
        {
            Client client = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Clients WHERE IPAddress = @IPAddress";
                        command.Parameters.AddWithValue("@IPAddress", ipAddress);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                client = new Client();
                                client.ID = Convert.ToInt32(reader["ID"]);
                                client.IPAddress = reader["IPAddress"].ToString();
                                client.Port = Convert.ToInt32(reader["Port"]);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return client;
        }

        // Insert a new job into the Jobs table
        public static bool InsertJob(Job job)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO Jobs (ClientID, JobData, IsCompleted)
                        VALUES (@ClientID, @JobData, @IsCompleted)";
                        command.Parameters.AddWithValue("@ClientID", job.ClientId);
                        command.Parameters.AddWithValue("@JobData", job.JobData);
                        command.Parameters.AddWithValue("@IsCompleted", job.IsCompleted ? 1 : 0);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // Get all completed jobs
        public static List<Job> GetAllCompletedJobs()
        {
            List<Job> jobs = new List<Job>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Jobs WHERE IsCompleted = 1";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Job job = new Job();
                                job.ID = Convert.ToInt32(reader["ID"]);
                                job.ClientId = Convert.ToInt32(reader["ClientID"]);
                                job.JobData = reader["JobData"].ToString();
                                job.IsCompleted = true;

                                jobs.Add(job);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return jobs;
        }

        // Get job by client ID
        public static List<Job> GetJobsForClient(int clientId)
        {
            List<Job> jobs = new List<Job>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Jobs WHERE ClientID = @ClientID";
                        command.Parameters.AddWithValue("@ClientID", clientId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Job job = new Job();
                                job.ID = Convert.ToInt32(reader["ID"]);
                                job.ClientId = Convert.ToInt32(reader["ClientID"]);
                                job.JobData = reader["JobData"].ToString();
                                job.IsCompleted = Convert.ToBoolean(reader["IsCompleted"]);

                                jobs.Add(job);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return jobs;
        }


        // Update job status
        public static bool UpdateJobStatus(int jobId, bool isCompleted)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE Jobs SET IsCompleted = @IsCompleted WHERE ID = @JobID";
                        command.Parameters.AddWithValue("@IsCompleted", isCompleted ? 1 : 0);
                        command.Parameters.AddWithValue("@JobID", jobId);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();

                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // Get job count for a client by ClientID
        public static int GetJobCountForClient(int clientId)
        {
            int jobCount = 0;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT COUNT(*) FROM Jobs WHERE ClientID = @ClientID";
                        command.Parameters.AddWithValue("@ClientID", clientId);

                        jobCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return jobCount;
        }


    }
}

