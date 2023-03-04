using System.Data.SqlClient;

namespace NoteMapper.Data.Sql.Extensions
{
    internal static class SqlDataReaderExtensions
    {
        public static DateTime? GetDateTimeOrNull(this SqlDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetDateTime(i);
        }

        public static int? GetIntOrNull(this SqlDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetInt32(i);
        }

        public static string? GetStringOrNull(this SqlDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetString(i);
        }

        public static async Task<T?> ReadSingleAsync<T>(this SqlDataReader reader, Func<SqlDataReader, T> read)
        {
            if (!await reader.ReadAsync())
            {
                return default;
            }

            return read(reader);
        }
    }
}
