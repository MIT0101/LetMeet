using LetMeet.Data.Entites.UsersInfo;

namespace LetMeet.Data.Dtos.Reports;
public class TopStudentAbsence : BasicUserInfoReport
{

    public Stage stage { get; set; }
    public string supervisorName { get; set; }
    public Guid supervisorId { get; set; }
    public int absneceTimes { get; set; }

}