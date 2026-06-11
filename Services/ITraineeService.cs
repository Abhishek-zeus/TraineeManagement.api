using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;

/*
TODO:
1) Store interface in seperate folder

*/
namespace TraineeManagement.myapp.Services
{
    public interface ITraineeService
    {
        public Task<PagedResponse> GetAll(int? pageNumber, int? pageSize);
        public Task<Trainee?> GetById(int id);
        public Task<Trainee> Create(Trainee trainee);
        public Task<Trainee> Update(int id, Trainee trainee);
        public Task<Trainee> Delete(int id);
        public Task<PagedResponse> Search(int? pageNumber, int? pageSize, String? search, String? status);
    }
}