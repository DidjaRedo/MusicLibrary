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

            test.AssertIsAsExpectedWhilePlaying(0, "NowPlaying", "PlayerTimes", "PlayerState");
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
            test.AssertIsAsExpectedWhilePlaying(1, "PlayerTimes");
        }

        [Fact]
        public void ShouldPlayEntireSongAndFadeToStop() {
            var test = new TestPlayer();
            var player = test.Player;

            Assert.Equal(PlayerState.Stopped, player.PlayerState);
            Assert.Null(player.NowPlaying);

            player.Play(test.TestTrack);

            var normalChanges = new string[] { "PlayerTimes" };
            var faderChanges = new string[] { "PlayerTimes", "EffectiveVolume" };
            var fadePoint = (test.TestTrack.DurationInSeconds - player.FadeDuration);
            double lastVolume = 1.0;
            for (int i = 0; i < test.TestTrack.DurationInSeconds-1; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                var expectedElapsed = i + 1;
                var fading = (i >= fadePoint);
                var expectedChanges = (fading ? faderChanges : normalChanges);

                test.AssertIsAsExpectedWhilePlaying(expectedElapsed, expectedChanges);

                if (fading) {
                    Assert.True(player.PlayerTimes.FaderVolume < lastVolume);
                }
                else {
                    Assert.Equal(1.0, player.PlayerTimes.FaderVolume);
                }
                lastVolume = player.PlayerTimes.FaderVolume;
            }

            test.Changes.Clear();
            test.Timer.Tick();
            Assert.Equal(new List<string>(new string[] { "PlayerTimes", "PlayerState", "EffectiveVolume" }), test.Changes);
            Assert.Equal(PlayerState.Stopped, player.PlayerState);
        }
    }
}
