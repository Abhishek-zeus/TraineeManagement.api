using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.DTOs.Mentors_DTO;

namespace TraineeManagement.myapp.Interfaces
{
    public interface IMentorService
    {
        public Task<List<RegisterMentorRequest>> GetMentors();
        public Task<RegisterMentorRequest> GetById(int id);
        public Task<RegisterMentorRequest> RegisterMentor(RegisterMentorRequest request);
        public Task<RegisterMentorRequest> UpdateMentor(int id, RegisterMentorRequest request);
        public Task<RegisterMentorRequest> DeleteMentor(int id);


    }
}