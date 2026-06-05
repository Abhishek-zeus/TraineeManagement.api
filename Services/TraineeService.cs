using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.Services
{
    public class TraineeService : ITraineeService
    {
        private static List<Trainee> trainees = new();
        private static int nextId = 1;

        public List<Trainee> GetAll()
        {
            return trainees;
        }

        public Trainee GetById(int id)
        {
            return trainees.FirstOrDefault(t => t.id == id);
        }

        public Trainee Create(Trainee trainee)
        {
            trainee.id = nextId++;
            trainee.CreatedDate = DateTime.Now;
            trainee.UpdatedDate = DateTime.Now;

            trainees.Add(trainee);
            return trainee;
        }


        public Trainee Update(int id, Trainee trainee)
        {
            var existing = trainees.FirstOrDefault(t => t.id == id);
            if(existing == null){
                return null;
            }
            existing.FirstName = trainee.FirstName;
            existing.LastName = trainee.LastName;
            existing.Email = trainee.Email;
            existing.TechStack = trainee.TechStack;
            existing.UpdatedDate = DateTime.Now;

            return existing;
        }


        public Trainee Delete(int id)
        {
            var trainee = trainees.FirstOrDefault(t => t.id == id);
            if(trainee == null){
                return null;
            }
            trainees.Remove(trainee);
            return trainee;
        }
    }
}