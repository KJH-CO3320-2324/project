using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Extensions
{
    public static class ScaleTypeExtensions
    {
        public static string ShortDisplayName(this ScaleType type)
        {
            switch (type)
            {
                case ScaleType.Major:
                    return "";
                default:
                    return ShortName(type);
            }
        }

        public static string ShortName(this ScaleType type)
        {
            switch (type)
            {
                case ScaleType.DominantSeven:
                    return "7";
                case ScaleType.Major:
                    return "Maj";
                case ScaleType.MajorSeven:
                    return "maj7";
                case ScaleType.Minor:
                    return "m";
                case ScaleType.MinorSeven:
                    return "m7";
                default:
                    return "";
            }
        }
    }
}
