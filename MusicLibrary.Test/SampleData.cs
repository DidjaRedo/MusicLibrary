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

        public static Dictionary<Dance, ExpectedDanceInfo> ExpectedDanceInfo = new Dictionary<Dance, ExpectedDanceInfo>() {
            { Dances.Waltz, new ExpectedDanceInfo() { TotalTracks = 276, TotalStandard = 239, TotalSmooth = 234, TotalSocial = 45 } },
            { Dances.Tango, new ExpectedDanceInfo() { TotalTracks = 233, TotalStandard = 213, TotalSocial = 22 } },
            { Dances.Foxtrot, new ExpectedDanceInfo() { TotalTracks = 210, TotalStandard = 125, TotalSmooth = 11, TotalSocial = 72 } },
            { Dances.Quickstep, new ExpectedDanceInfo() { TotalTracks = 109, TotalStandard = 102, TotalSocial = 9 } },
            { Dances.VienneseWaltz, new ExpectedDanceInfo() { TotalTracks = 88, TotalStandard = 84, TotalSocial = 2 } },
            { Dances.Samba, new ExpectedDanceInfo() { TotalTracks = 137, TotalLatin = 92, TotalSocial = 43 } },
            { Dances.ChaCha, new ExpectedDanceInfo() { TotalTracks = 136, TotalLatin = 88, TotalRhythm = 5, TotalSocial = 41 } },
            { Dances.Rumba, new ExpectedDanceInfo() { TotalTracks = 108, TotalLatin = 79, TotalRhythm = 13, TotalSocial = 13 } },
            { Dances.PasoDoble, new ExpectedDanceInfo() { TotalTracks = 19, TotalLatin = 18 } },
            { Dances.Jive, new ExpectedDanceInfo() { TotalTracks = 74, TotalLatin = 68, TotalSocial = 4 } },
            { Dances.EastCoastSwing, new ExpectedDanceInfo() { TotalTracks = 64, TotalRhythm = 16, TotalSocial = 48, TotalSwing = 0 } },
            { Dances.Mambo, new ExpectedDanceInfo() { TotalTracks = 26, TotalRhythm = 25, TotalSocial = 1 } },
            { Dances.Bolero, new ExpectedDanceInfo() { TotalTracks = 30, TotalRhythm = 16, TotalSocial = 13 } },
            { Dances.WestCoastSwing, new ExpectedDanceInfo() { TotalTracks = 11, TotalSocial = 9, TotalSwing = 0 } },
            { Dances.Hustle, new ExpectedDanceInfo() { TotalTracks = 40, TotalSocial = 40 } },
            { Dances.NightclubTwoStep, new ExpectedDanceInfo() { TotalTracks = 35, TotalSocial = 35 } },
            { Dances.Salsa, new ExpectedDanceInfo() { TotalTracks = 10, TotalSocial = 7 } },
            { Dances.Merengue, new ExpectedDanceInfo() { TotalTracks = 1, TotalSocial = 1 } }
        };
    }
}
