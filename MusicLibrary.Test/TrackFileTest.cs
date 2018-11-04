using System;
using Xunit;

using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class TrackFileTest
    {
        [Fact]
        public void ShouldConstructFromAnMp3File() {
            var track = new TrackFile("./data/mp3/music/Various Artists/Quest - Foxtrot Z/13 - Premium Standard - A Wink & A Smile.mp3");
            Assert.Equal("A Wink & A Smile", track.Title);
            Assert.Equal(new string[] { "Premium Standard" }, track.ArtistNames);
            Assert.Equal("Quest - Foxtrot Z", track.AlbumTitle);
            Assert.Equal(new string[] { "Various Artists" }, track.AlbumArtistNames);
            Assert.Equal(7, track.Comments.Count);
            Assert.Equal("NoAuto", track.MediaMonkey.Custom1);
            Assert.Equal("FOX", track.MediaMonkey.Custom2);
            Assert.Equal("Quest", track.MediaMonkey.Custom3);
            Assert.Equal("Social", track.MediaMonkey.Custom4);
            Assert.Equal("Any", track.MediaMonkey.Custom5);
            Assert.Equal("Seasonal", track.MediaMonkey.Occasion);
;        }
    }
}
