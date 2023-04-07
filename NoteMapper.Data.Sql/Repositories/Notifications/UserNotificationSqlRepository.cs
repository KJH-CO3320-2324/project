using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Notifications;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Notifications
{
    public class UserNotificationSqlRepository : SqlRepositoryBase<UserNotification>,
        IUserNotificationRepository
    {
        public UserNotificationSqlRepository(SqlRepositorySettings settings, 
            IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "NotificationId", "HiddenUtc", "Dismissed"
        };

        protected override string TableName => "UserNotifications";

        public Task<ServiceResult> CreateAsync(UserNotification userNotification)
        {
            string sql = $"INSERT INTO {TableName} (UserNotificationId, CreatedUtc, UserId, NotificationId) " +
                         $"VALUES (@UserNotificationId, @CreatedUtc, @UserId, @NotificationId) ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserNotificationId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@UserId", userNotification.UserId, DbType.Guid),
                GetParameter("@NotificationId", userNotification.NotificationId, DbType.Guid)
            });
        }

        public Task<UserNotification?> FindAsync(Guid userId, Guid notificationId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId AND NotificationId = @NotificationId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid),
                GetParameter("@NotificationId", notificationId, DbType.Guid)
            });
        }

        public Task<IReadOnlyCollection<UserNotification>> GetAsync(Guid userId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId ";
            return ReadManyAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid)
            });
        }

        public Task<ServiceResult> UpdateAsync(UserNotification userNotification)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET HiddenUtc = @HiddenUtc, Dismissed = @Dismissed " +
                         "WHERE UserId = @UserId AND NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userNotification.UserId, DbType.Guid),
                GetParameter("@NotificationId", userNotification.NotificationId, DbType.Guid),
                GetParameter("@HiddenUtc", userNotification.HiddenUtc, DbType.DateTime),
                GetParameter("@Dismissed", userNotification.Dismissed, DbType.Boolean)
            });
        }

        protected override UserNotification Map(DbDataReader reader)
        {
            return new UserNotification(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetDateTimeOrNull(2),
                reader.GetBoolean(3));
        }
    }
}
