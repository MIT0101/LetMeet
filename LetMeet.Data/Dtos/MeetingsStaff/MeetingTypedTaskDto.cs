using LetMeet.Data.Entites.Meetigs;
using System.ComponentModel.DataAnnotations;

namespace LetMeet.Data.Dtos.MeetingsStaff;

public class MeetingTypedTaskDto<TaskType>
{
    public int id { get; init; }

    [Required(ErrorMessage = "Supervisor Is Required")]
    [Display(Name = "Supervisor")]
    public Guid supervisorId { get; set; }

    [Required(ErrorMessage = "Day Is Required")]
    [Range(minimum: 0, maximum: 6, ErrorMessage = "Day Must be between 0 and 6")]
    public int day { get; set; }

    [Required(ErrorMessage = "Student Is Required")]
    [Display(Name = "Student")]
    public Guid studentId { get; set; }

    [Range(minimum: 0, maximum: 24, ErrorMessage = "Total Meeting Time Must be Between 0 and 24")]

    public int totalTimeHoure { get { return endHour - startHour; } }


    [Required(ErrorMessage = "Date is Required")]
    public DateTime date { get; set; }


    [Required(ErrorMessage = "Meeting Start Hour is Required")]
    [Range(minimum: 0, maximum: 23, ErrorMessage = "Meeting Start Hour Must be Between 0 and 23")]
    public int startHour { get; set; }


    [Required(ErrorMessage = "Meeting End Hour is Required")]
    [Range(minimum: 0, maximum: 24, ErrorMessage = "Meeting End Hour Must be Between 0 and 24")]
    public int endHour { get; set; }

    public string? description { get; set; }

    [Required(ErrorMessage = "Present is Required")]
    public bool hasTasks { get; set; } = false;


    public virtual List<TaskType>? tasks { get; set; }
}


