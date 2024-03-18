namespace WeatherLibrary.ApiAccess;
public class ApiAccess
{
    readonly RestClient _client;

    public ApiAccess()
    {
        string baseUrl = "https://api.brightsky.dev/";
        _client = new RestClient(baseUrl);
    }

    public async Task<WeatherResponseModel> GetWeatherData()
    {
        var response = await _client.GetJsonAsync<WeatherResponseModel>("current_weather?dwd_station_id=01050");

        return response;
    }
}
