using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using MusicLibrary.Lib;

using Newtonsoft.Json;

namespace MusicLibrary.Test
{
    public class TrackDancesTest
    {
        protected class TrackDanceInferenceTest
        {
            public Track[] Tracks { get; set; }
            public TrackDanceInfo[] Results { get; set; }
        }

        [Fact]
        public void ShouldInferDanceInformationCorrectly() {
            var json = System.IO.File.ReadAllText("./data/json/TrackDanceInferenceData.json");
            var tests = JsonConvert.DeserializeObject<TrackDanceInferenceTest[]>(json);
            foreach (var test in tests) {
                var expectedJson = JsonConvert.SerializeObject(test.Results, Formatting.Indented);
                foreach (var track in test.Tracks) {
                    var gotJson = JsonConvert.SerializeObject(track.Dances.Dances, Formatting.Indented);
                    Assert.Equal(expectedJson, gotJson);
                }
            }
        }
    }
}
