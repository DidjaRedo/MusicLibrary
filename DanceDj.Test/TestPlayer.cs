using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;
using DanceDj.Core;

namespace DanceDj.Test
{
    public class TestPlayer
    {
        public TestPlayer() {
            Library = SampleData.GetTestLibrary();
            TestTrack = Library.Tracks[0];
            Timer = new DanceDj.Utils.MockTimer(1000);
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
    }
}
