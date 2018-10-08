using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using MusicLibrary.Lib;
using Xunit;

namespace MusicLibrary.Test
{
    public class JsonTest
    {
        protected void VerifyTracksMatch(ITrack expected, ITrack got) {
            Assert.Equal(expected.AlbumArtistNames, got.AlbumArtistNames);
            Assert.Equal(expected.AlbumTitle, got.AlbumTitle);
            Assert.Equal(expected.ArtistNames, got.ArtistNames);
            Assert.Equal(expected.BeatsPerMinute, got.BeatsPerMinute);
            Assert.Equal(expected.Comments, got.Comments);
            Assert.Equal(expected.Genres, got.Genres);
            Assert.Equal(expected.Grouping, got.Grouping);
            //Assert.True(expected.MediaMonkey.Equals(got.MediaMonkey));
            Assert.Equal(expected.MediaMonkey, got.MediaMonkey);
            Assert.Equal(expected.Rating, got.Rating);
            Assert.Equal(expected.Ratings, got.Ratings);
            Assert.Equal(expected.Title, got.Title);
            Assert.Equal(expected.TrackNumber, got.TrackNumber);
        }

        [Fact]
        public void ShouldRoundTrip() {
            var file = new TrackFile("./data/13 - Premium Standard - A Wink & A Smile.mp3");
            var fileJson = JsonConvert.SerializeObject((ITrack)file);
            var track = JsonConvert.DeserializeObject<Track>(fileJson);
            VerifyTracksMatch(file, track);
        }
    }
}
