using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Enums;


namespace TraineeManagement.myapp.Interfaces
{
    public interface ITaskAssignmentService
    {
        public Task<CreateTaskAssignmentRequest> AssignTask(CreateTaskAssignmentRequest request);
        public Task<List<CreateTaskAssignmentRequest>> GetAssignments();
        public Task<CreateTaskAssignmentRequest> GetById(int id, CancellationToken cancellationToken);
        public Task<CreateTaskAssignmentRequest> UpdateTask(int Id, AssignmentStatus status, CancellationToken cancellationToken);
        


    }
}