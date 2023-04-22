using LetMeet.Data.Entites.UsersInfo;

namespace LetMeet.Business;


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
    public void MergeFreeHours(int startHour, int endHour)
    {
        HashSet<int> mutalFreeTime = new HashSet<int>();
        for (int i = startHour; i < endHour; i++)
        {
            if (FreeHours.Contains(i))
            {
                mutalFreeTime.Add(i);
            }
        }
        FreeHours = mutalFreeTime;
        this.UnAvailbleHours = UpdateUnAvailble();

    }
    private HashSet<int> UpdateUnAvailble()
    {
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
    public static Dictionary<int, DayHours> GetMutualDays(List<DayFree> supervisorFreeDays, List<DayFree> studentFreeDays)
    {
        supervisorFreeDays = supervisorFreeDays ?? new List<DayFree>();
        studentFreeDays = studentFreeDays ?? new List<DayFree>();
        Dictionary<int, DayFree> firstMap = new Dictionary<int, DayFree>();
        Dictionary<int, DayHours> mutualDays = new Dictionary<int, DayHours>();
        for (int i = 0; i < supervisorFreeDays.Count; i++)
        {
            firstMap.Add(supervisorFreeDays[i].day, supervisorFreeDays[i]);
        }

        for (int i = 0; i < studentFreeDays.Count; i++)
        {
            if (firstMap.ContainsKey(studentFreeDays[i].day))
            {
                DayFree map1Day = firstMap[supervisorFreeDays[i].day];
                DayFree map2Day = studentFreeDays[i];
                DayHours day = new DayHours(map1Day.day, map1Day.startHour, map1Day.endHour);
                day.MergeFreeHours(map2Day.startHour, map2Day.endHour);
                mutualDays.Add(day.day, day);
            }
        }

        return mutualDays;

    }

}
