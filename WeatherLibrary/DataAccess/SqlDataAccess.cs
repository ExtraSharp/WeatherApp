﻿namespace WeatherLibrary.DataAccess;
public class SqlDataAccess : IDataAccess
{
    public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
    {
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            return connection.Query<T>(sql, parameters).ToList();
        }
    }

    public void SaveData<T>(string sql, T parameters, string connectionString)
    {
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            connection.Execute(sql, parameters);
        }
    }
}
