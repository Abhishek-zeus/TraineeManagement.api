using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.DTOs;
namespace TraineeManagement.myapp.Interfaces
{
    public interface ITraineeService
    {
        public Task<PagedResponse<TraineeDTO>> GetTrainees(int? pageNumber, int? pageSize, string? search, string? status);
        public Task<Trainee?> GetById(int id, CancellationToken cancellationToken);
        public Task<Trainee> CreateTrainee(TraineeDTO trainee);
        public Task<Trainee> UpdateTrainee(int id, TraineeDTO trainee, CancellationToken cancellationToken);
        public Task<Trainee> DeleteTrainee(int id, CancellationToken cancellationToken);
        public Task<PagedResponse<TraineeDTO>> Search(int? pageNumber, int? pageSize, String? search, String? status);
    }
}