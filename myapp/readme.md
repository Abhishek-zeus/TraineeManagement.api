# TraineeManagement.api

## Technology Used
I have used C# language and .NET platform for building this ASP.NET Core Web API. For data storage, I have used a MySQL database to store the information of Trainees, Mentors, Tasks, Reviews and Submissions during program execution. 

## EF Core migration commands
dotnet ef migrations add AddTableName
dotnet ef database update

## Login credentials for testing
- Register (Sign Up) as a new User
- Login using Username and password

## JWT usage instructions
- When logged in, a JWT token will be provided which will be valid for 60 Mins
- Use the Token to get Authorized 

## How to Run
1. Run the Program using 'dotnet run'.
2. The Swagger UI will be displayed with Trainee (GET,POST,PUT, DELETE) APIs and Schemas.
3. Tap on the required option.
4. Follow the prompts to perform the respective data operations.

## Database Setup Steps
 
- First import required packages and make sure all the packages are of the same version so that we do not get any version mismatch error.
- Update the Program.cs file for using the MySql database instead of In-memory datase.
- In appsettings.json add another entry for Connection string like this:
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=trainee_management_db;user=root;password=root;"
  },
- Run dotnet build to make sure there are no errors.
- Run the migration command => dotnet ef migrations add InitialCreate
- Once the migration is completed run this command to make tables in the database => dotnet ef database update
- Once ran successfully, the code and the database are in sync. We can test the connection by using swagger UI, try adding one entry using POST end point and see if it is shown in the datase or not.
 
## Backend Setup Steps

- Create Controllers, Service and AppDbContext Layer
- Connect to Database
- Configure JWT Credentials
- Add all configurations in Program.cs
- Save and execute 'dotnet run' 

## API List

- GET    /api/health
 
- POST   /api/auth/login
 
- GET    /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active
- GET    /api/trainees/{id}
- POST   /api/trainees
- PUT    /api/trainees/{id}
- DELETE /api/trainees/{id}
 
- GET    /api/mentors
- GET    /api/mentors/{id}
- POST   /api/mentors
- PUT    /api/mentors/{id}
- DELETE /api/mentors/{id}
 
- GET    /api/learning-tasks
- GET    /api/learning-tasks/{id}
- POST   /api/learning-tasks
- PUT    /api/learning-tasks/{id}
- DELETE /api/learning-tasks/{id}
 
- POST   /api/task-assignments
- GET    /api/task-assignments
- GET    /api/task-assignments/{id}
- PUT    /api/task-assignments/{id}/status
 
- POST   /api/submissions
- GET    /api/submissions
- GET    /api/submissions/{id}
 
- POST   /api/reviews
- GET    /api/reviews
- GET    /api/reviews/{id}
 
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
 
 
## Known Limitations
- Scalability
- Currently there is no Frontend
- Better architecture or schema design
 
## Security Checklist
- Authentication -> JWT validation enabled
- Authorization -> Protected APIs require token
- Password storage -> Passwords stored as hash only
- Excessive data exposure -> DTOs used; password hash not returned
- Injection -> EF Core used; no unsafe raw SQL
- Security misconfiguration -> CORS restricted to expected origin
- Sensitive data exposure -> Secrets not hardcoded in controllers
- Error handling -> Stack traces not returned
- Logging -> Passwords and tokens not logged
 
## Next Improvement areas
- Improve the scalability of the api's
- Adding Frontend