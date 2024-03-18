namespace WeatherLibrary.DataAccess;
public class DataRepository : IDataRepository
{
    private readonly SqliteCrud sql;

    public DataRepository(string connectionString)
    {
        sql = new SqliteCrud(connectionString);
    }

    public List<MonthModel> GetAvailableMonths()
    {
        return sql.GetAvailableMonths();
    }

    public void DeleteMonth(MonthModel month)
    {
        sql.DeleteMonth(month);
    }

    public void DeleteAllMonths()
    {
        sql.DeleteAllMonths();
    }

    public bool MonthExists(int month, int year)
    {
        return sql.MonthExists(year, month) == true;
    }

    public void CreateMonth(MonthModel month)
    {
        sql.CreateMonth(month);
    }

    public void UpdateMonth(MonthModel month)
    {
        sql.UpdateMonth(month);
    }

    public List<DayModel> GetMonths(int month)
    {
        return sql.GetMonths(month);
    }

    public List<DayModel> GetDays(int month, int day)
    {
        return sql.GetDays(month, day);
    }
}
