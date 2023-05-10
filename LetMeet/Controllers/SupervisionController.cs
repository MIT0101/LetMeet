using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;

namespace LetMeet.Controllers
{
    [Authorize(Roles ="Admin")]
    public class SupervisionController : Controller
    {
        private readonly ILogger<SupervisionController> _logger;
        private readonly ISupervisionService _supervisionService;


        public SupervisionController(ILogger<SupervisionController> logger, ISupervisionService supervisionService)
        {
            _logger = logger;
            _supervisionService = supervisionService;
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id, List<string> errors, List<string> messages)
        {
            var suprvisors = await _supervisionService.GetSupervisorOrStudent(id);
            if(suprvisors is null)
            {
                return View();
            }

            // show all unsupervised students
            IEnumerable<StudentSelectDto> unsupervisedStudents = await _supervisionService.GetUnSupervisedStudents();
            ViewData[ViewStringHelper.UnSupervisedStudents] = unsupervisedStudents;
            //get current supervised students Dated
            IEnumerable<StudentDatedSelectDto> supervisorStudents = await _supervisionService.GetAllSupervisorStudents(id);
            ViewData[ViewStringHelper.SupervisorStudents] = supervisorStudents;
            ViewData[ViewStringHelper.Errors] = errors ;
            ViewData[ViewStringHelper.Messages] = messages;
            ViewData[ViewStringHelper.Supervisor] = suprvisors;
            //ViewData[ViewStringHelper.Supervisor] = new SupervisorSelectDto(Guid.Parse("f43548ae-981e-4a34-6b06-08db32234eb5"), "My Supervisor2");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudnetToSupervisor(Guid id, AddStudentToSupervisorDto addStudent)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
            }

            OneOf.OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>> result =
                await _supervisionService.AddStudentToSupervisor(supervisorId: id, studentId: addStudent.studentId,
                startDate: addStudent.startDate,
                endDate: addStudent.endDate);

            result.Switch(
                            supervisionInfo =>
                               messages.Add("Student added to supervisor")
                            ,

                            validationResults =>
                               errors.AddRange(validationResults?.Select(e => e.ErrorMessage))
                            ,

                           serviceMassages =>
                              errors.AddRange(serviceMassages.Select(m => m.Message))
                            );

            return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudnetFromSupervisor(Guid id, SupervisionDto supervisionDto)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
            }
            OneOf.OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>> result =
                await _supervisionService.RemoveStudentFromSupervisor(supervisorId: supervisionDto.supervisorId, studentId:supervisionDto.studentId);

            result.Switch(
                            supervisionInfo =>
                               messages.Add("Student Removed From supervisor")
                            ,

                            validationResults =>
                               errors.AddRange(validationResults?.Select(e => e.ErrorMessage))
                            ,

                           serviceMassages =>
                              errors.AddRange(serviceMassages.Select(m => m.Message))
                            );

            return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtendStudnetSupervisorTime(Guid id, Guid studentId)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
            }
            OneOf.OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>> result =
                await _supervisionService.ExtendStudentSupervisionExpire(studentId);

            result.Switch(
                            supervisionInfo =>
                               messages.Add($"Student Supervision Time Extends to {supervisionInfo.endDate}")
                            ,

                            validationResults =>
                               errors.AddRange(validationResults?.Select(e => e.ErrorMessage))
                            ,

                           serviceMassages =>
                              errors.AddRange(serviceMassages.Select(m => m.Message))
                            );

            return RedirectToAction(actionName: nameof(SupervisionController.Update), new { id, errors, messages });
        }
    }
}
