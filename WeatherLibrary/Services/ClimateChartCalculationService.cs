namespace WeatherLibrary.Services;
public class ClimateChartCalculationService
{
    public List<ClimateChartModel> CalculateChartData(List<List<DayModel>> allMonths)
    {
        List<ClimateChartModel> output = CalculateMonthlyStatistics(allMonths);
        List<ClimateChartModel> monthlyDataWithEmpty = AddEmptyData(output);
        ClimateChartModel yearTotals = GetYearlyData(output);
        output = monthlyDataWithEmpty;
        output.Add(yearTotals);

        return output;
    }

    private List<ClimateChartModel> CalculateMonthlyStatistics(List<List<DayModel>> allMonths)
    {
        List<ClimateChartModel> monthlyStatistics = new List<ClimateChartModel>();

        foreach (var monthData in allMonths)
        {
            ClimateChartModel statistics = new ClimateChartModel
            {
                Month = monthData.First().Month, // Assuming all days in a month have the same month value
                RecordHigh = monthData.Any() ? monthData.Max(x => x.MaxTemp) : 0,
                MeanDailyMax = monthData.Any() ? monthData.Average(x => x.MaxTemp) : 0,
                DailyMean = monthData.Any() ? monthData.Average(x => x.MeanTemp) : 0,
                MeanDailyMin = monthData.Any() ? monthData.Average(x => x.MinTemp) : 0,
                RecordLow = monthData.Any() ? monthData.Min(x => x.MinTemp) : 0
            };

            monthlyStatistics.Add(statistics);
        }

        foreach (var item in monthlyStatistics)
        {
            item.MeanMax = CalculateMonthlyMeans(item.Month, allMonths, "max");
            item.MeanMin = CalculateMonthlyMeans(item.Month, allMonths, "min");
        }

        return monthlyStatistics;
    }

    private List<ClimateChartModel> AddEmptyData(List<ClimateChartModel> overallStatistics)
    {
        List<ClimateChartModel> output = new List<ClimateChartModel>();

        foreach (var item in overallStatistics)
        {
            output.Add(item);
        }

        int numberOfMonths = output.Count();

        for (int i = numberOfMonths + 1; i < 13; i++)
        {
            output.Add(new ClimateChartModel
            {
                Month = i,
                RecordHigh = 0,
                MeanMax = 0,
                MeanDailyMax = 0,
                DailyMean = 0,
                MeanDailyMin = 0,
                MeanMin = 0,
                RecordLow = 0
            });
        }

        return output;
    }

    private ClimateChartModel GetYearlyData(List<ClimateChartModel> overallStatistics)
    {
        ClimateChartModel output = new ClimateChartModel
        {
            Month = 0, // Set to 0 as it's not used for overall statistics
            RecordHigh = overallStatistics.Any() ? overallStatistics.Max(x => x.RecordHigh) : 0,
            MeanMax = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanMax) : 0,
            MeanDailyMax = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanDailyMax) : 0,
            DailyMean = overallStatistics.Any() ? overallStatistics.Average(x => x.DailyMean) : 0,
            MeanDailyMin = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanDailyMin) : 0,
            MeanMin = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanMin) : 0,
            RecordLow = overallStatistics.Any() ? overallStatistics.Min(x => x.RecordLow) : 0
        };

        return output;
    }

    private double CalculateMonthlyMeans(int monthIndex, List<List<DayModel>> allMonths, string minMax)
    {
        List<double> output = minMax == "max" ?
            allMonths.SelectMany(x => x)
                     .Where(day => day.Month == monthIndex)
                     .GroupBy(day => day.Year)
                     .Select(group => group.Max(day => day.MaxTemp))
                     .ToList() :
            allMonths.SelectMany(x => x)
                     .Where(day => day.Month == monthIndex)
                     .GroupBy(day => day.Year)
                     .Select(group => group.Min(day => day.MinTemp))
                     .ToList();

        double meanMonthlyTemp = output.Any() ? output.Average() : 0;

        return meanMonthlyTemp;
    }
}
