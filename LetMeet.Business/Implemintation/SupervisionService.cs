﻿using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories;
using LetMeet.Repositories.Infrastructure;
using Microsoft.Extensions.Options;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Implemintation
{
    public class SupervisionService : ISupervisionService
    {
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly ISupervisonRepository _supervionsRepo;
        private readonly AppServiceOptions _options;

        public SupervisionService(IUserProfileRepository userProfileRepo, ISupervisonRepository supervionsRepo, IOptions<AppServiceOptions> serviceOptions)
        {
            this._options = serviceOptions.Value;
            _supervionsRepo = supervionsRepo;
            _userProfileRepo = userProfileRepo;
        }

        public async Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> AddStudentToSupervisor(Guid supervisorId, Guid studentId, DateTime startDate, DateTime endDate)
        {
            //UserInfo? student = (await _userProfileRepo.GetUserByIdAsync(studentId)).Result;
            //UserInfo? supervisor = (await _userProfileRepo.GetUserByIdAsync(supervisorId)).Result;

            UserInfo? student = (await _userProfileRepo.GetUserByIdAsync(studentId)).Result;
            UserInfo? supervisor = (await _userProfileRepo.GetUserByIdAsync(supervisorId)).Result;

            if (student is null || supervisor is null)
            {
                return new List<ValidationResult>() { new ValidationResult("Can Not Find Student Or Supervisor") };
            }
            int CurrentSupervisorStudents = (await _supervionsRepo.GetCurrentSupervisorStudents(supervisor)).value;
            if (CurrentSupervisorStudents==-1) {
                return new List<ServiceMassage>() { new ServiceMassage($"Can not Supervisor Student") };

            }
            if (CurrentSupervisorStudents >= _options.MaxStudentsPerSupervisor)
            {
                return new List<ServiceMassage>() { new ServiceMassage($"Supervisor {supervisor.fullName} Has Reached Max Student Number") };
            }

            var reposResult = await _supervionsRepo.GetSupervisionAsync(supervisor, student);
            if (reposResult.State == ResultState.NotFound)
            {
                //create superviion
                SupervisionInfo supervion = new SupervisionInfo()
                {
                    supervisor = supervisor,
                    student = student,
                    startDate = startDate,
                    endDate = endDate
                };
                var createResult = await _supervionsRepo.CreateAsync(supervion);

                if (!createResult.Success)
                {
                    return new List<ServiceMassage>() { new ServiceMassage($"Can Not Add {student.fullName} to {supervisor.fullName} Supervisor") };
                }
                return supervion;
            }

            if (reposResult.State == ResultState.Seccess)
            {
                //return the found one
                return reposResult.Result;
            }

            return new List<ServiceMassage>() { new ServiceMassage($"Can Not Add {student.fullName} to {supervisor.fullName} Supervisor") };

        }

        public async Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> ExtendStudentSupervisionExpire(Guid studentId)
        {
            var supervision = (await _supervionsRepo.GetSupervisionAsync(studentId)).Result;
            if (supervision == null) {
                return new List<ValidationResult>() { new ValidationResult("Can Not Find Supervision Info") };
            }
            if (supervision.extendTimes>=_options.MaxExtendTimes) {
                //if student has reached maximum number of extend times init 2 times
                return new List<ServiceMassage>() { new ServiceMassage($"Can Not Extend Supervision Because Student has Reached Max Number Of Extending {supervision.extendTimes} Times") };
            }

            supervision.endDate= supervision.endDate.AddMonths(_options.NumberOfMonthsPerExtend);
            supervision.extendTimes++;

            var updateResult = await _supervionsRepo.UpdateAsync(supervision);

            if(!updateResult.Success)
            {
                return new List<ServiceMassage>() { new ServiceMassage($"Can Not Extend Supervision With {_options.NumberOfMonthsPerExtend} Months") };
            }
            return supervision;
        }

        public async Task<List<SupervisorOrStudentSelectDto>> GetAllAvailableSupervisorsAsync()
        {
            return (await _supervionsRepo.GetAvailableSupervisorNamesAsync(_options.MaxStudentsPerSupervisor)).Result;
        }

        public async Task<IEnumerable<StudentDatedSelectDto>> GetAllSupervisorStudents(Guid supervisorId)
        {
            return (await _supervionsRepo.GetSupervisorStudents(supervisorId)).Result ?? new List<StudentDatedSelectDto>();
        }

        public async Task<SupervisorOrStudentSelectDto?> GetSupervisorOrStudent(Guid userInfoId)
        {
            var supervisor= (await _userProfileRepo.GetUserByIdAsync(userInfoId)).Result;
            if (supervisor is null) {
                return null;
            }
            return new SupervisorOrStudentSelectDto(supervisor.id,supervisor.fullName);
        }

        public async Task<IEnumerable<StudentSelectDto>> GetUnSupervisedStudents()
        {
            return (await _supervionsRepo.GetUnSupervisedStudents()).Result ?? new List<StudentSelectDto>();
        }

        public async Task<OneOf<Dictionary<int, DayHours>, List<ValidationResult>, List<ServiceMassage>>> GetMutualFreeDay(Guid supervisorId, Guid studentId, DateTime date)
        {
            var supervisorFreeDays = (await _userProfileRepo.GetFreeDaysAsync(supervisorId)).Result;
            var studentFreeDays = (await _userProfileRepo.GetFreeDaysAsync(studentId)).Result;

            if (supervisorFreeDays is null || studentFreeDays is null)
            {
                return new List<ServiceMassage> { new ServiceMassage("Can Not Find Supervisor Or Student") };
            }
            //get supervisor and student mutual free days use DayHour
            Dictionary<int, DayHours> mutualDays = DayHours.GetMutualDays(supervisorFreeDays, studentFreeDays);

            return mutualDays;
        }

        public async Task<OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>>> RemoveStudentFromSupervisor(Guid supervisorId, Guid studentId)
        {

            UserInfo? student = (await _userProfileRepo.GetUserByIdAsync(studentId)).Result;
            UserInfo? supervisor = (await _userProfileRepo.GetUserByIdAsync(supervisorId)).Result;

            if (student is null || supervisor is null)
            {
                return new List<ValidationResult>() { new ValidationResult("Can Not Find Student Or Supervisor") };
            }

            var reposResult = await _supervionsRepo.GetSupervisionAsync(supervisor, student);
            if (reposResult.State == ResultState.NotFound)
            {
               return new List<ServiceMassage>() { new ServiceMassage($"Can Not Found Supervision of {student.fullName} with {supervisor.fullName} Supervisor") };
            }

            if (reposResult.State == ResultState.Seccess)
            {
                //remove subervsion
                var removeResult =await _supervionsRepo.RemoveAsync(reposResult.Result);
                if (!removeResult.Success) {
                    return new List<ServiceMassage>() { new ServiceMassage($"Can Not Remove Supervision of {student.fullName} with {supervisor.fullName} Supervisor") };

                }
                // must return the removed one
                return removeResult.Result;
            }
            return new List<ServiceMassage>() { new ServiceMassage($"Can Not Remove Supervision of {student.fullName} with {supervisor.fullName} Supervisor") };
        }

        public async Task<SupervisorOrStudentSelectDto> GetStudentSupervisor(Guid studentId)
        {
            var foundSupervisor=(await _supervionsRepo.GetStudentSupervisor(studentId)).Result;
            if (foundSupervisor is null)
            {
                return null;
            }
            return foundSupervisor;
        }

        public async Task<List<SupervsionSummary>> GetSupervisionsSummary(UserRole userRole)
        {
            if (userRole != UserRole.Admin) {
                return new List<SupervsionSummary>();
            }
            var supervions=(await _supervionsRepo.GetSupervisionsSummary()).Result;
            if (supervions is null || supervions.Count <1) {
                return new List<SupervsionSummary>();
            }
            return supervions;
        }
    }
}
