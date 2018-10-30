using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    public class LibraryFilter : ObservableBase
    {
        public Library Library { get; }

        public FilteredTracks Default { get; }

        protected Dictionary<string, FilteredTracks> _filters = new Dictionary<string, FilteredTracks>();
        public IReadOnlyDictionary<string, FilteredTracks> Filters;

        public IReadOnlyList<ITrack> AllTracks => Library.Tracks;

        public LibraryFilter(Library library, TrackDanceFilter defaultFilter = null) {
            Library = library;

            Default = new FilteredTracks(this, defaultFilter ?? new TrackDanceFilter(), true);
            Filters = new ReadOnlyDictionary<string, FilteredTracks>(_filters);
        }

        public bool Refresh() {
            if (Default.Refresh()) {
                foreach (var fi in _filters.Values) {
                    fi.Refresh();
                }
                return true;
            }
            return false;
        }

        public FilteredTracks AddFilter(TrackDanceFilter filter, string name = null) {
            name = name ?? filter.Name;
            if (string.IsNullOrEmpty(name)) {
                throw new ApplicationException("Filter name must be specified.");
            }
            else if (_filters.ContainsKey(name)) {
                throw new ApplicationException($"Filter {name} already defined.");
            }
            var fi = new FilteredTracks(this, filter);
            _filters[name] = fi;
            RaisePropertyChanged("Filters");
            return fi;
        }

        public void AddFilters(IEnumerable<TrackDanceFilter> filters) {
            foreach (var filter in filters) {
                AddFilter(filter);
            }
        }

        public void AddFilters(params TrackDanceFilter[] filters) {
            foreach (var filter in filters) {
                AddFilter(filter);
            }
        }

        public void AddDanceFiltersByCategory(params DanceCategory[] categories) {
            foreach (var category in categories) {
                foreach (var dance in Dances.All) {
                    var mask = Dance.ToCategoriesMask(category);
                    var effective = dance.Categories & mask;
                    if (effective != DanceCategories.None) {
                        AddFilter(new TrackDanceFilter(dance) {
                            Name = $"{dance.Name}-{mask.ToString()}",
                            Categories = effective
                        });
                    }
                }
            }
        }

    }
}
