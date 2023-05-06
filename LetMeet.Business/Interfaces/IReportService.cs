using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Reports;
using LetMeet.Data.Entites.Identity;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Interfaces;

public interface IReportService
{
    Task<List<TopStudentAbsence>> GetTopStudentsAbsence(UserRole currentUserRole);
    Task<List<TopSupervisorAbsence>> GetTopSupervisorsAbsence(UserRole currentUserRole);
    Task<List<FullSupervisor>> GetFullSupervisors(UserRole currentUserRole);

    Task<List<IdelSupervisor>> GetIdleSupervisors(UserRole currentUserRole);

    Task<OneOf<StudentReport, List<ValidationResult>, List<ServiceMassage>>> GetStudentReport(Guid currentUserId, UserRole currentUserRole, Guid studentId);
    Task<OneOf<SupervisorReport, List<ValidationResult>, List<ServiceMassage>>> GetSupervisorReport(Guid currentUserId, UserRole currentUserRole, Guid supervisorId);

}
