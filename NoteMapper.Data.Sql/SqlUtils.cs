using System.Data;
using System.Data.SqlClient;

namespace NoteMapper.Data.Sql
{
    internal static class SqlUtils
    {
        public static SqlParameter GetParameter(string name, object? value, SqlDbType type)
        {
            SqlParameter parameter = new();
            parameter.ParameterName = name;
            parameter.SqlDbType = type;

            if (value != null)
            {
                parameter.Value = value;
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
            return parameter;
        }
    }
}
