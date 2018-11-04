using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
        private LibraryFilter Library { get; }
        public LibraryViewModel(LibraryFilter library) {
            Library = library;
            Default = new FilteredTracksViewModel(library.DefaultFilter);

            var groupVMs = library.FilterGroups.Select<FilterGroup, FilterGroupViewModel>((fi) => new FilterGroupViewModel(fi));
            FilterGroups = new ObservableCollection<FilterGroupViewModel>(groupVMs);
            SelectedFilterGroup = (FilterGroups.Count > 0 ? FilterGroups[0] : null);

            var filterVMs = library.AllFilters.Select<FilteredTracks, FilteredTracksViewModel>((fi) => new FilteredTracksViewModel(fi));
            Filters = new ObservableCollection<FilteredTracksViewModel>(filterVMs);

            foreach (var fg in FilterGroups) {
                fg.PropertyChanged += FilterGroupPropertyChanged;
            }
        }

        private void FilterGroupPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "SelectedTrack") {
                if (Object.ReferenceEquals(sender, _selectedFilterGroup)) {
                    RaisePropertyChanged("SelectedTrack");
                }
                else if ((sender is FilterGroupViewModel) && FilterGroups.Contains(sender)) {
                    SelectedFilterGroup = (FilterGroupViewModel)sender;
                }
            }
        }

        public FilteredTracksViewModel Default { get; }
        public ObservableCollection<FilterGroupViewModel> FilterGroups { get; }
        public ObservableCollection<FilteredTracksViewModel> Filters { get; }

        protected FilterGroupViewModel _selectedFilterGroup;
        public FilterGroupViewModel SelectedFilterGroup {
            get => _selectedFilterGroup;
            set {
                if (Set("SelectedFilterGroup", ref _selectedFilterGroup, value)) {
                    RaisePropertyChanged("SelectedTrack");
                }
            }
        }

        public TrackViewModel SelectedTrack => SelectedFilterGroup?.SelectedTrack;
    }
}
