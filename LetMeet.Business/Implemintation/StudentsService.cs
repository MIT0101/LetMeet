using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Repositories.Infrastructure;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Implemintation
{
    public class StudentsService : IStudentsService
    {
        private readonly IUserProfileRepository _profileRepo;

        public StudentsService(IUserProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public async Task<OneOf<StudentSelectDto, List<ValidationResult>, IEnumerable<ServiceMassage>>> GetStudentSummary(Guid studentId)
        {
            var repoResult = await _profileRepo.GetSummary(studentId);

            if (!repoResult.Success) {
                return new List<ServiceMassage> { new ServiceMassage("Student Not Found") };
            }
            return repoResult.Result;
        }

    }
}
