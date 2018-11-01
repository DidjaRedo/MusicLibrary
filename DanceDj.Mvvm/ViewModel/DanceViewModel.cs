using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class DanceViewModel {
        protected DanceViewModel(Dance dance) {
            Dance = dance;
            Categories = dance.EnumerateCategories().ToArray();
        }

        static DanceViewModel() {
            foreach (var dance in Dances.All) {
                AllDances[dance] = new DanceViewModel(dance);
            }
        }

        public Dance Dance { get; }

        public string Name => Dance.Name;
        public string Abbreviation => Dance.Abbreviation;
        public string[] AllNames => Dance.AllNames;
        public DanceCategory[] Categories { get; }
        public DanceCategories CategoriesMask => Dance.Categories;
        public ICollection<Dance.TempoRange> TempoRanges => Dance.TempoRanges;

        public static DanceViewModel GetOrAdd(Dance dance) {
            if (!AllDances.ContainsKey(dance)) {
                AllDances[dance] = new DanceViewModel(dance);
            }
            return AllDances[dance];
        }

        public static IEnumerable<DanceViewModel> ComputeMissingDances(IEnumerable<DanceViewModel> dances) {
            return AllDances.Values.Where((d) => !dances.Contains(d));
        }

        public static IEnumerable<DanceViewModel> GetAllDances() {
            return AllDances.Values.AsEnumerable();
        }

        private static Dictionary<Dance, DanceViewModel> AllDances = new Dictionary<Dance, DanceViewModel>();
    }

    public static class DanceExtensions {
        public static IEnumerable<DanceViewModel> ToViewModel(this IEnumerable<Dance> dances) {
            return dances.Select<Dance, DanceViewModel>((d) => DanceViewModel.GetOrAdd(d));
        }
    }
}
