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
        public TrackDanceFilter Default { get; }

        protected Dictionary<string, FilterInfo> _Filters = new Dictionary<string, FilterInfo>();
        public IReadOnlyDictionary<string, FilterInfo> Filters;

        public LibraryFilter(Library library, TrackDanceFilter defaultFilter = null) {
            Library = library;
            Default = defaultFilter ?? new TrackDanceFilter();
            Filters = new ReadOnlyDictionary<string, FilterInfo>(_Filters);
        }

        public FilterInfo AddFilter(TrackDanceFilter filter, string name = null) {
            name = name ?? filter.Name;
            if (string.IsNullOrEmpty(name)) {
                throw new ApplicationException("Filter name must be specified.");
            }
            else if (_Filters.ContainsKey(name)) {
                throw new ApplicationException($"Filter {name} already defined.");
            }
            var fi = new FilterInfo(Library, filter, Default);
            _Filters[name] = fi;
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
            public FilterInfo(Library library, TrackDanceFilter configured, TrackDanceFilter defaultFilter) {
                Configured = configured;
                Effective = TrackDanceFilter.Merge(defaultFilter, configured);
                Tracks.AddRange(Effective.Filter(library.Tracks));
            }

            public TrackDanceFilter Configured { get; }
            public TrackDanceFilter Effective { get; set; }
            public List<ITrack> Tracks { get; } = new List<ITrack>();

            public override string ToString() {
                return Effective.ToString();
            }
        }
    }
}
