using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MusicLibrary.Lib
{
    public class FilterGroup
    {
        public FilterGroup(string name, bool isDefault) {
            Name = name;
            IsDefault = isDefault;
            All = new ReadOnlyObservableCollection<FilteredTracks>(_all);
        }

        public FilterGroup(string name) : this(name, false) { }

        public string Name { get; }
        public bool IsDefault { get; }

        protected ObservableCollection<FilteredTracks> _all = new ObservableCollection<FilteredTracks>();
        public ReadOnlyObservableCollection<FilteredTracks> All;

        public FilteredTracks this[string name] {
            get => All.Single((f) => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Add(FilteredTracks filter) {
            if (All.Contains(filter)) {
                return;
            }
            if ((!IsDefault) && (!String.IsNullOrEmpty(filter.FilterGroup))) {
                if (!filter.FilterGroup.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) {
                    throw new ApplicationException($"Filter {filter.Name} belongs to filter group {filter.FilterGroup} - cannot add to {Name}");
                }
            }
            if (All.Any((f) => f.Name.Equals(filter.Name, StringComparison.InvariantCultureIgnoreCase))) {
                throw new ApplicationException($"Filter group {Name} already contains a filter named {filter.Name}.");
            }

            _all.Add(filter);

            if (!IsDefault) {
                if (String.IsNullOrEmpty(filter.FilterGroup)) {
                    filter.FilterGroup = Name;
                }
                filter.PropertyChanged += FilterPropertyChangeHandler;
            }
        }

        public void AddRange(params FilteredTracks[] filters) {
            foreach (var filter in filters) {
                Add(filter);
            }
        }

        public bool Remove(FilteredTracks filter) {
            if (_all.Remove(filter)) {
                filter.PropertyChanged -= FilterPropertyChangeHandler;
                return true;
            }
            return false;
        }

        public void RemoveRange(params FilteredTracks[] filters) {
            foreach (var filter in filters) {
                Remove(filter);
            }
        }

        private void FilterPropertyChangeHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "FilterGroup") {
                var filter = (FilteredTracks)sender;
                if (!filter.FilterGroup.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) {
                    Remove(filter);
                }
            }
        }
    }
}
