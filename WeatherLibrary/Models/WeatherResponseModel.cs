﻿namespace WeatherLibrary.Models;
public class WeatherResponseModel
{
    public Weather? weather { get; set; }
    public Source[] sources { get; set; }
}

public class Weather
{

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    [JsonPropertyName("dew_point")]
    public float? DewPoint { get; set; }

    [JsonPropertyName("relative_humidity")]
    public float? Humidity { get; set; }

    [JsonPropertyName("cloud_cover")]
    public float? CloudCover { get; set; }
}

public class Source
{
    public int id { get; set; }

    [JsonPropertyName("dwd_station_id")]
    public string? StationId { get; set; }

    [JsonPropertyName("station_name")]
    public string? StationName { get; set; }

    [JsonPropertyName("distance")]
    public float? Distance { get; set; }

    [JsonPropertyName("observation_type")]
    public string? ObservationType { get; set; }
}






