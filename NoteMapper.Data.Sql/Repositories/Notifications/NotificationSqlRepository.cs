using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Notifications;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Notifications
{
    public class NotificationSqlRepository : SqlRepositoryBase<Notification>, INotificationRepository
    {
        public NotificationSqlRepository(SqlRepositorySettings settings, 
            IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "NotificationId", "Heading", "ContentHtml", "Active", "HideForDays"
        };

        protected override string TableName => "Notifications";

        public Task<ServiceResult> CreateAsync(Notification notification)
        {
            string sql = $"INSERT INTO {TableName} (NotificationId, CreatedUtc, Heading, ContentHtml, Active, HideForDays) " +
                         "VALUES (@NotificationId, @CreatedUtc, @Heading, @ContentHtml, @Active, @HideForDays) ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@NotificationId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@Active", notification.Active, DbType.Boolean),
                GetParameter("@ContentHtml", notification.ContentHtml, DbType.String),
                GetParameter("@Heading", notification.Heading, DbType.String),
                GetParameter("@HideForDays", notification.HideForDays, DbType.Int32)
            });
        }

        public Task<ServiceResult> DeleteAsync(Guid notificationId)
        {
            string sql = $"DELETE {TableName} " +
                         "WHERE NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@NotificationId", notificationId, DbType.Guid)
            });
        }

        public Task<Notification?> FindAsync(Guid notificationId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE NotificationId = @NotificationId ";
            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@NotificationId", notificationId, DbType.Guid)
            });
        }

        public Task<IReadOnlyCollection<Notification>> GetActiveNotificationsAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE Active = 1 " +
                         "ORDER BY CreatedUtc ";
            return ReadManyAsync(sql, Array.Empty<DbParameter>());
        }

        public Task<IReadOnlyCollection<Notification>> GetAllAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} ";
            return ReadManyAsync(sql, Array.Empty<DbParameter>());
        }

        public Task<ServiceResult> UpdateAsync(Notification notification)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET Active = @Active, Heading = @Heading, ContentHtml = @ContentHtml, HideForDays = @HideForDays " +
                         "WHERE NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@NotificationId", notification.NotificationId, DbType.Guid),
                GetParameter("@Active", notification.Active, DbType.Boolean),
                GetParameter("@ContentHtml", notification.ContentHtml, DbType.String),
                GetParameter("@Heading", notification.Heading, DbType.String),
                GetParameter("@HideForDays", notification.HideForDays, DbType.Int32)
            });
        }

        protected override Notification Map(DbDataReader reader)
        {
            return new Notification(
                reader.GetGuid(0),
                reader.GetStringOrNull(1),
                reader.GetString(2),
                reader.GetBoolean(3),
                reader.GetInt32(4));
        }
    }
}
