using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class SelectionRepository : ISelectionRepository
    {
        private List<string> stages;
        private List<string> userRoles;

        public SelectionRepository()
        {
            stages = Enum.GetNames(typeof(Stage)).Where(x => x != Stage.Unknown.ToString()).ToList();
            userRoles = Enum.GetNames(typeof(UserRole)).ToList();
        }

        public List<string> GetStages()
        {
            return stages;
        }

        public List<string> GetUserRoles()
        {
            return userRoles;
        }
    }
}
