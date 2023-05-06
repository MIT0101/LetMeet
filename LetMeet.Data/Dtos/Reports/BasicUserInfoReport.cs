using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Reports;

public class BasicUserInfoReport
{
    public Guid id { get; set; }
    public string fullName { get; set; }
    public string email { get; set; }
}
