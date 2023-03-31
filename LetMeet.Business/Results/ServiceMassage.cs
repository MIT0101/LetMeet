using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Results
{
    public struct ServiceMassage
    {
        public string Message { get; init; }

        public ServiceMassage(string message)
        {
            Message = message;
        }
    }
}
