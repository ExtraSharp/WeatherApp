namespace WeatherLibrary.Interfaces;
public interface IDataAccess
{
    public List<T> LoadData<T, U>(string sql, U parameters, string connectionString);

    public void SaveData<T>(string sql, T parameters, string connectionString);
}
