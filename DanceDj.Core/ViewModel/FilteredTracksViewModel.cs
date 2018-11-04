using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.ViewModel
{
    public class FilteredTracksViewModel : ViewModelBase
    {
        public FilteredTracksViewModel(FilteredTracks fi) {
            FilterInfo = fi;
            Filter = new FilterViewModel(fi.Filter);
            var trackVMs = fi.Tracks.Select<ITrack, TrackViewModel>((t) => TrackViewModel.GetOrAdd(t));
            InnerTracks = new ObservableCollection<TrackViewModel>(trackVMs);
            Tracks = new ReadOnlyObservableCollection<TrackViewModel>(InnerTracks);
            SelectedTrack = (InnerTracks.Count > 0 ? InnerTracks[0] : null);

            fi.PropertyChanged += Fi_PropertyChanged;
        }

        private void Fi_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RefreshTracks();
        }

        private void RefreshTracks() {
            var count = InnerTracks.Count;
            var trackVMs = FilterInfo.Tracks.Select<ITrack, TrackViewModel>((t) => TrackViewModel.GetOrAdd(t));
            InnerTracks = new ObservableCollection<TrackViewModel>(trackVMs);
            Tracks = new ReadOnlyObservableCollection<TrackViewModel>(InnerTracks);
            RaisePropertyChanged("Tracks");

            if ((_selectedTrack == null) || (!InnerTracks.Contains(_selectedTrack))) {
                SelectedTrack = (InnerTracks.Count > 0 ? InnerTracks[0] : null);
            }

            if (InnerTracks.Count != count) {
                RaisePropertyChanged("NameAndCount");
            }
        }

        internal FilteredTracks FilterInfo { get; }

        public string Name => Filter.Name;
        public string NameAndCount => $"{Name} ({Tracks.Count})";
 
        public FilterViewModel Filter { get; }

        private ObservableCollection<TrackViewModel> InnerTracks { get; set;  }
        public ReadOnlyObservableCollection<TrackViewModel> Tracks { get; private set; }

        protected TrackViewModel _selectedTrack;
        public TrackViewModel SelectedTrack { get => _selectedTrack; set => Set("SelectedTrack", ref _selectedTrack, value); }
    }
}
