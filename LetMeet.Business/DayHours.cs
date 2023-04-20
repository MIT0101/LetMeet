namespace LetMeet.Business.Implemintation;

public partial class MeetingService
{
    public class DayHours
    {
        public int day { get; init; }
        public int startHour { get; init; }
        public int endHour { get; init; }

        private ISet<int> FreeHours;
        private ISet<int> UnAvailbleHours;
        public DayHours(int day, int startHour, int endHour)
        {
            this.day = day;
            this.startHour = startHour;
            this.endHour = endHour;
            GetFreeHours();
            GetUnAvailableHours();
        }
        public void AddFreeHours(int startHour,int endHour) {
            HashSet<int> mutalFreeTime= new HashSet<int>();
            for (int i = startHour; i < endHour; i++)
            {
                if (FreeHours.Contains(i)) { 
                mutalFreeTime.Add(i);
                }
            }
            FreeHours = mutalFreeTime;
            this.UnAvailbleHours = UpdateUnAvailble();
            
        }
        private HashSet<int> UpdateUnAvailble() {
            var availble = new HashSet<int>();
            for (int i = 0; i <= 24; i++)
            {
                if (!FreeHours.Contains(i))
                {
                    availble.Add(i);
                }
            }
            return availble;
        }
        public ISet<int> GetFreeHours()
        {
            if (FreeHours != null)
            {
                return FreeHours;
            }
            FreeHours = new HashSet<int>();
            for (int i = startHour; i < endHour; i++)
            {
                FreeHours.Add(i);
            }
            return FreeHours;
        }

        public ISet<int> GetUnAvailableHours()
        {
            if (UnAvailbleHours != null)
            {
                return UnAvailbleHours;
            }
            this.UnAvailbleHours = UpdateUnAvailble();

            return UnAvailbleHours;

        }

        public bool CanAdd(int toAddStartHour, int toAddEndHour)
        {
            for (int i = toAddStartHour; i < toAddEndHour; i++)
            {
                if (!FreeHours.Contains(i))
                {
                    return false;
                }
            }
            return true;


        }

    }
}
