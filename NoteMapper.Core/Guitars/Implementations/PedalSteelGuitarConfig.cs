namespace NoteMapper.Core.Guitars.Implementations
{
    public class PedalSteelGuitarConfig
    {
        public IReadOnlyCollection<string> Modifiers { get; set; } = Array.Empty<string>();

        public IReadOnlyCollection<KeyValuePair<string, string>> MutuallyExclusiveModifiers { get; set; } 
            = Array.Empty<KeyValuePair<string, string>>();

        public IReadOnlyCollection<string> Strings { get; set; } = Array.Empty<string>();

        public static string GetModifierConfig(string type, string name, params int[] offsets)
        {
            int[][] groupedOffsets = new int[offsets.Length / 2][];
            for (int i = 0; i < offsets.Length; i+= 2)
            {
                groupedOffsets[i / 2] = new int[]
                {
                    offsets[i],
                    offsets[i + 1]
                };
            }

            string offsetConfig = string.Join(",", groupedOffsets.Select(x => GetModifierOffsetConfig(x[0], x[1])));
            return $"{type}|{name}|{offsetConfig}";
        }

        public static string GetStringConfig(int noteIndex, int octaveIndex, int frets)
        {
            return $"n={noteIndex}|o={octaveIndex}|f=0-{frets}";
        }

        private static string GetModifierOffsetConfig(int stringIndex, int offset)
        {
            string sign = offset > 0 ? "+" : "";
            return $"{stringIndex}{sign}{offset}";
        }
    }
}
