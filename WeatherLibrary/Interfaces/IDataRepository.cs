namespace WeatherLibrary.Interfaces;
public interface IDataRepository
{
    List<MonthModel> GetAvailableMonths();
    void DeleteMonth(MonthModel? month);
    void DeleteAllMonths();
    bool MonthExists(int year, int month);
    void CreateMonth(MonthModel month);
    void UpdateMonth(MonthModel month);
    List<DayModel> GetMonths(int month);
    List<DayModel> GetDays(int month, int day);
}
