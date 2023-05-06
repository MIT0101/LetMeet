namespace LetMeet.Data.Dtos.Reports
{
    public interface ISingleReport
    {
        public List<MeetingReportSummary>? EndedMeetings { get; set; }
    }
}
