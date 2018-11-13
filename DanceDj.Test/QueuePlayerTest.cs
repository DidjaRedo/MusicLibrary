using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using DanceDj.Core;

namespace DanceDj.Test
{
    public class QueuePlayerTest
    {
        [Fact]
        public void ShouldAddTracks() {
            var player = new TestPlayer();
            var queue = new QueuePlayer(player.Player);
            Assert.Empty(queue.LastPlayed);
            Assert.Null(queue.NowPlaying);
            Assert.Empty(queue.Queue);

            var t1 = player.GetNextTrack();
            queue.Add(t1);
            Assert.Empty(queue.LastPlayed);
            Assert.Null(queue.NowPlaying);
            Assert.Collection(queue.Queue, (t) => Assert.Equal(t1, t));

            var t2 = player.GetNextTrack();
            queue.Add(t2);
            Assert.Empty(queue.LastPlayed);
            Assert.Null(queue.NowPlaying);
            Assert.Collection(queue.Queue, (t) => Assert.Equal(t1, t), (t) => Assert.Equal(t2, t));
        }

        [Fact]
        public void ShouldPlayTheQueuedTrack() {
            var player = new TestPlayer();
            var queue = new QueuePlayer(player.Player);

            var track = player.GetNextTrack();
            queue.Add(track);
            queue.Play();
            Assert.Empty(queue.LastPlayed);
            Assert.Equal(track, queue.NowPlaying);
            Assert.Empty(queue.Queue);
        }

        [Fact]
        public void ShouldGoForwardToTheNextTrack() {
            var player = new TestPlayer();
            var queue = new QueuePlayer(player.Player);

            var tracks = player.GetNextTracks(3).ToArray();
            queue.Add(tracks);
            queue.Play();
            Assert.Empty(queue.LastPlayed);
            Assert.Equal(tracks[0], queue.NowPlaying);
            Assert.Collection(queue.Queue, (t) => Assert.Equal(tracks[1], t), (t) => Assert.Equal(tracks[2], t));

            queue.GoForward();
            Assert.Collection(queue.LastPlayed, (t) => Assert.Equal(tracks[0], t));
            Assert.Equal(tracks[1], queue.NowPlaying);
            Assert.Collection(queue.Queue, (t) => Assert.Equal(tracks[2], t));
        }

        [Fact]
        public void ShouldGoBackToThePreviousTrackIfAtStart() {
            var player = new TestPlayer();
            var queue = new QueuePlayer(player.Player);

            var tracks = player.GetNextTracks(3).ToArray();
            queue.Add(tracks);
            queue.Play();
            queue.GoForward();
            queue.GoBack();
            Assert.Empty(queue.LastPlayed);
            Assert.Equal(tracks[0], queue.NowPlaying);
            Assert.Collection(queue.Queue, (t) => Assert.Equal(tracks[1], t), (t) => Assert.Equal(tracks[2], t));
        }
    }
}
