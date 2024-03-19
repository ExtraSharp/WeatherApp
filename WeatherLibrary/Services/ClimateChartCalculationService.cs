namespace WeatherLibrary.Services;
public static class ClimateChartCalculationService
{
    public static List<ClimateChartModel> CalculateChartData(List<List<DayModel>> allMonths)
    {
        var output = CalculateMonthlyStatistics(allMonths);
        var monthlyDataWithEmpty = AddEmptyData(output);
        var yearTotals = GetYearTotals(output, allMonths);
        output = monthlyDataWithEmpty;
        output.Add(yearTotals);
        
        return output;
    }

    private static List<ClimateChartModel> CalculateMonthlyStatistics(List<List<DayModel>> allMonths)
    {
        List<ClimateChartModel> monthlyStatistics = new List<ClimateChartModel>();

        foreach (var monthData in allMonths)
        {
            ClimateChartModel statistics = new ClimateChartModel
            {
                Month = monthData.First().Month,
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

    private static List<ClimateChartModel> AddEmptyData(List<ClimateChartModel> overallStatistics)
    {
        var output = overallStatistics.ToList();

        var numberOfMonths = output.Count;

        for (var i = numberOfMonths + 1; i < 13; i++)
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

    private static ClimateChartModel GetYearTotals(List<ClimateChartModel> overallStatistics, List<List<DayModel>> allMonths)
    {
        ClimateChartModel output = new ClimateChartModel
        {
            Month = 0, // Set to 0 as it's not used for overall statistics
            RecordHigh = overallStatistics.Any() ? overallStatistics.Max(x => x.RecordHigh) : 0,
            MeanMax = CalculateYearlyMeans(allMonths, "max"),
            MeanDailyMax = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanDailyMax) : 0,
            DailyMean = overallStatistics.Any() ? overallStatistics.Average(x => x.DailyMean) : 0,
            MeanDailyMin = overallStatistics.Any() ? overallStatistics.Average(x => x.MeanDailyMin) : 0,
            MeanMin = CalculateYearlyMeans(allMonths, "min"),
            RecordLow = overallStatistics.Any() ? overallStatistics.Min(x => x.RecordLow) : 0
        };

        return output;
    }

    private static double CalculateMonthlyMeans(int monthIndex, List<List<DayModel>> allMonths, string minMax)
    {
        var output = minMax == "max" ?
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

        var meanMonthlyTemp = output.Any() ? output.Average() : 0;

        return meanMonthlyTemp;
    }

    private static double CalculateYearlyMeans(IEnumerable<List<DayModel>> allMonths, string minMax)
    {
        var yearlyTemps = minMax == "max" ?
            allMonths.SelectMany(x => x)
                     .GroupBy(day => day.Year)
                     .Where(group => group.Count() >= 365) // Filter only years with at least 365 days
                     .Select(group => group.Max(day => day.MaxTemp))
                     .ToList() :
            allMonths.SelectMany(x => x)
                     .GroupBy(day => day.Year)
                     .Where(group => group.Count() >= 365) // Filter only years with at least 365 days
                     .Select(group => group.Min(day => day.MinTemp))
                     .ToList();

        var overallYearlyMean = yearlyTemps.Any() ? yearlyTemps.Average() : 0;

        return overallYearlyMean;
    }
}
