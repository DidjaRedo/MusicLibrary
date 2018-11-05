using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;
using DanceDj.Core;

using Xunit;

namespace DanceDj.Test
{
    public class TestPlayer
    {
        public TestPlayer() {
            Library = SampleData.GetTestLibrary();
            TestTrack = Library.Tracks[0];
            Timer = new DanceDj.Utils.MockTimer(1000) { AutoReset = true, Enabled = true };
            Player = new MockPlayer() { Timer = Timer };
            Changes = new List<string>();

            Player.PropertyChanged += (s, e) => {
                Changes.Add(e.PropertyName);
            };
        }

        public Library Library { get; protected set; }
        public ITrack TestTrack { get; set; }
        public DanceDj.Utils.MockTimer Timer { get; protected set; }
        public MockPlayer Player { get; protected set; }
        public List<string> Changes { get; protected set; }

        public void AssertChanges(params string[] expected) {
            Assert.Equal(expected, Changes);
        }

        public void AssertIsAsExpectedWhilePlaying(int expectedElapsed, params string[] expectedChanges) {
            Assert.Equal(expectedChanges, Changes);
            Assert.Equal(TestTrack, Player.NowPlaying);
            Assert.Equal(PlayerState.Playing, Player.PlayerState);
            Assert.Equal(TestTrack.DurationInSeconds, Player.PlayerTimes.TotalTimeInSeconds);
            Assert.Equal(TestTrack.DurationInSeconds - expectedElapsed, Player.PlayerTimes.RemainingTimeInSeconds);
            Assert.Equal(expectedElapsed, Player.PlayerTimes.ElapsedTimeInSeconds);
            Assert.Equal(Player.FadeDuration, Player.PlayerTimes.FadeOutTimeInSeconds);
            if (Player.PlayerTimes.RemainingTimeInSeconds < Player.FadeDuration) {
                Assert.True(Player.PlayerTimes.FaderVolume < 1.0);
            }
            else {
                Assert.Equal(1.0, Player.PlayerTimes.FaderVolume);
            }
        }
    }
}
