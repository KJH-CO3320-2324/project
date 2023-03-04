using NoteMapper.Core.Extensions;

namespace NoteMapper.Core.Tests.Extensions
{
    public static class ListExtensionsTests
    {
        [Test]
        public static void SetCount_SameCount_DoesNotAlter()
        {
            List<int> list = new()
            {
                1, 2, 3
            };

            list.SetCount(3, 0);

            CollectionAssert.AreEqual(new[]
            {
                1, 2, 3
            }, list);
        }

        [Test]
        public static void SetCount_AddItems()
        {
            List<int> list = new()
            {
                1, 2
            };

            list.SetCount(3, 0);

            CollectionAssert.AreEqual(new[]
            {
                1, 2, 0
            }, list);
        }

        [Test]
        public static void SetCount_RemoveItems()
        {
            List<int> list = new()
            {
                1, 2, 3, 4
            };

            list.SetCount(3, 0);

            CollectionAssert.AreEqual(new[]
            {
                1, 2, 3
            }, list);
        }
    }
}
