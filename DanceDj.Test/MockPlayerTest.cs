using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using MusicLibrary.Lib;
using DanceDj.Core;

namespace DanceDj.Test
{
    public class MockPlayerTest
    {
        [Fact]
        public void ShouldUpdateForPlay() {
            var test = new TestPlayer();
            var player = test.Player;

            Assert.Equal(PlayerState.Stopped, player.PlayerState);
            Assert.Null(player.NowPlaying);

            player.Play(test.TestTrack);

            var expected = new List<String>(new string[] { "NowPlaying", "PlayerTimes", "PlayerState" });
            Assert.Equal(expected, test.Changes);
            Assert.Equal(test.TestTrack, player.NowPlaying);
            Assert.Equal(PlayerState.Playing, player.PlayerState);
            Assert.Equal(test.TestTrack.DurationInSeconds, player.PlayerTimes.TotalTimeInSeconds);
            Assert.Equal(test.TestTrack.DurationInSeconds, player.PlayerTimes.RemainingTimeInSeconds);
            Assert.Equal(0, player.PlayerTimes.ElapsedTimeInSeconds);
            Assert.Equal(player.FadeDuration, player.PlayerTimes.FadeOutTimeInSeconds);
            Assert.Equal(1.0, player.PlayerTimes.FaderVolume);
        }

        [Fact]
        public void ShouldUpdateOnTick() {
            var test = new TestPlayer();
            var player = test.Player;

            Assert.Equal(PlayerState.Stopped, player.PlayerState);
            Assert.Null(player.NowPlaying);

            player.Play(test.TestTrack);
            test.Changes.Clear();
            test.Timer.Tick();

            var expected = new List<String>(new string[] { "PlayerTimes" });
            Assert.Equal(expected, test.Changes);
            Assert.Equal(test.TestTrack, player.NowPlaying);
            Assert.Equal(PlayerState.Playing, player.PlayerState);
            Assert.Equal(test.TestTrack.DurationInSeconds, player.PlayerTimes.TotalTimeInSeconds);
            Assert.Equal(test.TestTrack.DurationInSeconds - 1, player.PlayerTimes.RemainingTimeInSeconds);
            Assert.Equal(1, player.PlayerTimes.ElapsedTimeInSeconds);
            Assert.Equal(player.FadeDuration, player.PlayerTimes.FadeOutTimeInSeconds);
            Assert.Equal(1.0, player.PlayerTimes.FaderVolume);
        }
    }
}
