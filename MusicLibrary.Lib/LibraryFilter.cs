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

        protected ObservableCollection<FilterGroup> _filterGroups = new ObservableCollection<FilterGroup>();
        public ReadOnlyObservableCollection<FilterGroup> FilterGroups;

        protected FilterGroup _allFilters = new FilterGroup("Default", true);
        public ReadOnlyObservableCollection<FilteredTracks> AllFilters => _allFilters.All;

        public IReadOnlyList<ITrack> AllTracks => Library.Tracks;

        public LibraryFilter(Library library, TrackDanceFilter defaultFilter = null) {
            Library = library;
            Default = new FilteredTracks(this, defaultFilter ?? new TrackDanceFilter(), true);
            FilterGroups = new ReadOnlyObservableCollection<FilterGroup>(_filterGroups);
        }

        public bool Refresh() {
            if (Default.Refresh()) {
                foreach (var fi in _allFilters.All) {
                    fi.Refresh();
                }
                return true;
            }
            return false;
        }

        public FilteredTracks AddFilter(TrackDanceFilter filter) {
            if (string.IsNullOrEmpty(filter.Name)) {
                throw new ApplicationException("Filter name must be specified.");
            }

            var fi = new FilteredTracks(this, filter);

            _allFilters.Add(fi);
            if (!String.IsNullOrEmpty(fi.FilterGroup)) {
                var fg = _filterGroups.SingleOrDefault((g) => g.Name.Equals(fi.FilterGroup, StringComparison.InvariantCultureIgnoreCase));
                if (fg == null) {
                    fg = new FilterGroup(fi.FilterGroup);
                    _filterGroups.Add(fg);
                }
                fg.Add(fi);
            }
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
                            FilterGroup = category.ToString(),
                            Categories = effective
                        });
                    }
                }
            }
        }

    }
}
