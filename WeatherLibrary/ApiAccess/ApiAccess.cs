namespace WeatherLibrary.ApiAccess;
public class ApiAccess
{
    private readonly RestClient _client;

    public ApiAccess()
    {
        const string baseUrl = "https://api.brightsky.dev/";
        _client = new RestClient(baseUrl);
    }

    public async Task<WeatherResponseModel?> GetWeatherData()
    {
        var response = await _client.GetJsonAsync<WeatherResponseModel>($"current_weather?lat=51.05&lon=13.73");

        return response;
    }
}
