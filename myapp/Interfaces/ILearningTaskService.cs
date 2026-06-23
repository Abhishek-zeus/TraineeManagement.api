using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.LearningTask_DTO;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Interfaces
{
    public interface ILearningTaskService
    {
        public Task<List<CreateTaskRequest>> GetLearningTasks();
        public Task<CreateTaskRequest> GetById(int id);
        public Task<CreateTaskRequest> RegisterTask(CreateTaskRequest request);
        public Task<CreateTaskRequest> UpdateTask(int id, CreateTaskRequest request);
        public Task<CreateTaskRequest> DeleteTask(int id);


    }
}