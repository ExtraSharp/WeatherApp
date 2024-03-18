namespace WeatherLibrary.Models;
public class WeatherResponseModel
{
    public Weather weather { get; set; }
    public Source[] sources { get; set; }
}

public class Weather
{
    
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("dew_point")]
    public float DewPoint { get; set; }

    [JsonPropertyName("relative_humidity")]
    public int Humidity { get; set; }
}

public class Source
{
    public int id { get; set; }
    public string dwd_station_id { get; set; }

    [JsonPropertyName("station_name")]
    public string StationName { get; set; }
}



