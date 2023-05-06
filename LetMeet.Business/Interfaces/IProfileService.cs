using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Dtos.User;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Interfaces
{
    public interface IProfileService
    {

        Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> AddFreeDay(Guid userId, AddFreeDayDto addFreeDayDto);
        Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> RemoveFreeDay(Guid userId, int freeDayId);
        Task<OneOf<StudentProfileDto, List<ValidationResult>, List<ServiceMassage>>> GetStudentProfile(Guid currentUserId,UserRole currentUserRole,Guid studentId);
        Task<OneOf<SupervisorProfileDto, List<ValidationResult>, List<ServiceMassage>>> GetSupervisorProfile(Guid currentUserId, UserRole currentUserRole,Guid supervisorId);
        Task<List<SupervisorOrStudentSelectDto>> GetAllStudents(Guid currentUserId,UserRole currentUserRole);
        Task<List<SupervisorOrStudentSelectDto>> GetAllSupervisors(Guid currentUserId,UserRole currentUserRole);


    }
}
