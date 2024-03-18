namespace WeatherLibrary.Models;
public class MonthModel
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<DayModel>? Days { get; set; } = new();
}
