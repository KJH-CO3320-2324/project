namespace NoteMapper.Core.MusicTheory
{
    public static class Accidental
    {
        public const string Flat = "♭";
        public const string Sharp = "#";

        public static AccidentalType Parse(string s)
        {
            switch (s)
            {
                case Flat:
                    return AccidentalType.Flat;
                case Sharp:
                    return AccidentalType.Sharp;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static AccidentalType? TryParse(string s)
        {
            try
            {
                return Parse(s);
            }
            catch
            {
                return default;
            }
        }

        public static string ToString(AccidentalType type)
        {
            switch (type)
            {
                case AccidentalType.Flat:
                    return Flat;
                case AccidentalType.Sharp:
                    return Sharp;
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
    }
}
