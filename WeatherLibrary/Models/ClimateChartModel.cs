namespace WeatherLibrary.Models;
public class ClimateChartModel
{
    public int Month { get; set; }
    public double RecordHigh { get; set; }
    public double MeanMax { get; set; }
    public double MeanDailyMax { get; set; }
    public double DailyMean { get; set; }
    
    public double MeanDailyMin { get; set; }
    public double MeanMin { get; set; }
    public double RecordLow { get; set; }
}
