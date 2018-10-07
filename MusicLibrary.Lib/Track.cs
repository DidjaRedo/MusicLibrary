using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    public class Track : ITrack {
        public string[] AlbumArtistNames { get;  }
        public string AlbumTitle { get; }
        public string[] ArtistNames { get;}
        public uint BeatsPerMinute { get; }
        public Dictionary<string, string> Comments { get; }
        public string[] Genres { get; }
        public string Grouping { get; }

        public List<TrackRating> Ratings { get; }

        public string Title { get; }

        public uint TrackNumber { get; }

        [JsonConstructor]
        public Track(uint trackNumber, string title, string[] artistNames, string[] albumArtistNames, string albumTitle, uint beatsPerMinute, Dictionary<string, string> comments, string[] genres, string grouping, List<TrackRating> ratings) {
            AlbumArtistNames = albumArtistNames;
            AlbumTitle = albumTitle;
            ArtistNames = artistNames;
            BeatsPerMinute = beatsPerMinute;
            Comments = comments;
            Genres = genres;
            Grouping = grouping;
            Ratings = ratings;
            Title = title;
            TrackNumber = trackNumber;
            MediaMonkey = new MediaMonkeyTags(this);
        }

        public TrackRating Rating => (Ratings.Count > 0) ? Ratings[0] : null;
        public MediaMonkeyTags MediaMonkey { get; }
    }
}
