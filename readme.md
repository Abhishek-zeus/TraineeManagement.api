# TraineeManagement.api

## Technology Used
   - Backend Framework: C# 12 / .NET 10 (ASP.NET Core Web API & Background Workers)
   - Database Management: MySQL Server
   - Message Broker: RabbitMQ (For asynchronous microservice communication)
   - Caching Layer: Redis Cache (For high-performance data retrieval)
   - Containerisation: Docker & Docker Compose (For local environment orchestration)
   - Core Data Models: Trainees, Mentors, Tasks, Reviews, Processing Jobs, Submissions and Submission Files.

## Architecture Diagram
```mermaid
flowchart TB
    Client["Client (Postman / Swagger / Frontend)"]

    subgraph Docker ["Docker Compose Network"]
        API["TraineeManagement.Api (JWT, Upload/Download, Jobs, Cache, Publisher)"]
        Worker["SubmissionProcessor.Worker (Consumer, Idempotency, Retry, DLX, Directory client)"]
        Directory["TrainingDirectory.Api (Read only trainee lookup)"]
        MySQL[("MySQL (Source of truth)")]
        Redis[("Redis (Cache-aside)")]
        RabbitMQ{{"RabbitMQ (submission-processing queue)"}}
        DLX{{"Dead Letter Exchange (submission-processing-failed)"}}
        Storage[["Shared uploads volume"]]
    end

    Client -->|JWT Bearer| API
    API -->|EF Core "(Cache MISS)"| MySQL
    API -->|GET data From Cache| Redis
    Redis -->|Cache HIT| API
    MySQL -->|Update Cache after MISS| Redis 
    API -->|Publish SubmissionProcessingRequested| RabbitMQ
    API <-->|Save/serve files| Storage
    RabbitMQ -->|Consume, manual ack| Worker
    RabbitMQ -.->|Exhausted retries / permanent failure| DLX
    Worker -->|EF Core read/write| MySQL
    Worker <-->|Read file for checksum| Storage
    Worker -->|HTTP GET, resilience pipeline| Directory
    API -.->|Correlation ID propagated| Worker
    Worker -.->|Correlation ID propagated| Directory

```
## How to Run
1. Run docker compose build && docker compose up -d
2. Then one time dotnet ef database update
3. Run the Program using 'dotnet run'.
4. The Swagger UI will be displayed with Trainee (GET,POST,PUT, DELETE) APIs and Schemas.
5. Tap on the required option.
6. Follow the prompts to perform the respective data operations.

## Configuration (`appsettings.json`)
Below is the standard configuration schema template. 

