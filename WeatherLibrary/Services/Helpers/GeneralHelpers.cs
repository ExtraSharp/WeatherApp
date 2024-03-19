namespace WeatherLibrary.Services.Helpers;
public static class GeneralHelpers
{
    public static MonthModel ReadDataFromCsv(string filePath)
    {
        var output = new MonthModel();

        var filename = Path.GetFileName(filePath);
        var yearSubstring = filename.Substring(0, 4);
        var monthSubstring = filename.Substring(4, 2);

        output.Year = int.Parse(yearSubstring);
        output.Month = int.Parse(monthSubstring);

        using var reader = new StreamReader(filePath);
        // Skip the header line
        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            string?[]? values = line?.Split(',');

            var day = new DayModel
            {
                Day = int.Parse(values?[0] ?? string.Empty),
                MeanTemp = TryParseFloat(values?[1]),
                MaxTemp = TryParseFloat(values?[2]),
                MinTemp = TryParseFloat(values?[3]),
                Precipitation = TryParseFloat(values?[4]),
                SunshineHours = TryParseFloat(values?[5]),
                Month = output.Month,
                Year = output.Year
            };

            output.Days?.Add(day);
        }

        return output;
    }

    public static string? GetConnectionString(string connectionStringName)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var config = builder.Build();

        var output = config.GetConnectionString(connectionStringName);

        return output;
    }

    private static float TryParseFloat(string? value)
    {
        if (float.TryParse(value, out var parsedFloat))
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
