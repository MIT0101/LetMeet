using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Business.Results;

namespace LetMeet.Business.Interfaces;

public interface ISupervisionService
{
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> AddStudentToSupervisor(Guid supervisorId, Guid studentId,
            DateTime startDate,DateTime endDate);
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> RemoveStudentFromSupervisor(Guid supervisorId, Guid studentId);
    Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> ExtendStudentSupervisionExpire(Guid studentId);

}

