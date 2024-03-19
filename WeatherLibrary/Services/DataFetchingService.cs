namespace WeatherLibrary.Services;
public class DataFetchingService(IDataRepository dataRepository)
{
    public IEnumerable<List<DayModel>> GetAllMonths()
    {
        var allMonths = new List<List<DayModel>>();

        for (var i = 1; i <= 12; i++)
        {
            allMonths.Add(dataRepository.GetMonths(i));
        }

        return allMonths;
    }

    public List<DayModel> GetDaysForMonth(int year, int month)
    {
        var numberOfDays = DateTime.DaysInMonth(year, month);
        var daysForMonth = new List<DayModel>();

        for (var i = 1; i <= numberOfDays; i++)
        {
            var days = dataRepository.GetDays(month, i);
            var maxTemp = days.Average(x => x.MaxTemp);
            var minTemp = days.Average(x => x.MinTemp);
            var precipitation = days.Average(x => x.Precipitation);

            var day = new DayModel
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
