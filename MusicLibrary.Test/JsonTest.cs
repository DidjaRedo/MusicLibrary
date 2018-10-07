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
        [Fact]
        public void ShouldRoundTrip() {
            var file = new TrackFile("./data/13 - Premium Standard - A Wink & A Smile.mp3");
            var fileJson = JsonConvert.SerializeObject(file);
            var track = JsonConvert.DeserializeObject<Track>(fileJson);
            Assert.Equal(file.AlbumTitle, track.AlbumTitle);
        }
    }
}
