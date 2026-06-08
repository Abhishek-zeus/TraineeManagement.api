# TraineeManagement.api

## Technology Used
I have used C# language and .NET platform for building this ASP.NET Core Web API. For data storage, I have used an EF Core in-memory database to store the information of Trainees during program execution. 

## Features Completed
- Add task
- View task list
- Mark task as completed
- Delete task

## How to Run
1. Run the Program using 'dotnet run'.
2. The Swagger UI will be displayed with Trainee (GET,POST,PUT, DELETE) APIs and Schemas.
3. Tap on the required option.
4. Follow the prompts to perform the respective data operations.

## API List
1. GET /api/Trainee --> to perform search or extract all Trainees
2. POST /api/Trainee --> to Add details of new Trainee
3. GET /api/Trainee/{id} --> to search Trainee by Id
4. PUT /api/Trainee/{id} --> to Update the details of a existing Trainee
5. DELETE /api/Trainee/{id} --> To Delete a particular Trainee by Id 

## Sample Request JSON
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "techStack": "string",
  "status": "string"
}

## Sample Response JSON
{
  "id": 1,
  "firstName": "Suraj",
  "lastName": "Prajapati",
  "email": "suraj@example.com",
  "techStack": "Python",
  "status": "Active",
  "createdDate": "2026-06-08T11:07:23.6641556+00:00",
  "updatedDate": "2026-06-08T11:07:23.6657546+00:00"
}

## Known Limitations

- It currently uses In Memory database which holds data temporarily. 
- It lacks real Sql/No Sql database connection.
- Absence of authentication and authorisation



