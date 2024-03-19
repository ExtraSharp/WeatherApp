namespace WeatherLibrary.DataAccess;
public class SqlCrud(string? connectionString) : IDataRepository
{
    private readonly SqlDataAccess _db = new SqlDataAccess();

    public DayModel? GetDayById(int year, int month, int day)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from dbo.WeatherData where Year = @Year and Month = @Month and Day = @Day ";

        var output = _db.LoadData<DayModel, dynamic>(sql, new { Year = year, Month = month, Day = day }, connectionString).FirstOrDefault();

        return output;
    }
    public void CreateDay(DayModel day)
    {
        var sql = "insert into dbo.WeatherData (Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours) values (@Year, @Month, @Day, @MaxTemp, @MeanTemp, @MinTemp, @Precipitation, @SunshineHours)";
        _db.SaveData(sql, day, connectionString);
    }

    public void DeleteAllMonths()
    {
        string sql = "delete from dbo.WeatherData";
        _db.SaveData(sql, new { }, connectionString);
    }

    public void DeleteMonth(MonthModel? month)
    {
        string sql = "delete from dbo.WeatherData where Year = @Year and Month = @Month";
        if (month != null) _db.SaveData(sql, new { month.Year, month.Month }, connectionString);
    }

    public void CreateMonth(MonthModel month)
    {
        string sql = "insert into dbo.WeatherData (Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours) values (@Year, @Month, @Day, @MaxTemp, @MeanTemp, @MinTemp, @Precipitation, @SunshineHours)";

        if (month.Days == null) return;
        foreach (var day in month.Days)
        {
            _db.SaveData(sql, day, connectionString);
        }
    }

    public void UpdateMonth(MonthModel month)
    {
        string sql = "update dbo.WeatherData set MaxTemp = @MaxTemp, MeanTemp = @MeanTemp, MinTemp = @MinTemp, Precipitation = @Precipitation, SunshineHours = @SunshineHours where Year = @Year and Month = @Month and Day = @Day";

        if (month.Days == null) return;
        foreach (var day in month.Days)
        {
            _db.SaveData(sql, day, connectionString);
        }
    }

    public List<MonthModel> GetAvailableMonths()
    {
        string sql = "select distinct Year, Month from dbo.WeatherData";

        return _db.LoadData<MonthModel, dynamic>(sql, new { }, connectionString);
    }

    public List<DayModel> GetMonths(int month)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from dbo.WeatherData where Month = @Month";

        List<DayModel> output = _db.LoadData<DayModel, dynamic>(sql, new { Month = month }, connectionString);

        return output;
    }

    public List<DayModel> GetDays(int month, int day)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from dbo.WeatherData where Month = @Month and Day = @Day";

        List<DayModel> output = _db.LoadData<DayModel, dynamic>(sql, new { Month = month, Day = day }, connectionString);

        return output;
    }

    public bool MonthExists(int year, int month)
    {
        string sql = "select count(*) from dbo.WeatherData where Year = @Year and Month = @Month";

        int count = _db.LoadData<int, dynamic>(sql, new { Year = year, Month = month }, connectionString).FirstOrDefault();

        return count > 0;
    }
}

