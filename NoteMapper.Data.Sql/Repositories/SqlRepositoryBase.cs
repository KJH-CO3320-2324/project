using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Sql.Repositories;

namespace NoteMapper.Data.Sql
{
    public abstract class SqlRepositoryBase<T> where T : class
    {
        private readonly DbQueryManager _queryManager;

        protected SqlRepositoryBase(SqlRepositorySettings settings, 
            IApplicationErrorRepository errorRepository)
        {
            _queryManager = new(settings, errorRepository);
        }

        protected abstract IReadOnlyCollection<string> SelectColumns { get; }

        protected string SelectColumnSql => string.Join(", ", SelectColumns.Select(x => $"{TableName}.{x}"));

        protected abstract string TableName { get; }

        protected Task<ServiceResult> ExecuteQueryAsync(string sql, IEnumerable<DbParameter> parameters)
        {
            return _queryManager
                .ExecuteQueryAsync(sql, parameters);
        }        

        protected DbParameter GetParameter(string name, object? value, DbType type)
        {
            return _queryManager.GetParameter(name, value, type);
        }

        protected abstract T Map(DbDataReader reader);

        protected Task<IReadOnlyCollection<T>> ReadManyAsync(string sql, IEnumerable<DbParameter> parameters)
        {
            return ReadManyAsync(sql, parameters, Map);
        }

        protected Task<IReadOnlyCollection<TCustom>> ReadManyAsync<TCustom>(string sql, IEnumerable<DbParameter> parameters,
            Func<DbDataReader, TCustom> map)
        {
            return _queryManager
                .ReadManyAsync(sql, parameters, map);
        }

        protected Task<T?> ReadSingleAsync(string sql, IEnumerable<DbParameter> parameters)
        {
            return _queryManager
                .ReadSingleAsync(sql, parameters, Map);
        }
    }
}