# Peer-to-Peer Distributed Job Execution System

This project implements a hybrid **Peer-to-Peer (P2P)** system for distributed job execution. It consists of:

- A **.NET MVC Web Service** for coordination and tracking
- A **.NET Remoting Desktop Client Application** to submit and process jobs
- An **ASP.NET Core Dashboard Website** for real-time system monitoring

The system distributes Python jobs across connected peer nodes, inspired by systems like Folding@Home and BitTorrent.

---

## System Components

### 1. ASP.NET MVC Web Service (Coordinator)

#### Responsibilities:
- Register and update client information (IP and port)
- Respond to clients with an updated list of active peers
- Receive and store job completion reports

#### Key Endpoints:
- `POST /api/clients/register`: Register or update a client
- `GET /api/clients/list`: Return list of all active clients
- `POST /api/jobs/report`: Submit completed job results

---

### 2. Desktop Client Application (Peer Node)

#### Technologies Used:
- .NET Remoting
- IronPython (via NuGet)
- RestSharp, Newtonsoft.JSON
- Threading for parallel server and network handling

#### Core Threads:
- **GUI Thread**: Displays status, job count, and allows job submission (via textbox or file)
- **Networking Thread**:
  - Retrieves peer list from Web Server
  - Sends jobs to peers and receives results
  - Executes Python jobs using IronPython
  - Sends completed job reports to both job host and server
- **Server Thread**:
  - Hosts .NET Remoting endpoint
  - Accepts job requests and returns results
  - Maintains queue of local pending jobs
  - Thread-safe job access and update

#### Job Lifecycle:
1. User submits a Python job (Base64 encoded)
2. Job is broadcast to available peers
3. Peer executes job using IronPython
4. Peer returns result and logs completion to Web Server

---

### 3. ASP.NET Core Dashboard Website

#### Features:
- Connects to Web Server to fetch client/job data via API
- Displays:
  - Active peer clients
  - Number of jobs completed by each
  - Timestamp of last known activity
- Auto-refresh every 1 minute via JavaScript

---

## Advanced Functionality

### Dead Client Detection
- Web Server checks client reachability periodically
- Clients that fail to respond are removed from the active list

---

## Data Format and Security

### Job Encoding
- All jobs are Base64 encoded for safe transmission

### Hashing and Integrity
- Each job includes a SHA256 hash of the Base64 string
- Peers validate hash before executing the job
- Prevents tampering or execution of corrupted code

---

## IronPython Integration

### Usage
- IronPython is used to execute Python job strings inside the CLR
- Each job is sandboxed and exception-handled

### Example:
```csharp
var engine = Python.CreateEngine();
var scope = engine.CreateScope();
engine.Execute(pythonScript, scope);
