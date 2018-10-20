using System.Collections.Generic;
using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    public interface ITrack
    {
        string[] AlbumArtistNames { get; }
        string AlbumTitle { get; }
        string[] ArtistNames { get; }
        uint BeatsPerMinute { get; }
        Dictionary<string, string> Comments { get; }
        string[] Genres { get; }
        string Grouping { get; }

        MediaMonkeyTags MediaMonkey { get; }
        string Path { get; }
        TrackRating Rating { get; }

        List<TrackRating> Ratings { get; }
        string Title { get; }
        uint TrackNumber { get; }

        System.DateTimeOffset LastPlayed { get; set; }

        TrackDances Dances { get; }
    }
}