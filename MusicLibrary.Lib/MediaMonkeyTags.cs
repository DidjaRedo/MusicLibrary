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

        protected string GetCommentOrDefault(string key) {
            string value = null;
            return _track.Comments.TryGetValue(key, out value) ? value : null;
        }

        public string Occasion => GetCommentOrDefault("Songs-DB_Occasion");
        public string Custom1 => GetCommentOrDefault("Songs-DB_Custom1");
        public string Custom2 => GetCommentOrDefault("Songs-DB_Custom2");
        public string Custom3 => GetCommentOrDefault("Songs-DB_Custom3");
        public string Custom4 => GetCommentOrDefault("Songs-DB_Custom4");
        public string Custom5 => GetCommentOrDefault("Songs-DB_Custom5");

        public override bool Equals(object obj) {
            var other = obj as MediaMonkeyTags;
            return (other != null) && (GetHashCode() == other.GetHashCode());
        }

        public override int GetHashCode() {
            var hash = Occasion.GetHashCode();
            hash ^= Custom1?.GetHashCode() ?? 0;
            hash ^= Custom2?.GetHashCode() ?? 0;
            hash ^= Custom3?.GetHashCode() ?? 0;
            hash ^= Custom4?.GetHashCode() ?? 0;
            hash ^= Custom5?.GetHashCode() ?? 0;
            return hash;
        }
    }
}
