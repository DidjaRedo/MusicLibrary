using System;
using Xunit;

using MusicLibrary.Lib;

namespace MusicLibrary.Test
{
    public class TrackTest
    {
        [Fact]
        public void ShouldConstructFromAnMp3File() {
            var track = new Track("./data/13 - Premium Standard - A Wink & A Smile.mp3");
            // var track = new Track("./data/03 - Domino - Business Of Love");
            Assert.Equal("A Wink & A Smile", track.Title);
            Assert.Equal(new string[] { "Premium Standard" }, track.ArtistNames);
            Assert.Equal("Quest - Foxtrot Z", track.AlbumTitle);
            Assert.Equal(new string[] { "Various Artists" }, track.AlbumArtistNames);
;        }
    }
}
