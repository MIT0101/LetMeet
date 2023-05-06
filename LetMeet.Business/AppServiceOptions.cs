using LetMeet.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business;

public class AppServiceOptions
{

    public const string NameOfSection = nameof(AppServiceOptions);

    [Range(minimum: 0, maximum: int.MaxValue)]
    public int MaxStudentsPerSupervisor{ get; set; } = 6;

    [Range(minimum: 1, maximum: 6)]
    public int NumberOfMonthsPerExtend { get; set; } = 6;

    [Range(minimum: 1, maximum: 3)]
    public int MaxExtendTimes { get; set; } = 2;
    [Range(minimum: 0.0, maximum: 24.0)]
    public float PaddingMeetHours { get; set; } = 0f;

    [Range(minimum: 0, maximum: int.MaxValue)]
    public int TopForReportCount { get; set; } = 10;
}
