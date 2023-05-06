using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Reports;

public class FullSupervisor : BasicUserInfoReport
{
    public int currentActiveStudentsCount { get; set; }
}
