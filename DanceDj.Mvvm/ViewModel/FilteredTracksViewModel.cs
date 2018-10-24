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
            Filter = new FilterViewModel(fi.Effective);
            var trackVMs = fi.Tracks.Select<ITrack, TrackViewModel>((t) => new TrackViewModel(t));
            InnerTracks = new ObservableCollection<TrackViewModel>(trackVMs);
            Tracks = new ReadOnlyObservableCollection<TrackViewModel>(InnerTracks);
        }

        internal LibraryFilter.FilterInfo FilterInfo { get; }

        public FilterViewModel Filter { get; }

        private ObservableCollection<TrackViewModel> InnerTracks { get; }
        public ReadOnlyObservableCollection<TrackViewModel> Tracks { get; }
    }
}
