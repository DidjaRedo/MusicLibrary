using System.Collections.Generic;

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
        TrackRating Rating { get; }
        List<TrackRating> Ratings { get; }
        string Title { get; }
        uint TrackNumber { get; }
    }
}