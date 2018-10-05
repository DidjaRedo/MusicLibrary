using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class Dances {

        protected Dictionary<string, Dance> _dances = new Dictionary<string, Dance>();

        protected Dances() {
            foreach (var dance in All) {
                foreach (var name in dance.AllNames) {
                    if (_dances.ContainsKey(name)) {
                        throw new Exception($"Duplicate dance name {name}.");
                    }
                    _dances.Add(name, dance);
                }
            }
        }

        public Dance this[string name] => _dances[name];

        public static readonly Dance Waltz = new Dance(
            new string[] {"Waltz", "Slow Waltz", "WAL"},
            new Dance.TempoRange() { Category = Dance.DanceCategory.Standard, MinBpm = 28 * 3, MaxBpm = 30 * 3 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Smooth, MinBpm = 28 * 3, MaxBpm = 30 * 3},
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 28 * 3, MaxBpm = 31 * 3}
        );

        public static readonly Dance Tango = new Dance(
            new string[] { "Tango", "TAN" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Standard, MinBpm = 31 * 4, MaxBpm = 33 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Smooth, MinBpm = 30 * 4, MaxBpm = 30 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 30 * 4, MaxBpm = 33 * 4 }
        );

        public static readonly Dance VienneseWaltz = new Dance(
            new string[] { "Viennese Waltz", "VWA" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Standard, MinBpm = 58 * 3, MaxBpm = 60 * 3 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Smooth, MinBpm = 53 * 3, MaxBpm = 54 * 3 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 53 * 3, MaxBpm = 60 * 3 }
        );

        public static readonly Dance Foxtrot = new Dance(
            new string[] { "Foxtrot", "Slow Foxtrot", "FOX" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Standard, MinBpm = 28 * 4, MaxBpm = 30 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Smooth, MinBpm = 30 * 4, MaxBpm = 32 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 28 * 4, MaxBpm = 32 * 4 }
        );

        public static readonly Dance Quickstep = new Dance(
            new string[] { "Quickstep", "QST" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Standard, MinBpm = 50 * 4, MaxBpm = 54 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 50 * 4, MaxBpm = 54 * 4 }
        );

        public static readonly Dance Samba = new Dance(
            new string[] { "Samba", "SAM" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Latin, MinBpm = 50 * 2, MaxBpm = 52 * 2 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 48 * 2, MaxBpm = 52 * 2 }
        );

        public static readonly Dance ChaCha = new Dance(
            new string[] { "Cha-Cha", "Cha Cha", "CHA" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Latin, MinBpm = 30 * 4, MaxBpm = 32 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 30 * 4, MaxBpm = 30 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 30 * 4, MaxBpm = 33 * 4 }
        );


        public static readonly Dance Rumba = new Dance(
            new string[] { "Rumba", "RUM" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Latin, MinBpm = 25 * 4, MaxBpm = 27 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 30 * 4, MaxBpm = 34 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 25 * 4, MaxBpm = 34 * 4 }
        );

        public static readonly Dance PasoDoble = new Dance(
            new string[] { "Paso Doble", "Paso", "PAS" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Latin, MinBpm = 60 * 2, MaxBpm = 62 * 2 }
        );

        public static readonly Dance Jive = new Dance(
            new string[] { "Jive", "JIV" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Latin, MinBpm = 42 * 4, MaxBpm = 44 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 42 * 4, MaxBpm = 44 * 4 }
        );

        public static readonly Dance EastCoastSwing = new Dance(
            new string[] { "East Coast Swing", "Fast Swing", "FSW", "ECS" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 34 * 4, MaxBpm = 36 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 33 * 4, MaxBpm = 36 * 4 }
        );

        public static readonly Dance Mambo = new Dance(
            new string[] { "Mambo", "MAM" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 47 * 4, MaxBpm = 51 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 47 * 4, MaxBpm = 51 * 4 }
        );

        public static readonly Dance Bolero = new Dance(
            new string[] { "Bolero", "BOL" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 24 * 4, MaxBpm = 26 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 24 * 4, MaxBpm = 26 * 4 }
        );

        public static readonly Dance WestCoastSwing = new Dance(
            new string[] { "West Coast Swing", "Slow Swing", "SSW", "WCS" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 28 * 4, MaxBpm = 32 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 37 * 4, MaxBpm = 33 * 4 }
        );

        public static readonly Dance Hustle = new Dance(
            new string[] { "Hustle", "Hustle", "HUS" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Rhythm, MinBpm = 28 * 4, MaxBpm = 30 * 4 },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 28 * 4, MaxBpm = 31 * 4 }
        );

        public static readonly Dance NightclubTwoStep = new Dance(
            new string[] { "Nightclub Two-Step", "2ST" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 54, MaxBpm = 64 }
        );

        public static readonly Dance Salsa = new Dance(
            new string[] { "Salsa", "SAL" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 180, MaxBpm = 300 }
        );

        public static readonly Dance Merengue = new Dance(
            new string[] { "Merengue", "MER" },
            new Dance.TempoRange() { Category = Dance.DanceCategory.Social, MinBpm = 130, MaxBpm = 200 }
        );

        public static readonly Dance[] All = new Dance[] {
            Dances.Waltz, Dances.Tango, Dances.Foxtrot, Dances.Quickstep, Dances.VienneseWaltz,
            Dances.Samba, Dances.ChaCha, Dances.Rumba, Dances.PasoDoble, Dances.Jive,
            Dances.EastCoastSwing, Dances.Mambo, Dances.Bolero,
            Dances.WestCoastSwing, Dances.Hustle, Dances.NightclubTwoStep,
            Dances.Salsa, Dances.Merengue
        };

        public static readonly Dances ByName = new Dances();
    }
}
