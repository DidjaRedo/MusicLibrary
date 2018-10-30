using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    public class LibraryFilter
    {
        public Library Library { get; }

        public FilterInfo Default { get; }

        protected Dictionary<string, FilterInfo> _filters = new Dictionary<string, FilterInfo>();
        public IReadOnlyDictionary<string, FilterInfo> Filters;

        public IReadOnlyList<ITrack> AllTracks => Library.Tracks;

        public LibraryFilter(Library library, TrackDanceFilter defaultFilter = null) {
            Library = library;

            Default = new FilterInfo(this, defaultFilter ?? new TrackDanceFilter(), true);
            Filters = new ReadOnlyDictionary<string, FilterInfo>(_filters);
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

        public FilterInfo AddFilter(TrackDanceFilter filter, string name = null) {
            name = name ?? filter.Name;
            if (string.IsNullOrEmpty(name)) {
                throw new ApplicationException("Filter name must be specified.");
            }
            else if (_filters.ContainsKey(name)) {
                throw new ApplicationException($"Filter {name} already defined.");
            }
            var fi = new FilterInfo(this, filter);
            _filters[name] = fi;
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

        public class FilterInfo
        {
            public FilterInfo(LibraryFilter library, TrackDanceFilter filter, bool isDefault = false) {
                Library = library;
                IsDefault = isDefault;
                Filter = filter;
                Tracks.AddRange(Filter.Apply(LibraryTracks));
            }

            public bool Refresh() {
                var newTracks = Filter.Apply(LibraryTracks);
                if (!newTracks.SequenceEqual(Tracks)) {
                    Tracks.Clear();
                    Tracks.AddRange(Filter.Apply(LibraryTracks));
                    return true;
                }
                return false;
            }

            protected LibraryFilter Library { get; }
            protected IEnumerable<ITrack> LibraryTracks => (IsDefault ? Library.AllTracks : Library.Default.Tracks);

            public string Name => Filter.Name;
            public TrackDanceFilter Filter { get; }
            public bool IsDefault { get; }
            public List<ITrack> Tracks { get; } = new List<ITrack>();

            private string _description;
            public string Description {
                get => _description ?? ToString();
                set => _description = value;
            }

            public override string ToString() {
                return Filter.ToString();
            }
        }
    }
}
