namespace WeatherLibrary.DataAccess;
public class SqliteCrud(string? connectionString) : IDataRepository
{
    private readonly SqliteDataAccess db = new SqliteDataAccess();

    public DayModel GetDayById(int year, int month, int day)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from WeatherData where Year = @Year and Month = @Month and Day = @Day ";

        DayModel output = new DayModel();

        output = db.LoadData<DayModel, dynamic>(sql, new { Year = year, Month = month, Day = day }, connectionString).FirstOrDefault();

        return output;
    }
    public void CreateDay(DayModel day)
    {
        string sql = "insert into WeatherData (Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours) values (@Year, @Month, @Day, @MaxTemp, @MeanTemp, @MinTemp, @Precipitation, @SunshineHours)";
        db.SaveData(sql, day, connectionString);
    }

    public void DeleteAllMonths()
    {
        string sql = "delete from dbo.WeatherData";
        db.SaveData(sql, new { }, connectionString);
    }

    public void DeleteMonth(MonthModel? month)
    {
        string sql = "delete from WeatherData where Year = @Year and Month = @Month";
        db.SaveData(sql, new { month.Year, month.Month }, connectionString);
    }

    public void CreateMonth(MonthModel month)
    {
        string sql = "insert into WeatherData (Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours) values (@Year, @Month, @Day, @MaxTemp, @MeanTemp, @MinTemp, @Precipitation, @SunshineHours)";

        foreach (var day in month.Days)
        {
            db.SaveData(sql, day, connectionString);
        }
    }

    public void UpdateMonth(MonthModel month)
    {
        string sql = "update WeatherData set MaxTemp = @MaxTemp, MeanTemp = @MeanTemp, MinTemp = @MinTemp, Precipitation = @Precipitation, SunshineHours = @SunshineHours where Year = @Year and Month = @Month and Day = @Day";

        foreach (var day in month.Days)
        {
            db.SaveData(sql, day, connectionString);
        }
    }

    public List<MonthModel> GetAvailableMonths()
    {
        string sql = "select distinct Year, Month from WeatherData";

        return db.LoadData<MonthModel, dynamic>(sql, new { }, connectionString);
    }

    public List<DayModel> GetMonths(int month)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from WeatherData where Month = @Month";

        List<DayModel> output = db.LoadData<DayModel, dynamic>(sql, new { Month = month }, connectionString);

        return output;
    }

    public List<DayModel> GetDays(int month, int day)
    {
        string sql = "select Id, Year, Month, Day, MaxTemp, MeanTemp, MinTemp, Precipitation, SunshineHours from WeatherData where Month = @Month and Day = @Day";

        List<DayModel> output = db.LoadData<DayModel, dynamic>(sql, new { Month = month, Day = day }, connectionString);

        return output;
    }

    public bool MonthExists(int year, int month)
    {
        string sql = "select count(*) from WeatherData where Year = @Year and Month = @Month";

        int count = db.LoadData<int, dynamic>(sql, new { Year = year, Month = month }, connectionString).FirstOrDefault();

        return count > 0;
    }
}
