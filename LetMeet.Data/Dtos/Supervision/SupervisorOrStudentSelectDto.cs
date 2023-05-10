using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Supervision
{
    public record SupervisorOrStudentSelectDto(Guid id,string FullName);
    public record StudentSelectDto(Guid studentId, string FullName);

    public record StudentDatedSelectDto(Guid studentId,string FullName,DateTime startDate,DateTime endDate) ;

    public record SupervsionSummary(int supervsionId,Guid supervisorId,Guid studentId ,string supervisorName,string studentName,DateTime endDate);


}
