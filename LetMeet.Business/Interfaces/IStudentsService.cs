using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Interfaces
{
    public interface IStudentsService
    {
        Task<OneOf<StudentSelectDto, List<ValidationResult>, IEnumerable<ServiceMassage>>> GetStudentSummary(Guid studentId);
    }
}
