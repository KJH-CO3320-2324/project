using System.Data.Common;

namespace NoteMapper.Data.Sql.Extensions
{
    internal static class DbDataReaderExtensions
    {
        public static DateTime? GetDateTimeOrNull(this DbDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetDateTime(i);
        }

        public static int? GetIntOrNull(this DbDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetInt32(i);
        }

        public static string? GetStringOrNull(this DbDataReader reader, int i)
        {
            return reader.IsDBNull(i)
                ? null
                : reader.GetString(i);
        }

        public static async Task<T?> ReadSingleAsync<T>(this DbDataReader reader, Func<DbDataReader, T> read)
        {
            if (!await reader.ReadAsync())
            {
                return default;
            }

            return read(reader);
        }
    }
}
