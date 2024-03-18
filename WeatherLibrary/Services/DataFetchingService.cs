namespace WeatherLibrary.Services;
public class DataFetchingService
{
    private readonly IDataRepository _dataRepository;

    public DataFetchingService(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
    }

    public List<List<DayModel>> GetAllMonths()
    {
        List<List<DayModel>> allMonths = new List<List<DayModel>>();

        for (int i = 1; i <= 12; i++)
        {
            allMonths.Add(_dataRepository.GetMonths(i));
        }

        return allMonths;
    }

    public List<DayModel> GetDaysForMonth(int year, int month)
    {
        int numberOfDays = DateTime.DaysInMonth(year, month);
        List<DayModel> daysForMonth = new List<DayModel>();

        for (int i = 1; i <= numberOfDays; i++)
        {
            var days = _dataRepository.GetDays(month, i);
            double maxTemp = days.Average(x => x.MaxTemp);
            double minTemp = days.Average(x => x.MinTemp);
            double precipitation = days.Average(x => x.Precipitation);

            DayModel day = new DayModel
            {
                Day = i,
                MaxTemp = maxTemp,
                MinTemp = minTemp,
                Precipitation = precipitation
            };

            daysForMonth.Add(day);
        }

        return daysForMonth;
    }
}
