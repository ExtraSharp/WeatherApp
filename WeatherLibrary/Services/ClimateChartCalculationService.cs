namespace WeatherLibrary.Services;
public class ClimateChartCalculationService
{
    public List<double> CalculateOverallStatistics(List<List<DayModel>> allMonths)
    {
        List<double> allRecordHighs = new List<double>();
        List<double> allRecordLows = new List<double>();
        List<double> allDailyMax = new List<double>();
        List<double> allDailyMeans = new List<double>();
        List<double> allDailyMins = new List<double>();
        //List<double> allPrecipitation = new List<double>();

        foreach (var monthData in allMonths)
        {
            allDailyMax.Add(monthData.Any() ? monthData.Average(x => x.MaxTemp) : 0);
            allRecordHighs.Add(monthData.Any() ? monthData.Max(x => x.MaxTemp) : 0);
            allRecordLows.Add(monthData.Any() ? monthData.Min(x => x.MinTemp) : 0);
            allDailyMeans.Add(monthData.Any() ? monthData.Average(x => x.MeanTemp) : 0);
            allDailyMins.Add(monthData.Any() ? monthData.Average(x => x.MinTemp) : 0);
            //allPrecipitation.Add(monthData.Any() ? monthData.Sum(x => x.Precipitation) : 0);
        }

        allRecordHighs.Add(allRecordHighs.Max());
        allRecordLows.Add(allRecordLows.Min());
        allDailyMax.Add(allDailyMax.Any() ? allDailyMax.Average() : 0);
        allDailyMeans.Add(allDailyMeans.Any() ? allDailyMeans.Average() : 0);
        allDailyMins.Add(allDailyMins.Any() ? allDailyMins.Average() : 0);
        //allPrecipitation.Add(allPrecipitation.Any() ? allPrecipitation.Sum() : 0);

        List<double> overallStatistics =
        [
            .. allRecordHighs,
            .. allRecordLows,
            .. allDailyMax,
            .. allDailyMeans,
            .. allDailyMins,
           // .. allPrecipitation
        ];

        return overallStatistics;
    }

    public List<double> CalculateMeanMonthlyMaxMins(List<List<DayModel>> allMonths)
    {
        List<double> allMeanMax = new List<double>();
        List<double> allMeanMin = new List<double>();

        for (int monthIndex = 0; monthIndex < 12; monthIndex++)
        {
            List<double> monthlyMaxTemps = allMonths.SelectMany(x => x)
                                                     .Where(day => day.Month == monthIndex + 1)
                                                     .GroupBy(day => day.Year)
                                                     .Select(group => group.Max(day => day.MaxTemp))
                                                     .ToList();

            List<double> monthlyMinTemps = allMonths.SelectMany(x => x)
                                                     .Where(day => day.Month == monthIndex + 1)
                                                     .GroupBy(day => day.Year)
                                                     .Select(group => group.Min(day => day.MinTemp))
                                                     .ToList();

            double meanMonthlyMax = monthlyMaxTemps.Any() ? monthlyMaxTemps.Average() : 0;
            double meanMonthlyMin = monthlyMinTemps.Any() ? monthlyMinTemps.Average() : 0;

            allMeanMax.Add(meanMonthlyMax);
            allMeanMin.Add(meanMonthlyMin);
        }

        allMeanMax.Add(allMeanMax.Any() ? allMeanMax.Average() : 0);
        allMeanMin.Add(allMeanMin.Any() ? allMeanMin.Average() : 0);

        List<double> monthlyStatistics = new List<double>();
        monthlyStatistics.AddRange(allMeanMax);
        monthlyStatistics.AddRange(allMeanMin);

        return monthlyStatistics;
    }   
}
