using LetMeet.Data.Dtos.Supervision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface ISelectionRepository
    {
        public List<string> GetStages();
        public List<string> GetUserRoles();


    }
}
