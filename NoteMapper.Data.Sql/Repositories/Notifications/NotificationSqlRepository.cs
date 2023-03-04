using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string sql = $"INSERT INTO {TableName} (Heading, ContentHtml, Active, HideForDays) " +
                         "VALUES (@Heading, @ContentHtml, @Active, @HideForDays) ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@Active", notification.Active, SqlDbType.Bit),
                GetParameter("@ContentHtml", notification.ContentHtml, SqlDbType.NVarChar),
                GetParameter("@Heading", notification.Heading, SqlDbType.NVarChar),
                GetParameter("@HideForDays", notification.HideForDays, SqlDbType.Int)
            });
        }

        public Task<ServiceResult> DeleteAsync(Guid notificationId)
        {
            string sql = $"DELETE {TableName} " +
                         "WHERE NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@NotificationId", notificationId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<Notification?> FindAsync(Guid notificationId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE NotificationId = @NotificationId ";
            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@NotificationId", notificationId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<IReadOnlyCollection<Notification>> GetActiveNotificationsAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE Active = 1 " +
                         "ORDER BY CreatedUtc ";
            return ReadManyAsync(sql, Array.Empty<SqlParameter>());
        }

        public Task<IReadOnlyCollection<Notification>> GetAllAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} ";
            return ReadManyAsync(sql, Array.Empty<SqlParameter>());
        }

        public Task<ServiceResult> UpdateAsync(Notification notification)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET Active = @Active, Heading = @Heading, ContentHtml = @ContentHtml, HideForDays = @HideForDays " +
                         "WHERE NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@NotificationId", notification.NotificationId, SqlDbType.UniqueIdentifier),
                GetParameter("@Active", notification.Active, SqlDbType.Bit),
                GetParameter("@ContentHtml", notification.ContentHtml, SqlDbType.NVarChar),
                GetParameter("@Heading", notification.Heading, SqlDbType.NVarChar),
                GetParameter("@HideForDays", notification.HideForDays, SqlDbType.Int)
            });
        }

        protected override Notification Map(SqlDataReader reader)
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
