using NoteMapper.Core.MusicTheory;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public class UserPreferences
    {
        public UserPreferences(AccidentalType accidental, bool intervals)
        {
            Accidental = accidental;
            Intervals = intervals;
        }

        public AccidentalType Accidental { get; set; }

        public bool Intervals { get; set; }

        public static UserPreferences FromCollection(IReadOnlyCollection<UserPreference> collection)
        {
            IDictionary<UserPreferenceType, string> dictionary = collection
                .ToDictionary(x => x.Type, x => x.Value);

            return FromDictionary(dictionary);
        }

        public static UserPreferences FromDictionary(IDictionary<UserPreferenceType, string> dictionary)
        {
            AccidentalType accidental = dictionary.ContainsKey(UserPreferenceType.Accidental) &&
                             Enum.TryParse(dictionary[UserPreferenceType.Accidental], out AccidentalType parsedAccidental)
                                ? parsedAccidental
                                : AccidentalType.Sharp;
            bool intervals = dictionary.ContainsKey(UserPreferenceType.Intervals) &&
                            bool.TryParse(dictionary[UserPreferenceType.Intervals], out bool parsedIntervals)
                                ? parsedIntervals
                                : false;
            return new UserPreferences(accidental, intervals);
        }

        public bool Equals(UserPreferences other)
        {
            return Accidental == other.Accidental &&
                Intervals == other.Intervals;
        }

        public IReadOnlyCollection<UserPreference> ToCollection()
        {
            return ToDictionary()
                .Select(x => new UserPreference(x.Key, x.Value))
                .ToArray();
        }

        public IDictionary<UserPreferenceType, string> ToDictionary()
        {
            return new Dictionary<UserPreferenceType, string>
            {
                { UserPreferenceType.Accidental, Accidental.ToString() },
                { UserPreferenceType.Intervals, Intervals.ToString() }
            };
        }
    }
}
