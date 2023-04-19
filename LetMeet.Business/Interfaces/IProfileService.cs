using LetMeet.Business.Results;
using LetMeet.Data.Dtos.User;
using LetMeet.Data.Entites.UsersInfo;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Interfaces
{
    public interface IProfileService
    {

       Task<OneOf<DayFree , List<ValidationResult>,List<ServiceMassage>>> AddFreeDay(Guid userId,AddFreeDayDto addFreeDayDto);
       Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> RemoveFreeDay(Guid userId, int freeDayId);

    }
}
