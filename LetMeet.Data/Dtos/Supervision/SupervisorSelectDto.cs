using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Supervision
{
    public record SupervisorSelectDto(Guid id,string FullName);
    public record StudentSelectDto(Guid id, string FullName);

    public record StudentDatedSelectDto(Guid id,string FullName,DateTime endDate);


}
