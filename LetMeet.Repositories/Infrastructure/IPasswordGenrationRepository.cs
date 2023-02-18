using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IPasswordGenrationRepository
    {
        public Task<string> GenerateRandomPassword(int length=16);
    }
}
