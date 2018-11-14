using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.ViewModel
{
    public class TrackViewModel : ViewModelBase
    {
        static TrackViewModel() {
            AllTracks = new ReadOnlyDictionary<ITrack, TrackViewModel>(_allTracks);
        }

        protected TrackViewModel(ITrack track) {
            Track = track;
        }

        public static TrackViewModel GetOrAdd(ITrack track) {
            if (!_allTracks.ContainsKey(track)) {
                _allTracks[track] = new TrackViewModel(track);
            }
            return _allTracks[track];
        }

        internal ITrack Track { get; }

        public uint TrackNumber => Track.TrackNumber;
        public string Title => Track.Title;
        public string[] Artists => Track.ArtistNames;
        public string Album => Track.AlbumTitle;
        public string[] AlbumArtists => Track.AlbumArtistNames;

        public override string ToString() {
            return Track.ToString();
        }

        private static Dictionary<ITrack, TrackViewModel> _allTracks = new Dictionary<ITrack, TrackViewModel>();
        public static IReadOnlyDictionary<ITrack,TrackViewModel> AllTracks { get; private set; }

    }

    public static class TrackViewModelExtensions
    {
        public static IEnumerable<TrackViewModel> ToViewModel(this IEnumerable<ITrack> tracks) {
            return tracks.Select<ITrack, TrackViewModel>((t) => TrackViewModel.GetOrAdd(t));
        }
    }
}
