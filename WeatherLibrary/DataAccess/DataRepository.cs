namespace WeatherLibrary.DataAccess;
public class DataRepository(string? connectionString) : IDataRepository
{
    private readonly SqliteCrud _sql = new(connectionString);

    public List<MonthModel> GetAvailableMonths()
    {
        return _sql.GetAvailableMonths();
    }

    public void DeleteMonth(MonthModel? month)
    {
        _sql.DeleteMonth(month);
    }

    public void DeleteAllMonths()
    {
        _sql.DeleteAllMonths();
    }

    public bool MonthExists(int month, int year)
    {
        return _sql.MonthExists(year, month);
    }

    public void CreateMonth(MonthModel month)
    {
        _sql.CreateMonth(month);
    }

    public void UpdateMonth(MonthModel month)
    {
        _sql.UpdateMonth(month);
    }

    public List<DayModel> GetMonths(int month)
    {
        return _sql.GetMonths(month);
    }

    public List<DayModel> GetDays(int month, int day)
    {
        return _sql.GetDays(month, day);
    }
}
