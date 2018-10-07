using System;
using System.Collections.Generic;

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
    public class TrackFile : ITrack
    {
        public uint TrackNumber => Tag.Track;
        public string Title => Tag.Title;
        public string[] ArtistNames => Tag.Performers;
        public string AlbumTitle => Tag.Album;
        public string[] AlbumArtistNames => Tag.AlbumArtists;
        public uint BeatsPerMinute => Tag.BeatsPerMinute;
        public Dictionary<string, string> Comments { get; }
        public string[] Genres => Tag.Genres;
        public string Grouping => Tag.Grouping;
        public TrackRating Rating => (Ratings.Count > 0) ? Ratings[0] : null;
        public List<TrackRating> Ratings { get; }
  
        public MediaMonkeyTags MediaMonkey { get; }
        public TrackDances Dances { get; }

        public TagLib.Tag Tag;

        public TrackFile(string path) {
            var track = TagLib.File.Create(path);
            Tag = track.Tag;

            var id3v2 = track.GetTag(TagTypes.Id3v2) as TagLib.Id3v2.Tag;
            if (id3v2 != null) {
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

        public override string ToString() {
            return $"{TrackNumber:D02} - {String.Join(";", ArtistNames)} - {Title}";
        }
    }
}
