using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace BlogDataLibrary.Database;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _configuration;

    public SqlDataAccess(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<T> LoadData<T, U>(
        string sqlStatement,
        U parameters,
        string connectionStringName,
        bool isStoredProcedure)
    {
        using IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName));
        CommandType commandType = GetCommandType(isStoredProcedure);

        return connection.Query<T>(sqlStatement, parameters, commandType: commandType).ToList();
    }

    public void SaveData<T>(
        string sqlStatement,
        T parameters,
        string connectionStringName,
        bool isStoredProcedure)
    {
        using IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName));
        CommandType commandType = GetCommandType(isStoredProcedure);

        connection.Execute(sqlStatement, parameters, commandType: commandType);
    }

    public T ExecuteScalar<T, U>(
        string sqlStatement,
        U parameters,
        string connectionStringName,
        bool isStoredProcedure)
    {
        using IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName));
        CommandType commandType = GetCommandType(isStoredProcedure);

        T? result = connection.ExecuteScalar<T>(sqlStatement, parameters, commandType: commandType);

        if (result is null)
        {
            throw new InvalidOperationException("The SQL scalar command returned no value.");
        }

        return result;
    }

    private string GetConnectionString(string connectionStringName)
    {
        string? connectionString = _configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' is missing or empty.");
        }

        return connectionString;
    }

    private static CommandType GetCommandType(bool isStoredProcedure)
    {
        return isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
    }
}
