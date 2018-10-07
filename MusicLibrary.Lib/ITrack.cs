using System.Collections.Generic;
using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    [JsonObject(MemberSerialization.OptOut)]
    public interface ITrack
    {
        string[] AlbumArtistNames { get; }
        string AlbumTitle { get; }
        string[] ArtistNames { get; }
        uint BeatsPerMinute { get; }
        Dictionary<string, string> Comments { get; }
        string[] Genres { get; }
        string Grouping { get; }

        [JsonIgnore]
        MediaMonkeyTags MediaMonkey { get; }

        [JsonIgnore]
        TrackRating Rating { get; }

        List<TrackRating> Ratings { get; }
        string Title { get; }
        uint TrackNumber { get; }
    }
}