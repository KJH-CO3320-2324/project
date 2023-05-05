using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteMapper.Data.Core.Notifications;

namespace NoteMapper.Data.Core.Tests.Notifications
{
    public static class UserNotificationTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public static void ShouldShow_DefaultUserNotification_ReturnsActive(bool active)
        {
            UserNotification userNotification = GetUserNotification();
            Notification notification = GetNotification(active: active);

            bool result = userNotification.ShouldShow(notification);
           
            Assert.That(result, Is.EqualTo(active));
        }

        [Test]
        public static void ShowShow_Dismissed_ReturnsFalse()
        {
            UserNotification userNotification = GetUserNotification(dismissed: true);
            Notification notification = GetNotification();

            bool result = userNotification.ShouldShow(notification);

            Assert.That(result, Is.False);
        }

        [Test]
        public static void ShowShow_Hidden_ReturnsTrueIfHiddenTimeElapsed()
        {
            UserNotification userNotification = GetUserNotification(hiddenUtc: DateTime.UtcNow.AddDays(-10));
            Notification notification = GetNotification(hideForDays: 1);

            bool result = userNotification.ShouldShow(notification);

            Assert.That(result, Is.True);
        }

        [Test]
        public static void ShowShow_Hidden_ReturnsFalseIfHiddenTimeNotElapsed()
        {
            UserNotification userNotification = GetUserNotification(hiddenUtc: DateTime.UtcNow.AddDays(-1));
            Notification notification = GetNotification(hideForDays: 2);

            bool result = userNotification.ShouldShow(notification);

            Assert.That(result, Is.False);
        }

        private static Notification GetNotification(int hideForDays = 1, 
            bool active = true)
        {
            return new Notification(Guid.Empty, null, "content", active, hideForDays);
        }

        private static UserNotification GetUserNotification(DateTime? hiddenUtc = null, 
            bool dismissed = false)
        {
            return new UserNotification(Guid.Empty, Guid.Empty, hiddenUtc, dismissed);
        }
    }
}
