using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Services
{
    public interface ITraineeService
    {
        public List<Trainee> GetAll();
        public Trainee? GetById(int id);
        public Trainee Create(Trainee trainee);
        public Trainee Update(int id, Trainee trainee);
        public Trainee Delete(int id);
    }
}