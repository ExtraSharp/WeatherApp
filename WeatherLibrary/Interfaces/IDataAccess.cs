namespace WeatherLibrary.Interfaces;
public interface IDataAccess
{
    public List<T> LoadData<T, TU>(string sql, TU parameters, string? connectionString);

    public void SaveData<T>(string sql, T parameters, string? connectionString);
}
