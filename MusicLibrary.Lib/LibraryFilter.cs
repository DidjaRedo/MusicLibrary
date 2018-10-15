using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    public class LibraryFilter
    {
        public Library Library { get; }
        public TrackDanceFilter Default { get; }

        protected Dictionary<string, FilterInfo> _Filters = new Dictionary<string, FilterInfo>();
        public IReadOnlyDictionary<string, FilterInfo> Filters;

        public LibraryFilter(Library library, TrackDanceFilter defaultFilter = null) {
            Library = library;
            Default = defaultFilter ?? new TrackDanceFilter();
            Filters = new ReadOnlyDictionary<string, FilterInfo>(_Filters);
        }

        public class FilterInfo
        {
            public TrackDanceFilter Configured { get; set; }
            public TrackDanceFilter Effective { get; set; }
            public List<ITrack> Tracks { get; } = new List<ITrack>();
        }
    }
}
