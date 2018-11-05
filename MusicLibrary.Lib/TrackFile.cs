using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using TagLib;

namespace MusicLibrary.Lib
{
    /// <summary>
    /// Represents a single track in a collection.
    /// </summary>
    /// <remarks>
    /// Mostly very thin wrapper around a TagLib Tag that pulls out common
    /// values and adds some semantics to others
    /// </remarks>
    [JsonObject(MemberSerialization.OptOut)]
    public class TrackFile : ITrack
    {
        public uint TrackNumber => Tag.Track;
        public string Title => Tag.Title;
        public string[] ArtistNames => Tag.Performers;
        public string AlbumTitle => Tag.Album;
        public string[] AlbumArtistNames => Tag.AlbumArtists;
        public uint BeatsPerMinute => Tag.BeatsPerMinute;
        public Dictionary<string, string> Comments { get; }
        public string[] Genres { get; }
        public string Grouping => Tag.Grouping;
        public string Path { get; }
        [JsonIgnore]
        public TrackRating Rating => (Ratings.Count > 0) ? Ratings[0] : null;
        public List<TrackRating> Ratings { get; }
        public int DurationInSeconds { get; }
        public DateTimeOffset? LastPlayed { get; set; }
  
        [JsonIgnore]
        public MediaMonkeyTags MediaMonkey { get; }

        [JsonIgnore]
        public TrackDances Dances { get; }

        [JsonIgnore] public TagLib.Tag Tag;

        public TrackFile(string path, string root = null) {
            var track = TagLib.File.Create(path);
            if (String.IsNullOrEmpty(root) || (!System.IO.Path.IsPathRooted(path)) || !path.StartsWith(root)) {
                Path = path;
            }
            else {
                Path = path.Remove(0, root.Length + 1);
            }
            Tag = track.Tag;
            DurationInSeconds = (int)track.Properties.Duration.TotalSeconds;

            Genres = TrimStrings(Tag.Genres);

            if (track.GetTag(TagTypes.Id3v2) is TagLib.Id3v2.Tag id3v2) {
                Comments = id3v2.GetAllComments(TagLib.Id3v2.Tag.Language);
                Ratings = id3v2.GetAllRatings();
            }
            else {
                Comments = new Dictionary<string, string>();
                Ratings = new List<TrackRating>();
            }

            MediaMonkey = new MediaMonkeyTags(this);
            Dances = new TrackDances(this);
        }

        public static TrackFile TryCreate(string path, string root = null) {
            TrackFile track = null;

            try {
                track = new TrackFile(path, root);
            }
            catch {
            };

            return track;
        }

        public override string ToString() {
            return $"{TrackNumber:D02} - {String.Join(";", ArtistNames)} - {Title}";
        }

        protected string[] TrimStrings(IEnumerable<string> strings) {
            return strings.Select<string, string>((s) => s.Trim()).ToArray();
        }
    }
}
