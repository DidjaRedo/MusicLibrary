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

            test.AssertIsAsExpected(PlayerState.Playing, 0, "NowPlaying", "PlayerTimes", "IsAtStart", "PlayerState");
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
            test.AssertIsAsExpected(PlayerState.Playing, 1, "PlayerTimes", "IsAtStart");
        }

        [Fact]
        public void ShouldPlayEntireSongAndFadeToStop() {
            var test = new TestPlayer();
            var player = test.Player;

            Assert.Equal(PlayerState.Stopped, player.PlayerState);
            Assert.Null(player.NowPlaying);

            player.Play(test.TestTrack);

            var firstTickChanges = new string[] { "PlayerTimes", "IsAtStart" };
            var normalChanges = new string[] { "PlayerTimes" };
            var faderChanges = new string[] { "PlayerTimes", "EffectiveVolume" };
            var fadePoint = (test.TestTrack.DurationInSeconds - player.FadeDuration);

            double lastVolume = 1.0;
            for (int i = 0; i < test.TestTrack.DurationInSeconds-1; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                var expectedElapsed = i + 1;
                var fading = (i >= fadePoint);
                var expectedChanges = ((i == 0) ? firstTickChanges : (fading ? faderChanges : normalChanges));

                test.AssertIsAsExpected(PlayerState.Playing, expectedElapsed, expectedChanges);

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
            Assert.Equal(new List<string>(new string[] { "PlayerTimes", "IsAtEnd", "PlayerState", "EffectiveVolume" }), test.Changes);
            Assert.Equal(PlayerState.Stopped, player.PlayerState);
        }

        [Fact]
        public void ShouldResumeAfterPause() {
            var test = new TestPlayer();
            var player = test.Player;
            player.Play(test.TestTrack);

            int pauseAt = test.TestTrack.DurationInSeconds / 2;

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertIsAsExpected(PlayerState.Playing, 1, "PlayerTimes", "IsAtStart");

            for (int i = 1; i < pauseAt; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                test.AssertIsAsExpected(PlayerState.Playing, i + 1, "PlayerTimes");
            }

            test.Changes.Clear();
            player.Pause();
            test.AssertIsAsExpected(PlayerState.Paused, pauseAt, "PlayerState");

            test.Changes.Clear();
            player.Play();
            test.AssertIsAsExpected(PlayerState.Playing, pauseAt, "PlayerState");

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertIsAsExpected(PlayerState.Playing, pauseAt + 1, "PlayerTimes");
        }

        [Fact]
        public void ShouldResetAfterStop() {
            var test = new TestPlayer();
            var player = test.Player;
            player.Play(test.TestTrack);

            int stopAt = test.TestTrack.DurationInSeconds / 2;

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertIsAsExpected(PlayerState.Playing, 1, "PlayerTimes", "IsAtStart");

            for (int i = 1; i < stopAt; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                test.AssertIsAsExpected(PlayerState.Playing, i + 1, "PlayerTimes");
            }

            test.Changes.Clear();
            player.Stop();
            test.AssertIsAsExpected(PlayerState.Stopped, 0, "PlayerTimes", "IsAtStart", "PlayerState");

            test.Changes.Clear();
            player.Play();
            test.AssertIsAsExpected(PlayerState.Playing, 0, "PlayerState");

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertIsAsExpected(PlayerState.Playing, 1, "PlayerTimes", "IsAtStart");
        }

        [Fact]
        public void ShouldFadeToPause() {
            var test = new TestPlayer();
            var player = test.Player;
            player.Play(test.TestTrack);

            int fadeAt = test.TestTrack.DurationInSeconds / 2;

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertIsAsExpected(PlayerState.Playing, 1, "PlayerTimes", "IsAtStart");

            for (int i = 1; i < fadeAt; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                test.AssertIsAsExpected(PlayerState.Playing, i + 1, "PlayerTimes");
            }

            test.Changes.Clear();
            player.StartFade();
            test.AssertChanges("PlayerTimes");

            var lastVolume = 1.0;
            for (int i = 0; i < player.FadeDuration - 1; i++) {
                test.Changes.Clear();
                test.Timer.Tick();
                test.AssertChanges("PlayerTimes", "EffectiveVolume");
                Assert.True(player.EffectiveVolume < lastVolume);
                lastVolume = player.EffectiveVolume;
            }

            test.Changes.Clear();
            test.Timer.Tick();
            test.AssertChanges("PlayerTimes", "PlayerState", "EffectiveVolume");
            Assert.Equal(PlayerState.Paused, test.Player.PlayerState);
        }
    }
}
