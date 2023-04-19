using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.User;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories;
using LetMeet.Repositories.Infrastructure;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Implemintation
{
    public class ProfileService : IProfileService
    {
        private readonly IUserProfileRepository _profileRepository;

        public ProfileService(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> AddFreeDay(Guid userId, AddFreeDayDto addFreeDayDto)
        {
            var validationResult= RepositoryValidationResult.DataAnnotationsValidation(addFreeDayDto);
            if(!validationResult.IsValid) {
                return validationResult.ValidationErrors;
            }
            var reposResult = await _profileRepository.AddFreeDay(userId, addFreeDayDto);

            if (reposResult.State == ResultState.ValidationError) {
                return reposResult.ValidationErrors;

            }
            if (!reposResult.Success) {
                List<ServiceMassage> massages = new List<ServiceMassage>();
                massages.AddRange(reposResult.ErrorMessages.Select(e => new ServiceMassage(e)));
                return massages;
            }

            return reposResult.Result;

        }

        public async Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> RemoveFreeDay(Guid userId, int freeDayId)
        {
            if (userId == Guid.Empty || freeDayId<0)
            {
                var validationErrors = new List<ValidationResult>();
                validationErrors.Add(new ValidationResult("Invalid data", new[] { "userId", "freeDayId" }));
                return validationErrors;
            }
            var reposResult = await _profileRepository.RemoveFreeDay(userId, freeDayId);

            if (reposResult.State == ResultState.ValidationError)
            {
                return reposResult.ValidationErrors;

            }
            if (!reposResult.Success)
            {
                List<ServiceMassage> massages = new List<ServiceMassage>();
                massages.AddRange(reposResult.ErrorMessages.Select(e => new ServiceMassage(e)));
                return massages;
            }

            return reposResult.Result;
        }
    }
}
