using Npgsql;
using Dapper;

namespace MyWinFormsApp.Helpers;

public static class DatabaseHelper
{
    private static bool _isConnected = true;
    public static bool IsConnected => _isConnected;
    public static event Action<bool>? ConnectionStatusChanged;

    static DatabaseHelper()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static NpgsqlConnection GetConnection()
        => new(AppConfig.GetConnectionString());

    private static void UpdateConnectionStatus(bool connected)
    {
        if (_isConnected != connected)
        {
            _isConnected = connected;
            ConnectionStatusChanged?.Invoke(connected);
        }
    }

    public static async Task<(bool Success, string Message)> TestConnectionAsync()
    {
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync<string>("SELECT version();");
            UpdateConnectionStatus(true);
            return (true, $"Connected successfully!\n{result}");
        }
        catch (NpgsqlException ex)
        {
            UpdateConnectionStatus(false);
            return (false, $"Database connection failed:\n{ex.Message}");
        }
        catch (Exception ex)
        {
            UpdateConnectionStatus(false);
            return (false, $"Unexpected error:\n{ex.Message}");
        }
    }

    public static async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = GetConnection();
            var result = await connection.QueryAsync<T>(sql, parameters);
            UpdateConnectionStatus(true);
            return result;
        }
        catch (NpgsqlException ex) when (IsConnectionError(ex))
        {
            UpdateConnectionStatus(false);
            throw new DatabaseOfflineException("Database is currently unavailable. Please check your connection.", ex);
        }
    }

    public static async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            UpdateConnectionStatus(true);
            return result;
        }
        catch (NpgsqlException ex) when (IsConnectionError(ex))
        {
            UpdateConnectionStatus(false);
            throw new DatabaseOfflineException("Database is currently unavailable. Please check your connection.", ex);
        }
    }

    public static async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = GetConnection();
            var result = await connection.ExecuteAsync(sql, parameters);
            UpdateConnectionStatus(true);
            return result;
        }
        catch (NpgsqlException ex) when (IsConnectionError(ex))
        {
            UpdateConnectionStatus(false);
            throw new DatabaseOfflineException("Database is currently unavailable. Please check your connection.", ex);
        }
    }

    public static async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = GetConnection();
            var result = await connection.ExecuteScalarAsync<T>(sql, parameters);
            UpdateConnectionStatus(true);
            return result;
        }
        catch (NpgsqlException ex) when (IsConnectionError(ex))
        {
            UpdateConnectionStatus(false);
            throw new DatabaseOfflineException("Database is currently unavailable. Please check your connection.", ex);
        }
    }

    private static bool IsConnectionError(NpgsqlException ex)
    {
        // Connection refused, timeout, broken pipe, etc.
        return ex.InnerException is System.Net.Sockets.SocketException
            || ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase);
    }
}

public class DatabaseOfflineException : Exception
{
    public DatabaseOfflineException(string message, Exception inner) : base(message, inner) { }
}
