using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IEmailRepository
    {
        //return secsess 
        //ffail returns Error
        Task<(ResultState state, bool isSended)> SendEmail(string recipientEmail, string subject, string body);

    }
}
