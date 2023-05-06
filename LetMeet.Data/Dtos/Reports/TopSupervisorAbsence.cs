namespace LetMeet.Data.Dtos.Reports;

    public class TopSupervisorAbsence :BasicUserInfoReport
    {
        public int absenceTimes { get; set; }
        public Guid studentId { get; set; }
        public string studentName { get; set; }
    }

