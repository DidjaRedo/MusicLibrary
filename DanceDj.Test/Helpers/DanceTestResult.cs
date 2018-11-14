using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;

namespace DanceDj.Test.Helpers
{
    class DanceTestResult : IEquatable<DanceTestResult>
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

        public void SetTotalForCategory(DanceCategory cat, int total) {
            switch (cat) {
                case DanceCategory.Standard:
                    TotalStandard = total;
                    break;
                case DanceCategory.Latin:
                    TotalLatin = total;
                    break;
                case DanceCategory.Smooth:
                    TotalSmooth = total;
                    break;
                case DanceCategory.Rhythm:
                    TotalRhythm = total;
                    break;
                case DanceCategory.Swing:
                    TotalSwing = total;
                    break;
                case DanceCategory.Social:
                    TotalSocial = total;
                    break;
                case DanceCategory.Competition:
                    TotalCompetition = total;
                    break;
                default:
                    throw new Exception($"Unexpected DanceCategory {cat}");
            }
        }

        bool IEquatable<DanceTestResult>.Equals(DanceTestResult other) {
            return (TotalTracks == other.TotalTracks)
                && (TotalStandard == other.TotalStandard) && (TotalLatin == other.TotalLatin)
                && (TotalSmooth == other.TotalSmooth) && (TotalRhythm == other.TotalRhythm)
                && (TotalSwing == other.TotalSwing)
                && (TotalSocial == other.TotalSocial) && (TotalCompetition == other.TotalCompetition);
        }
    }
}