Security Note: Sensitive fields (passwords, secret keys) use empty string placeholders (`""`) in source control. Real credentials are securely injected at runtime using Docker Environment Variables (`ConnectionStrings__DefaultConnection`, `JwtSettings__SecretKey`, etc.).

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=mysql_server;Port=3307;Database=trainee_management_db;Uid=root;Pwd="
  },
  "JwtSettings": {
    "Issuer": "https://identity.local",
    "Audience": "https://api.local",
    "SecretKey": ""
  },
  "FileStorage": {
    "UploadDirectory": "/app/uploads",
    "MaxFileSizeInBytes": 10485760 
  },
  "RabbitMQ": {
    "HostName": "rabbitmq_broker",
    "Port": 5672,
    "UserName": "guest",
    "Password": "",
    "SubmissionQueue": "trainee.submissions.queue"
  },
  "Redis": {
    "ConnectionString": "redis_cache:6379,password="
  }
}
```
 - ConnectionStrings: Links the app to the MySQL database. Uses the Docker container name (`mysql_server`) as the host.
 - JwtSettings: Secures authentication. The `SecretKey` signs and validates trainee login tokens to prevent tampering.
 - FileStorage: Configures file uploads. Points to `/app/uploads`, which maps to a shared Docker volume used by both the API and Worker.
 - RabbitMQ: Network settings for the message broker. Tells the API where to publish messages and the Worker where to consume them.
 - Redis: Configures the in-memory cache to store high-frequency data and reduce direct MySQL database load.

## API List
- GET /api/health: Returns application health status
- GET /api/trainees: Returns all trainees
- GET /api/trainees?search=amit: Searches trainees by first name, last name, email and tech stack
- GET /api/trainees/{id}: Get trainee by Id
- POST /api/trainees: Create a trainee
- PUT /api/trainees/{id}: Update a trainee
- DELETE /api/trainees/{id}: Delete a trainee
- POST /api/auth/login: Used for Logging in a user

- POST /api/auth/login: Returns JWT Token along with login minutes on correct credentials.

- GET /api/mentors : Returns list of mentors. Also contains search param to filter by required mentors.

- GET /api/mentors/{id} : Get specific mentor by id
- POST /api/mentors : Used for creating new mentor
- PUT /api/mentors/{id} : Update mentor contents of a particular ID.
- DELETE /api/mentors/{id} : Update mentor contents of a particular ID.

- GET /api/learning-tasks : Returns list of Learning Tasks. Also contains search param to filter by required Tasks.

- GET /api/learning-tasks/{id} : Get specific Tasks by id
- POST /api/learning-tasks : Used for creating new Tasks
- PUT /api/learning-tasks/{id} : Update Tasks contents of a particular ID.
- DELETE /api/learning-tasks/{id} : Delete a Tasks by specified ID

- GET /api/task-assignments : Gets all the Task assignments from the database

- GET /api/task-assignments/{id} : Gets a particular Task assignment with the id parameter
- POST /api/task-assignments : Adds a new Task assignment to the database
- PUT /api/task-assignments/{id}/status : Changes status of a particular Task assignment using id parameter

- POST /api/submissions : Adds a new Submission to the database

- GET /api/submissions : Gets all the Submissions from the database
- GET /api/submissions/{id} : Gets a particular Submission with the id parameter

- POST /api/submissions/{submissionId}/files : Upload a submission file to the local storage

- GET /api/submission-files/{id}/download : Download the submission file
- DELETE /api/submission-files/{id} : Delete a submission file

- POST /api/submissions/{submissionId}/files

- GET /api/submission-files/{id}/download
- DELETE /api/submission-files/{id}

- GET /api/training-directory/assignments/{assignmentId}/processing-profile

## Sample Request JSON
 
Sample POST /login request:
{
  "username": "Admin",
  "password": "admin@12345"
}
 
Sample POST /api/trainees request:
{
  "firstName": "Piyush",
  "lastName": "Pawar",
  "email": "piyushpawar@example.com",
  "techStack": ".net",
  "status": "Active"
}
 
Sample PUT /api/trainees/{1} request:
{  
  "status": "InActive"
}
 
Sample POST /api/mentor request:
{
  "firstName": "Om",
  "lastName": "Deshmukh",
  "email": "omdeshmukh@example.com",
  "expertise": "Java Spring Boot",
  "status": "Active"
}
 
Sample PUT /api/mentor/{1} request:
{
  "status": "Inactive"
}
 
Sample POST /api/learning-tasks request:
{
  "title": "Trainee Management API",
  "description": "Building a backend for trainee management",
  "expectedTechStack": ".net, asp.net core, ef core",
  "dueDate": "2026-06-15T10:03:59.092Z",
  "status": "Draft"
}
 
Sample PUT /api/learning-tasks request:
{
  "status": "Published"
}
 
Sample POST /api/task-assignment request:
{
  "traineeId": 1,
  "mentorId": 1,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T10:08:57.407Z",
  "dueDate": "2026-06-15T10:08:57.407Z",
  "status": "Assigned",
  "remarks": ""
}
 
Sample PUT /api/task-assignment/{1}/status request:
{
  "status": "InProgress"
}
 
Sample POST /api/submissions request:
{
  "taskAssignmentId": 1,
  "submissionUrl": "https://localhost:9000",
  "notes": "Nothing",
  "submittedDate": "2026-06-15T10:14:07.178Z",
  "status": "Submitted"
}
 
Sample POST /api/reviews request:
{
  "submissionId": 1,
  "mentorId": 1,
  "feedback": "Good implementation",
  "score": 9/10,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T10:16:43.838Z"
}
 
## Sample Response JSON
 
Sample POST /login reponse:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWJoaXNoZWsiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJTREUiLCJleHAiOjE3ODE2MTg5OTAsImlzcyI6IlRyYWluZWVNYW5hZ2VtZW50QXBpIiwiYXVkIjoiVHJhaW5lZU1hbmFnZW1lbnRDbGllbnQifQ.cgcxFLSn4SILpkOpB6EUfGyH2M_luPkDuF8WYFLqlIU",
  "expiresIn": 3600,
  "user": {
    "id": 2,
    "username": "Abhishek",
    "role": "SDE"
  }
}
 
Sample GET /api/health response:
 
{
  "status": "running",
  "application": "Trainee Management API",
  "timestamp": "2026-06-08T11:09:44.9139355+00:00"
}
 
Sample GET /api/Trainee?pageNumber=1&pageSize=10&search=abhi response:

{
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 1,
  "data": [
    {
      "firstName": "Abhishek",
      "lastName": "Revankar",
      "email": "abhi@example.com",
      "techStack": "Java",
      "status": "Active"
    }
  ]
}
 
Sample GET /api/trainees/{id} response:
 
{
  "id": 1,
  "firstName": "Abhishek",
  "lastName": "Revankar",
  "email": "abhi@example.com",
  "techStack": "Java",
  "status": "Active",
  "createdDate": "2026-06-10T08:54:52.283302Z",
  "updatedDate": "2026-06-10T08:54:52.351906Z"
}
 
 
Sample GET /api/mentor response:
[
  {
    "firstName": "Pranav",
    "lastName": "Baraiyya",
    "email": "pbr@example.com",
    "expertise": ".NET",
    "status": "Active"
  }
]
 
Sample GET /api/mentor/{1} response:
{
  "firstName": "Pranav",
  "lastName": "Baraiyya",
  "email": "pbr@example.com",
  "expertise": ".NET",
  "status": "Active"
}
 
Sample POST /api/mentor response:
{
  "firstName": "Narayan",
  "lastName": "Sarodi",
  "email": "narayan@example.com",
  "expertise": "Teaching",
  "status": "Active"
}
 
Sample PUT /api/mentor/{1} response:
{
  "firstName": "Narayan",
  "lastName": "Sarodi",
  "email": "narayan@example.com",
  "expertise": "Teaching",
  "status": "Active"
}

Sample GET /api/learning-tasks response:
[
  {
    "title": "Learn .NET",
    "description": "Refer Documentation",
    "expectedTechStack": ".NET",
    "dueDate": "2026-06-16T08:22:41.637Z",
    "status": "Draft"
  },
  {
    "title": "Test Dev Tools",
    "description": "Refer Documentation",
    "expectedTechStack": "Dev Tools",
    "dueDate": "2026-06-16T08:22:41.637Z",
    "status": "Published"
  }
]
 
Sample GET /api/learning-tasks/{1} response:
{
  "title": "Learn .NET",
  "description": "Refer Documentation",
  "expectedTechStack": ".NET",
  "dueDate": "2026-06-16T08:22:41.637Z",
  "status": "Draft"
}
 
Sample POST /api/learning-tasks response:
{
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-16T13:25:20.134Z",
  "status": 0
}

 
Sample PUT /api/learning-tasks/{1} response:
{
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-16T13:25:20.134Z",
  "status": 0
}

 
Sample GET /api/task-assignment response:
[
  {
    "traineeId": 1,
    "mentorId": 1,
    "learningTaskId": 2,
    "dueDate": "2026-06-16T08:33:42.151Z",
    "remarks": "Assigned"
  }
]
 
Sample GET /api/task-assignment/{1} response:
{
  "traineeId": 1,
  "mentorId": 1,
  "learningTaskId": 2,
  "dueDate": "2026-06-16T08:33:42.151Z",
  "remarks": "Assigned"
}
 
Sample GET /api/submissions response:
[
  {
    "taskAssignmentId": 1,
    "submissionUrl": "Github.com",
    "notes": "I have completed day1 task",
    "status": 0
  }
]
 
 
Sample GET /api/submissions/{1} response:
{
  "taskAssignmentId": 1,
  "submissionUrl": "Github.com",
  "notes": "I have completed day1 task",
  "status": 0
}


Sample GET /api/reviews response:
[
  {
    "submissionId": 1,
    "mentorId": 1,
    "feedback": "Code seems to be good, but still some changes required",
    "score": 90,
    "status": 1
  }
]
 
Sample GET /api/reviews/{1} response:
{
  "submissionId": 1,
  "mentorId": 1,
  "feedback": "Code seems to be good, but still some changes required",
  "score": 90,
  "status": 1
}

## Login credentials for testing
- Register (Sign Up) as a new User
- Login using Username and password

## Design Decisions
# Caching Strategy (Redis)
- Cache Keys (`trainee:{id}`, `submission-summary:{id}`): Predictable, greppable, and uniform convention for all entities.
- Cache TTL (Trainee: 120s | Submission: 60s): Trainee data changes rarely; submission status changes rapidly during active processing.
- Cache Invalidation (`RemoveAsync` on write): Simpler and safer than overwriting; the next read naturally falls back to MySQL and repopulates.
- Cache Failures (Try/Catch around Redis): Treated as a cache MISS. Caching is a performance optimization, not a hard system dependency.

# Messaging Reliability (RabbitMQ)
- Queue Durability (Durable + Persistent): Ensures both queues and messages survive a Docker or broker restart.
- Acknowledgement Model (Manual ACK + `prefetchCount: 1`): Prioritises reliability over throughput; never loses work or overloads a worker instance.
- Retry Limit (`MaxAttempts: 3`): Uses exponential backoff to absorb transient blips without flooding logs on hopeless errors.
- Dead-Lettering (`x-dead-letter-exchange`): Routes failed messages to a separate queue to provide a physical, inspectable record of dead items.

# Resilience & System Integrity
- Idempotency Guard: Checks if `ProcessingJob.Status == "Completed"` before executing work to prevent duplicate side-effects.
- Message Contract (Plain DTO with `ContractVersion`): Decoupled from EF Core entities so database schemas can evolve without breaking the Worker.
- HTTP Resilience (2s timeout, 3 retries, Circuit Breaker): Fails fast on broken dependencies instead of hanging or retrying forever.
- Lookup Failures (Log and Continue): Directory lookup is treated as supplementary, not authoritative for processing success.

# Architecture & Security
- DI Scoping (Fresh `IServiceScopeFactory` per message): Prevents a long-lived Singleton Worker from leaking tracked `DbContext` instances.
- File Naming (Server-generated UUID): Discards client-supplied filenames to close path traversal security vulnerabilities entirely.

## Known Limitations
- Scalability
- Currently there is no Frontend
- Better architecture or schema design (Redundant code in Myapp and Worker)
- More Advanced Authentication and Authorization is required

## Challenges Faced
Some of the technical/logical challenges faced were: - Dotnet installation of packages (particularly Swagger) due to proxy server difficulties

## Next Improvement Areas
Integration with frontend