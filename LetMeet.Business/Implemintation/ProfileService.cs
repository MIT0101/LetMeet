using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.User;
using LetMeet.Data.Entites.Identity;
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
        private readonly ISupervisionService _supervisionService;

        public ProfileService(IUserProfileRepository profileRepository, ISupervisionService supervisionService)
        {
            _profileRepository = profileRepository;
            _supervisionService = supervisionService;
        }

        public async Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> AddFreeDay(Guid userId, AddFreeDayDto addFreeDayDto)
        {
            var validationResult = RepositoryValidationResult.DataAnnotationsValidation(addFreeDayDto);
            if (!validationResult.IsValid)
            {
                return validationResult.ValidationErrors;
            }
            if (addFreeDayDto.startHour > addFreeDayDto.endHour)
            {
                List<ValidationResult> hoursValidation = new List<ValidationResult>();
                hoursValidation.Add(new ValidationResult("Start Hour must be lower than End Hour", new string[] { "startHour", "endHour" }));
                return hoursValidation;
            }
            var reposResult = await _profileRepository.AddFreeDay(userId, addFreeDayDto);

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

        public async Task<OneOf<StudentProfileDto, List<ValidationResult>, List<ServiceMassage>>> GetStudentProfile(Guid currentUserId, UserRole currentUserRole, Guid studentId)
        {
            //check if studentId is null or empty
            if (studentId == null || studentId == Guid.Empty)
            {
                var validationErrors = new List<ValidationResult>() { new ValidationResult("Invalid data", new[] { "studentId" }) };
                return validationErrors;
            }
            //if the current user is student and he want to get his profile or if the current user is supervisor and he want to get student profile
            if (currentUserId != studentId && currentUserRole != UserRole.Supervisor && currentUserRole != UserRole.Admin)
            {
                return new List<ServiceMassage>() { new ServiceMassage("You don't have permission to see this profile") };
            }
            //If the current user is supervisor check if he is supervisor of the student
            if (currentUserRole == UserRole.Supervisor)
            {
               //get supervisor students and check if the student is one of them
                var studentSupervisor = await _supervisionService.GetStudentSupervisor(currentUserId);
                if (studentSupervisor is not null && studentSupervisor.id != currentUserId ) {
                    return new List<ServiceMassage>() { new ServiceMassage("You don't have permission to see this profile") };
                }
            }

            //get student profile from repository
            var reposResult = await _profileRepository.GetStudentProfile(studentId);

            //check if repository result is valid
            if (reposResult.State != ResultState.Seccess)
            {
                return new List<ServiceMassage>() { new ServiceMassage("Can't Get Student Profile") };

            }
            return reposResult.Result;

        }

        public async Task<OneOf<SupervisorProfileDto, List<ValidationResult>, List<ServiceMassage>>> GetSupervisorProfile(Guid currentUserId, UserRole currentUserRole, Guid supervisorId)
        {
            //check if studentId is null or empty
            if (supervisorId == null || supervisorId == Guid.Empty)
            {
                var validationErrors = new List<ValidationResult>() { new ValidationResult("Invalid data", new[] { "supervisorId" }) };
                return validationErrors;
            }
            //check if the current user is supervisor and he want to get his profile or if the current user is admin and he want to get supervisor profile
            if (currentUserId != supervisorId && currentUserRole != UserRole.Admin )
            {
                return new List<ServiceMassage>() { new ServiceMassage("You don't have permission to see this profile") };
            }
            //get student profile from repository
            var reposResult = await _profileRepository.GetSupervisorProfile(supervisorId);

            //check if repository result is valid
            if (reposResult.State != ResultState.Seccess)
            {
                return new List<ServiceMassage>() { new ServiceMassage("Can't Get Supervisor Profile") };

            }
            return reposResult.Result;
        }

        public async Task<OneOf<DayFree, List<ValidationResult>, List<ServiceMassage>>> RemoveFreeDay(Guid userId, int freeDayId)
        {
            if (userId == Guid.Empty || freeDayId < 0)
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
