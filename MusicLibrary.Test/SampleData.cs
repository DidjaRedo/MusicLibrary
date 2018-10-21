using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    class SampleData
    {
        public static Library GetTestLibrary() {
            return Library.FromJsonFile("./data/json/library.json");
        }

        public static Dictionary<Dance, DanceTestResult> ExpectedDanceInfo = new Dictionary<Dance, DanceTestResult>() {
            { Dances.Waltz, new DanceTestResult() { TotalTracks = 276, TotalStandard = 239, TotalSmooth = 234, TotalSocial = 45 } },
            { Dances.Tango, new DanceTestResult() { TotalTracks = 233, TotalStandard = 213, TotalSocial = 22 } },
            { Dances.Foxtrot, new DanceTestResult() { TotalTracks = 210, TotalStandard = 125, TotalSmooth = 11, TotalSocial = 73 } },
            { Dances.Quickstep, new DanceTestResult() { TotalTracks = 109, TotalStandard = 102, TotalSocial = 10 } },
            { Dances.VienneseWaltz, new DanceTestResult() { TotalTracks = 88, TotalStandard = 84, TotalSocial = 4 } },
            { Dances.Samba, new DanceTestResult() { TotalTracks = 137, TotalLatin = 92, TotalSocial = 45 } },
            { Dances.ChaCha, new DanceTestResult() { TotalTracks = 136, TotalLatin = 88, TotalRhythm = 5, TotalSocial = 43 } },
            { Dances.Rumba, new DanceTestResult() { TotalTracks = 108, TotalLatin = 79, TotalRhythm = 13, TotalSocial = 16 } },
            { Dances.PasoDoble, new DanceTestResult() { TotalTracks = 19, TotalLatin = 18, TotalSocial = 1 } },
            { Dances.Jive, new DanceTestResult() { TotalTracks = 74, TotalLatin = 68, TotalSocial = 6, TotalSwing = 43 } },
            { Dances.EastCoastSwing, new DanceTestResult() { TotalTracks = 64, TotalRhythm = 16, TotalSocial = 48, TotalSwing = 61 } },
            { Dances.Mambo, new DanceTestResult() { TotalTracks = 26, TotalRhythm = 25, TotalSocial = 1 } },
            { Dances.Bolero, new DanceTestResult() { TotalTracks = 30, TotalRhythm = 16, TotalSocial = 14 } },
            { Dances.WestCoastSwing, new DanceTestResult() { TotalTracks = 11, TotalSocial = 9, TotalSwing = 11 } },
            { Dances.Hustle, new DanceTestResult() { TotalTracks = 40, TotalSocial = 40 } },
            { Dances.NightclubTwoStep, new DanceTestResult() { TotalTracks = 35, TotalSocial = 35 } },
            { Dances.Salsa, new DanceTestResult() { TotalTracks = 10, TotalSocial = 9 } },
            { Dances.Merengue, new DanceTestResult() { TotalTracks = 1, TotalSocial = 1 } }
        };

        public const int NumTracksWithoutDances = 199;

        public static Dictionary<double, int> ExpectedTracksByRating = new Dictionary<double, int>() {
            { 5.0, 11 },
            { 4.5, 14 },
            { 4.0, 14 },
            { 3.5, 471 },
            { 3.0, 471 },
            { 2.5, 881 },
            { 2.0, 1005 },
            { 1.5, 1005 },
            { 1.0, 1110 },
            { 0.5, 1110 },
            { 0.0, 1171 }
        };

        public static Dictionary<DanceReviewStatus, int> ExpectedTracksByReviewStatus = new Dictionary<DanceReviewStatus, int>() {
            { DanceReviewStatus.NeedsReview, 15 },
            { DanceReviewStatus.Reviewed, 1583 }
        };
    }
}
