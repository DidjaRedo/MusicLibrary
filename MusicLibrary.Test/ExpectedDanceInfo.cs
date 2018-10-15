using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    class ExpectedDanceInfo
    {
        public int TotalTracks { get; set; }
        public int TotalStandard { get; set; }
        public int TotalLatin { get; set; }
        public int TotalSmooth { get; set; }
        public int TotalRhythm { get; set; }
        public int TotalSwing { get; set; }
        public int TotalSocial { get; set; }
        public int TotalCompetition { get; set; }

        public int GetTotalForCategory(DanceCategory cat) {
            switch (cat) {
                case DanceCategory.Standard:
                    return TotalStandard;
                case DanceCategory.Latin:
                    return TotalLatin;
                case DanceCategory.Smooth:
                    return TotalSmooth;
                case DanceCategory.Rhythm:
                    return TotalRhythm;
                case DanceCategory.Swing:
                    return TotalSwing;
                case DanceCategory.Social:
                    return TotalSocial;
                case DanceCategory.Competition:
                    return TotalCompetition;
                default:
                    throw new Exception($"Unexpected DanceCategory {cat}");
            }
        }
    }
}
