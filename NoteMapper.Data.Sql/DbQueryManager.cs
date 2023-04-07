using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Sql.Repositories;

namespace NoteMapper.Data.Sql
{
    public class DbQueryManager
    {
        private readonly IApplicationErrorRepository? _errorRepository;
        private readonly SqlRepositorySettings _settings;

        public DbQueryManager(SqlRepositorySettings settings, IApplicationErrorRepository? errorRepository)
        {
            _errorRepository = errorRepository;
            _settings = settings;
        }

        public async Task<ServiceResult> ExecuteQueryAsync(string sql, IEnumerable<DbParameter> parameters)
        {
            using (DbConnection connection = GetConnection())
            {
                await connection.OpenAsync();

                using (DbTransaction transaction = await connection.BeginTransactionAsync())
                {
                    using (DbCommand command = GetCommand(connection, transaction, sql, parameters))
                    {
                        command.CommandType = CommandType.Text;

                        try
                        {
                            await command.ExecuteNonQueryAsync();
                            await transaction.CommitAsync();
                            return ServiceResult.Successful();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            await LogExceptionAsync(ex, command);
                            return ServiceResult.Failure("");
                        }
                    }
                }
            }
        }

        public DbParameter GetParameter(string name, object? value, DbType type)
        {
            DbParameter parameter;

            switch (_settings.Provider)
            {
                case "sql":
                    parameter = new SqlParameter();                    
                    break;
                case "sqlite":
                    parameter = new SQLiteParameter();
                    break;
                default:
                    throw new NotSupportedException();
            }

            parameter.ParameterName = name;
            parameter.DbType = type;

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

        public async Task<IReadOnlyCollection<T>> ReadManyAsync<T>(string sql, IEnumerable<DbParameter> parameters,
            Func<DbDataReader, T> map)
        {
            using (DbConnection conn = GetConnection())
            {
                using (DbCommand cmd = GetCommand(conn, sql, parameters))
                {
                    await conn.OpenAsync();

                    DbDataReader reader = await cmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                    {
                        return Array.Empty<T>();
                    }

                    List<T> entities = new();
                    do
                    {
                        T entity = map(reader);
                        entities.Add(entity);
                    } while (await reader.ReadAsync());

                    return entities;
                }
            }
        }

        public async Task<T?> ReadSingleAsync<T>(string sql, IEnumerable<DbParameter> parameters,
            Func<DbDataReader, T> map)
        {
            using (DbConnection conn = GetConnection())
            {
                using (DbCommand cmd = GetCommand(conn, sql, parameters))
                {
                    await conn.OpenAsync();

                    try
                    {
                        DbDataReader reader = await cmd.ExecuteReaderAsync();
                        if (!await reader.ReadAsync())
                        {
                            return default;
                        }

                        return map(reader);
                    }
                    catch (Exception ex)
                    {
                        await LogExceptionAsync(ex, cmd);
                        return default;
                    }
                }
            }
        }

        private DbCommand GetCommand(DbConnection connection, string sql, IEnumerable<DbParameter> parameters)
        {
            DbCommand cmd;
            if (connection is SqlConnection sqlConnection)
            {
                cmd = new SqlCommand(sql, sqlConnection);
            }
            else if (connection is SQLiteConnection sqliteConnection)
            {
                cmd = new SQLiteCommand(sql, sqliteConnection);
            }
            else
            {
                throw new NotSupportedException();
            }

            cmd.Parameters.AddRange(parameters.ToArray());
            return cmd;
        }

        private DbCommand GetCommand(DbConnection connection, DbTransaction transaction, string sql, IEnumerable<DbParameter> parameters)
        {
            DbCommand cmd;
            if (connection is SqlConnection sqlConnection && transaction is SqlTransaction sqlTransaction)
            {
                cmd = new SqlCommand(sql, sqlConnection, sqlTransaction);
            }
            else if (connection is SQLiteConnection sqliteConnection && transaction is SQLiteTransaction sqliteTransaction)
            {
                cmd = new SQLiteCommand(sql, sqliteConnection, sqliteTransaction);
            }
            else
            {
                throw new NotSupportedException();
            }

            cmd.Parameters.AddRange(parameters.ToArray());
            return cmd;
        }

        private DbConnection GetConnection()
        {
            switch (_settings.Provider)
            {
                case "sqlserver":
                    return new SqlConnection(_settings.ConnectionString);
                case "sqlite":
                    return new SQLiteConnection(_settings.ConnectionString);
                default:
                    throw new NotSupportedException();
            }
        }

        private Task LogExceptionAsync(Exception ex, DbCommand cmd)
        {
            if (!_settings.LogErrors || _errorRepository == null)
            {
                return Task.CompletedTask;
            }

            ApplicationError error = new(_settings.CurrentEnvironment, ex);
            error.AddProperty("Command.Sql", cmd.CommandText);

            foreach (DbParameter parameter in cmd.Parameters)
            {
                error.AddProperty($"Command.Parameters.{parameter.ParameterName}", parameter.Value?.ToString());
            }

            return _errorRepository.CreateAsync(error);
        }
    }
}
