using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using MusicLibrary.Lib;
using GalaSoft.MvvmLight;

namespace DanceDj.ViewModel
{
    public class FilterGroupViewModel : ViewModelBase
    {
        public FilterGroupViewModel(FilterGroup group) {
            Group = group;

            var filterVMs = group.All.Select<FilteredTracks, FilteredTracksViewModel>((fi) => new FilteredTracksViewModel(fi));
            Filters = new ObservableCollection<FilteredTracksViewModel>(filterVMs);
            SelectedFilter = (Filters.Count > 0 ? Filters[0] : null);
            foreach (var f in Filters) {
                f.PropertyChanged += FilterPropertyChanged;
            }
        }

        private void FilterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "SelectedTrack") {
                if (Object.ReferenceEquals(sender, _selectedFilter)) {
                    RaisePropertyChanged("SelectedTrack");
                }
                else if ((sender is FilteredTracksViewModel) && Filters.Contains(sender)) {
                    SelectedFilter = (FilteredTracksViewModel)sender;
                }
            }
        }

        protected FilterGroup Group { get; }

        public string Name => Group.Name;
        public ObservableCollection<FilteredTracksViewModel> Filters { get; }

        protected FilteredTracksViewModel _selectedFilter;
        public FilteredTracksViewModel SelectedFilter {
            get => _selectedFilter;
            set {
                if (Set("SelectedFilter", ref _selectedFilter, value)) {
                    RaisePropertyChanged("SelectedTrack");
                }
            }
        }

        public TrackViewModel SelectedTrack => SelectedFilter?.SelectedTrack;
    }
}
