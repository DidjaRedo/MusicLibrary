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
            { Dances.Foxtrot, new DanceTestResult() { TotalTracks = 209, TotalStandard = 125, TotalSmooth = 11, TotalSocial = 72 } },
            { Dances.Quickstep, new DanceTestResult() { TotalTracks = 108, TotalStandard = 102, TotalSocial = 9 } },
            { Dances.VienneseWaltz, new DanceTestResult() { TotalTracks = 86, TotalStandard = 84, TotalSocial = 2 } },
            { Dances.Samba, new DanceTestResult() { TotalTracks = 135, TotalLatin = 92, TotalSocial = 43 } },
            { Dances.ChaCha, new DanceTestResult() { TotalTracks = 134, TotalLatin = 88, TotalRhythm = 5, TotalSocial = 41 } },
            { Dances.Rumba, new DanceTestResult() { TotalTracks = 105, TotalLatin = 79, TotalRhythm = 13, TotalSocial = 13 } },
            { Dances.PasoDoble, new DanceTestResult() { TotalTracks = 18, TotalLatin = 18 } },
            { Dances.Jive, new DanceTestResult() { TotalTracks = 72, TotalLatin = 68, TotalSocial = 4 } },
            { Dances.EastCoastSwing, new DanceTestResult() { TotalTracks = 64, TotalRhythm = 16, TotalSocial = 48, TotalSwing = 0 } },
            { Dances.Mambo, new DanceTestResult() { TotalTracks = 26, TotalRhythm = 25, TotalSocial = 1 } },
            { Dances.Bolero, new DanceTestResult() { TotalTracks = 29, TotalRhythm = 16, TotalSocial = 13 } },
            { Dances.WestCoastSwing, new DanceTestResult() { TotalTracks = 11, TotalSocial = 9, TotalSwing = 0 } },
            { Dances.Hustle, new DanceTestResult() { TotalTracks = 40, TotalSocial = 40 } },
            { Dances.NightclubTwoStep, new DanceTestResult() { TotalTracks = 35, TotalSocial = 35 } },
            { Dances.Salsa, new DanceTestResult() { TotalTracks = 8, TotalSocial = 7 } },
            { Dances.Merengue, new DanceTestResult() { TotalTracks = 1, TotalSocial = 1 } }
        };

        public static Dictionary<double, int> ExpectedTracksByRating = new Dictionary<double, int>() {
            { 5.0, 11 },
            { 4.5, 14 },
            { 4.0, 14 },
            { 3.5, 468 },
            { 3.0, 468 },
            { 2.5, 878 },
            { 2.0, 999 },
            { 1.5, 999 },
            { 1.0, 1103 },
            { 0.5, 1103 },
            { 0.0, 1162 }
        };
    }
}
