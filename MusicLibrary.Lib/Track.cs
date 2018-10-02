using System;
using System.Collections.Generic;

using TagLib;

namespace MusicLibrary.Lib
{
    /// <summary>
    /// Represents a single track in a collection.
    /// </summary>
    /// <remarks>
    /// Mostly very thin wrapper around a TagLib Tag
    /// </remarks>
    public class Track
    {
        public uint TrackNumber => Tag.Track;
        public string Title => Tag.Title;
        public string[] ArtistNames => Tag.Performers;
        public string AlbumTitle => Tag.Album;
        public string[] AlbumArtistNames => Tag.AlbumArtists;
        public uint BeatsPerMinute => Tag.BeatsPerMinute;

        public Dictionary<string,string> Comments { get; }

        public TagLib.Tag Tag;

        public Track(string path) {
            var track = TagLib.File.Create(path);
            Tag = track.Tag;

            var id3v2 = track.GetTag(TagTypes.Id3v2) as TagLib.Id3v2.Tag;
            if (id3v2 != null) {
                Comments = id3v2.Comments;
            }
            else {
                Comments = new Dictionary<string, string>();
            }
        }
    }
}
