using System.Data;
using System.Data.SqlClient;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Notifications;
using NoteMapper.Data.Core.Users;
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
            string sql = $"INSERT INTO {TableName} (UserId, NotificationId) " +
                         $"VALUES (@UserId, @NotificationId) ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userNotification.UserId, SqlDbType.UniqueIdentifier),
                GetParameter("@NotificationId", userNotification.NotificationId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<UserNotification?> FindAsync(Guid userId, Guid notificationId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId AND NotificationId = @NotificationId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier),
                GetParameter("@NotificationId", notificationId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<IReadOnlyCollection<UserNotification>> GetAsync(Guid userId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId ";
            return ReadManyAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<ServiceResult> UpdateAsync(UserNotification userNotification)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET HiddenUtc = @HiddenUtc, Dismissed = @Dismissed " +
                         "WHERE UserId = @UserId AND NotificationId = @NotificationId ";
            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userNotification.UserId, SqlDbType.UniqueIdentifier),
                GetParameter("@NotificationId", userNotification.NotificationId, SqlDbType.UniqueIdentifier),
                GetParameter("@HiddenUtc", userNotification.HiddenUtc, SqlDbType.DateTime),
                GetParameter("@Dismissed", userNotification.Dismissed, SqlDbType.Bit)
            });
        }

        protected override UserNotification Map(SqlDataReader reader)
        {
            return new UserNotification(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetDateTimeOrNull(2),
                reader.GetBoolean(3));
        }
    }
}
