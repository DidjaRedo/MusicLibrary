using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class FilteredTracksViewModel : ViewModelBase
    {
        public FilteredTracksViewModel(LibraryFilter.FilterInfo fi) {
            FilterInfo = fi;
            Filter = new FilterViewModel(fi.Filter);
            var trackVMs = fi.Tracks.Select<ITrack, TrackViewModel>((t) => TrackViewModel.GetOrAdd(t));
            InnerTracks = new ObservableCollection<TrackViewModel>(trackVMs);
            Tracks = new ReadOnlyObservableCollection<TrackViewModel>(InnerTracks);

            Filter.PropertyChanged += FilterPropertyChangedHandler;
        }

        private void FilterPropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            FilterInfo.Refresh();
            RefreshTracks();
        }

        private void RefreshTracks() {
            var trackVMs = FilterInfo.Tracks.Select<ITrack, TrackViewModel>((t) => TrackViewModel.GetOrAdd(t));
            InnerTracks = new ObservableCollection<TrackViewModel>(trackVMs);
            Tracks = new ReadOnlyObservableCollection<TrackViewModel>(InnerTracks);
            RaisePropertyChanged("Tracks");
        }

        internal LibraryFilter.FilterInfo FilterInfo { get; }

        public string Name => Filter.Name;
        public string NameAndCount => $"{Name} ({Tracks.Count})";
 
        public FilterViewModel Filter { get; }

        private ObservableCollection<TrackViewModel> InnerTracks { get; set;  }
        public ReadOnlyObservableCollection<TrackViewModel> Tracks { get; private set; }
    }
}
