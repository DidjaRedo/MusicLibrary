using System;
using System.Collections.Generic;
using System.Text;

namespace MusicLibrary.Lib
{
    public class MediaMonkeyTags
    {
        private ITrack _track;
        public MediaMonkeyTags(ITrack track) {
            _track = track;
        }

        public string Occasion => _track.Comments["Songs-DB_Occasion"];
        public string Custom1 => _track.Comments["Songs-DB_Custom1"];
        public string Custom2 => _track.Comments["Songs-DB_Custom2"];
        public string Custom3 => _track.Comments["Songs-DB_Custom3"];
        public string Custom4 => _track.Comments["Songs-DB_Custom4"];
        public string Custom5 => _track.Comments["Songs-DB_Custom5"];

        public override bool Equals(object obj) {
            var other = obj as MediaMonkeyTags;
            return (other != null) && (GetHashCode() == other.GetHashCode());
        }

        public override int GetHashCode() {
            var hash = Occasion.GetHashCode();
            hash ^= Custom1.GetHashCode();
            hash ^= Custom2.GetHashCode();
            hash ^= Custom3.GetHashCode();
            hash ^= Custom4.GetHashCode();
            hash ^= Custom5.GetHashCode();
            return hash;
        }
    }
}
