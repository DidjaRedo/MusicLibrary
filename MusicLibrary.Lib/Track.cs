using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    public class Track : ITrack {
        public uint TrackNumber { get; }
        public string Title { get; }
        public string[] ArtistNames { get; }
        public string AlbumTitle { get; }

        public string[] AlbumArtistNames { get;  }
        public uint BeatsPerMinute { get; }
        public Dictionary<string, string> Comments { get; }
        public string[] Genres { get; }
        public string Grouping { get; }

        public string Path { get; }
        public List<TrackRating> Ratings { get; }

        [JsonConstructor]
        public Track(string path, uint trackNumber, string title, string[] artistNames, string[] albumArtistNames, string albumTitle, uint beatsPerMinute, Dictionary<string, string> comments, string[] genres, string grouping, List<TrackRating> ratings) {
            TrackNumber = trackNumber;
            Title = title;
            ArtistNames = artistNames;
            AlbumTitle = albumTitle;
            AlbumArtistNames = albumArtistNames;
            BeatsPerMinute = beatsPerMinute;
            Comments = comments ?? new Dictionary<string, string>();
            Genres = genres;
            Grouping = grouping;
            Path = path;
            Ratings = ratings;
            MediaMonkey = new MediaMonkeyTags(this);
            Dances = new TrackDances(this);
        }

        [JsonIgnore]
        public TrackRating Rating => (Ratings.Count > 0) ? Ratings[0] : null;

        [JsonIgnore]
        public MediaMonkeyTags MediaMonkey { get; }

        [JsonIgnore]
        public TrackDances Dances { get; }

        public override string ToString() {
            return $"{TrackNumber:D02} - {String.Join(";", ArtistNames)} - {Title}";
        }

        public static Track FromJsonText(string json) {
            return JsonConvert.DeserializeObject<Track>(json);
        }

        public static Track FromJsonFile(string path) {
            var json = System.IO.File.ReadAllText(path);
            return FromJsonText(json);
        }
    }
}
