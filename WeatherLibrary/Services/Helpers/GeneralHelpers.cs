namespace WeatherLibrary.Services.Helpers;
public static class GeneralHelpers
{
    public static MonthModel ReadDataFromCsv(string filePath)
    {
        MonthModel output = new MonthModel();

        string filename = Path.GetFileName(filePath);
        string yearSubstring = filename.Substring(0, 4);
        string monthSubstring = filename.Substring(4, 2);

        output.Year = int.Parse(yearSubstring);
        output.Month = int.Parse(monthSubstring);

        using (var reader = new StreamReader(filePath))
        {
            // Skip the header line
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                var day = new DayModel
                {
                    Day = int.Parse(values[0]),
                    MeanTemp = TryParseFloat(values[1]),
                    MaxTemp = TryParseFloat(values[2]),
                    MinTemp = TryParseFloat(values[3]),
                    Precipitation = TryParseFloat(values[4]),
                    SunshineHours = TryParseFloat(values[5]),
                    Month = output.Month,
                    Year = output.Year
                };

                output.Days.Add(day);
            }
        }
        return output;
    }

    public static string GetConnectionString(string connectionStringName)
    {
        string output = "";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var config = builder.Build();

        output = config.GetConnectionString(connectionStringName);

        return output;
    }

    public static float TryParseFloat(string value)
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
}
