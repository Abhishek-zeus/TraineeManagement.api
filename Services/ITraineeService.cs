using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Services
{
    public interface ITraineeService
    {
        public Task<List<Trainee>> GetAll();
        public Task<Trainee?> GetById(int id);
        public Task<Trainee> Create(Trainee trainee);
        public Task<Trainee> Update(int id, Trainee trainee);
        public Task<Trainee> Delete(int id);
        public Task<List<Trainee>> Search(String search);
    }
}