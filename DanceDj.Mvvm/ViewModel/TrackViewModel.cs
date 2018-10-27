using System;
using System.Collections.Generic;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class TrackViewModel : ViewModelBase
    {
        public TrackViewModel(ITrack track) {
            Track = track;
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
    }
}
