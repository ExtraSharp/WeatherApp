using Microsoft.Extensions.Configuration;
using System.Data;
using WeatherLibrary.DataAccess;
using WeatherLibrary.Models;

SqlCrud sql = new SqlCrud(GetConnectionString());

MonthModel month = new MonthModel();
string filePath = "D:\\202203.csv";

using (var reader = new StreamReader(filePath))
{
    // Skip the header line
    reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        var values = line.Split(',');

        string filename = Path.GetFileName(filePath);
        string yearSubstring = filename.Substring(0, 4);
        string monthSubstring = filename.Substring(4, 2);

        month.Year = int.Parse(yearSubstring);
        month.Month = int.Parse(monthSubstring);


        var day = new DayModel
        {
            Day = int.Parse(values[0]),
            MeanTemp = TryParseFloat(values[1]),
            MaxTemp = TryParseFloat(values[2]),
            MinTemp = TryParseFloat(values[3]),
            Precipitation = TryParseFloat(values[4]),
            SunshineHours = TryParseFloat(values[5]),
            Month = month.Month,
            Year = month.Year
        };

        month.Days.Add(day);
    }

    //Console.WriteLine("Day - Mean - max - min - prec - sun");
    //Console.WriteLine("---------------------------------------");

    //foreach (var day in month.Days)
    //{
    //    Console.WriteLine($"{day.Day} - {day.MeanTemp.ToString("0.0")} - {day.MaxTemp.ToString("0.0")} - {day.MinTemp.ToString("0.0")} - {day.Precipitation.ToString("0.0")} - {day.SunshineHours.ToString("0.0")} ");
    //}

     SaveMonth(month);
    // DeleteMonth(2024, 2);
    // UpdateMonth(month);
    //GetDays(2, 2);
    //GetAverages(3, 2);

    //ShowAvailableData();
}

void GetAverages(int month, int day = 0) // Default value 0 for day parameter
{
    var days = (day == 0) ? sql.GetMonths(month) : sql.GetDays(month, day);
    DisplayAverages(days);
}

void DisplayAverages(List<DayModel> days)
{
    // Calculations for average and absolute min/max
    double avgMeanTemp = days.Average(item => item.MeanTemp);
    double avgMaxTemp = days.Average(item => item.MaxTemp);
    double avgMinTemp = days.Average(item => item.MinTemp);
    double avgPrecipitation = days.Average(item => item.Precipitation);
    double avgSunshineHours = days.Average(item => item.SunshineHours);

    double absoluteMaxTemp = days.Max(item => item.MaxTemp);
    double absoluteMinTemp = days.Min(item => item.MinTemp);

    // Display existing data
    foreach (var item in days)
    {
        Console.WriteLine($"{item.Day} - {item.MeanTemp.ToString("0.0")} - {item.MaxTemp.ToString("0.0")} - {item.MinTemp.ToString("0.0")} - {item.Precipitation.ToString("0.0")} - {item.SunshineHours.ToString("0.0")} ");
    }

    // Display averages
    Console.WriteLine($"\nDaily Mean in °C: {avgMeanTemp.ToString("0.0")}");
    Console.WriteLine($"Mean Daily Maximum in °C: {avgMaxTemp.ToString("0.0")}");
    Console.WriteLine($"Mean Daily Minimum in °C: {avgMinTemp.ToString("0.0")}");
    Console.WriteLine($"Average Precipitation: {avgPrecipitation.ToString("0.0")}");
    Console.WriteLine($"Average Sunshine Hours: {avgSunshineHours.ToString("0.0")}");

    // Display absolute min/max
    Console.WriteLine($"\nRecord High in °C: {absoluteMaxTemp.ToString("0.0")}");
    Console.WriteLine($"Record Low in °C: {absoluteMinTemp.ToString("0.0")}");
}

void ShowAvailableData()
{
    var data = sql.GetAvailableMonths();

    foreach (var item in data)
    {
        Console.WriteLine("");
    }
}

void UpdateMonth(MonthModel month)
{
    sql.UpdateMonth(month);

    Console.WriteLine("Month updated");
}

//void DeleteMonth(int year, int month)
//{
//    sql.DeleteMonth(year, month);

//    Console.WriteLine("Month deleted");
//}

void SaveMonth(MonthModel month)
{
    if (CheckIfMonthExists(month.Month, month.Year) == false)
    {
        foreach (var day in month.Days)
        {
            sql.CreateDay(day);
        }
        Console.WriteLine("Data has been saved.");
    }
    else
    {
        Console.WriteLine("Month already exists.");
    }
}

bool CheckIfMonthExists(int month, int year)
{
    bool output = false;

    return sql.MonthExists(year, month) == true;
}

float TryParseFloat(string value)
{
    float parsedFloat;
    if (float.TryParse(value, out parsedFloat))
    {
        return parsedFloat;
    }
    else
    {
        // Handle empty or invalid values (e.g., assign a default value or log a warning)
        return 0; // Assign a default value if parsing fails
    }
}

static string GetConnectionString(string connectionStringName = "Default")
{
    string output = "";

    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    var config = builder.Build();

    output = config.GetConnectionString(connectionStringName);

    return output;
}


