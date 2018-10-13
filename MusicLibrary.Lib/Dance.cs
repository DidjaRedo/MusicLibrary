using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class Dance
    {
        public enum DanceCategory
        {
            Standard = 0,
            Latin = 1,
            Smooth = 2,
            Rhythm = 3,
            Swing = 4,
            Social = 5,
            Competition = 6
        }

        [Flags]
        public enum DanceCategories
        {
            None = 0,
            Standard = 1 << DanceCategory.Standard,
            Latin = 1 << DanceCategory.Latin,
            Smooth = 1 << DanceCategory.Smooth,
            Rhythm = 1 << DanceCategory.Rhythm,
            Swing = 1 << DanceCategory.Swing,
            Social = 1 << DanceCategory.Social,
            Competition = 1 << DanceCategory.Competition
        };

        public string Name { get; }
        public string[] AllNames { get; }
        public DanceCategories Categories { get; } = DanceCategories.None;
        public TempoRange? GetTempo(DanceCategory category) {
            return _tempos[category];
        }

        private readonly Dictionary<DanceCategory, TempoRange?> _tempos = new Dictionary<DanceCategory, TempoRange?>();

        public Dance(string[] names, IEnumerable<TempoRange> tempos) {
            if ((names == null) || (names.Length < 1)) {
                throw new ApplicationException("Dance constructer needs at least one name");
            }

            Name = names[0];
            AllNames = names;
            foreach (var tempo in tempos) {
                var flag = (DanceCategories)(1 << (int)tempo.Category);
                if ((Categories & flag) != 0) {
                    throw new ApplicationException($"Tempos for {tempo.Category} specified multiple times for {Name}");
                }
                Categories |= flag;
                _tempos[tempo.Category] = tempo;
            }
        }

        public Dance(string[] names, params TempoRange[] tempos) : this(names, (IEnumerable<TempoRange>)tempos) {

        }

        public override string ToString() {
            return Name;
        }

        public struct TempoRange {
            public DanceCategory Category;
            public int MinBpm { get; set; }
            public int MaxBpm { get; set; }
        }
    }
}
