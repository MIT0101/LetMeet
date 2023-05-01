using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.Identity;

namespace LetMeet.Business.Interfaces;

public interface ISupervisionService
{
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> AddStudentToSupervisor(Guid supervisorId, Guid studentId,
            DateTime startDate, DateTime endDate);
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> RemoveStudentFromSupervisor(Guid supervisorId, Guid studentId);
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> ExtendStudentSupervisionExpire(Guid studentId);

    Task<List<SupervisorOrStudentSelectDto>> GetAllAvailableSupervisorsAsync();

    Task<SupervisorOrStudentSelectDto> GetSupervisorOrStudent(Guid userInfoId);
    Task<SupervisorOrStudentSelectDto> GetStudentSupervisor(Guid studentId);
    Task<IEnumerable<StudentDatedSelectDto>> GetAllSupervisorStudents(Guid supervisorId);

    Task<IEnumerable<StudentSelectDto>> GetUnSupervisedStudents();
    Task<OneOf<Dictionary<int, DayHours>, List<ValidationResult>,List<ServiceMassage>>> GetMutualFreeDay(Guid supervisorId,Guid studentId,DateTime date);

    Task<List<SupervsionSummary>> GetSupervisionsSummary(UserRole userRole);
}

