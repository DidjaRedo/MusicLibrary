using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MusicLibrary.Lib
{
    public class FilteredTracks : ObservableBase
    {
        public FilteredTracks(LibraryFilter library, TrackDanceFilter filter, bool isDefault = false) {
            Library = library;
            IsDefault = isDefault;
            Filter = filter;
            Tracks.AddRange(Filter.Apply(LibraryTracks));

            Filter.PropertyChanged += FilterPropertyChangeHandler;
            Filter.Dances.CollectionChanged += (s, e) => Refresh();
            if (!IsDefault) {
                Library.DefaultFilter.PropertyChanged += FilterOrTrackChangeHandler;
            }
        }

        private void FilterPropertyChangeHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "Name":
                case "FilterGroup":
                    RaisePropertyChanged(e.PropertyName);
                    break;
                default:
                    Refresh();
                    break;
            }
        }

        private void FilterOrTrackChangeHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Refresh();
        }

        public bool Refresh() {
            var newTracks = Filter.Apply(LibraryTracks);
            if (!newTracks.SequenceEqual(Tracks)) {
                Tracks.Clear();
                Tracks.AddRange(Filter.Apply(LibraryTracks));
                RaisePropertyChanged("Tracks");
                return true;
            }
            return false;
        }

        protected LibraryFilter Library { get; }
        protected IEnumerable<ITrack> LibraryTracks => (IsDefault ? Library.AllTracks : Library.DefaultFilter.Tracks);

        public string Name { get => Filter.Name; set => Filter.Name = value; }
        public string FilterGroup { get => Filter.FilterGroup; set => Filter.FilterGroup = value; }
        public TrackDanceFilter Filter { get; }
        public bool IsDefault { get; }
        public List<ITrack> Tracks { get; } = new List<ITrack>();

        private string _description;

        public string Description {
            get => _description ?? ToString();
            set {
                if (_description != value) {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public override string ToString() {
            return Filter.ToString();
        }
    }
}
