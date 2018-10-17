using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    class DanceInfoTagParser
    {
        public bool IsBallroom { get; protected set; }
        public DanceCategories Categories { get; protected set; }
        public IReadOnlyList<Dance> Dances { get; }
        public IReadOnlyList<string> UnusedTags { get; }

        public Dictionary<DanceCategory, uint> Ratings { get; }

        public uint? GetEffectiveRating(DanceCategory cat, uint? defaultRating) {
            uint rating;
            return Ratings.TryGetValue(cat, out rating) ? rating : defaultRating;
        }

        protected uint? DefaultRating { get; }
        protected DanceCategories DefaultCategories;
        protected Dictionary<uint?, DanceCategories> CategoriesByRating = new Dictionary<uint?, DanceCategories>();
        protected Dictionary<Dance, uint?> RatingsByDance = new Dictionary<Dance, uint?>();

        protected bool TryGetCategoryMask(string tag, out DanceCategories mask) {
            return Enum.TryParse<DanceCategories>(tag, true, out mask);
        }

        protected bool TryGetDance(string tag, out Dance dance) {
            dance = MusicLibrary.Lib.Dances.ByName[tag];
            return dance != null;
        }

        protected string ExtractRating(string tag, out uint? rating) {
            rating = null;
            if (tag.Contains('@')) {
                var split = tag.Split('@');
                if (split.Length == 2) {
                    double val;
                    if (double.TryParse(split[1], out val) && (val >= 0.0) && (val <= 5.0)) {
                        rating = TrackRating.FiveStarRatingToRaw(val);
                        return split[0];
                    }
                }
                throw new ApplicationException($"Malformed category tag \"{tag}\".");
            }
            return tag;
        }

        protected void AddOrUpdateRatingForDance(Dance dance, uint? rating) {
            uint? existing;
            if (RatingsByDance.TryGetValue(dance, out existing) && existing.HasValue) {
                if (rating.HasValue) {
                    if (existing.Value != rating.Value) {
                        throw new ApplicationException($"Conflicting ratings ({existing.Value} and {rating.Value}) for {dance}");
                    }
                }
            }
            RatingsByDance[dance] = rating;
        }

        protected void AddOrUpdateCategories(DanceCategories categories, uint? rating) {
            DanceCategories existing;
            if (CategoriesByRating.TryGetValue(rating, out existing)) {
                var newCategories = categories | existing;
                CategoriesByRating[rating] = newCategories;
            }
            else {
                CategoriesByRating[rating] = categories;
            }
        }

        protected void UpdateAllCategories() {
            var all = DanceCategories.None;

            foreach (var kvp in CategoriesByRating) {
                if (kvp.Key.HasValue) {
                    if ((all & kvp.Value) != 0) {
                        throw new ApplicationException($"Multiple ratings for {kvp.Value}");
                    }
                    all |= kvp.Value;
                }
            }

            DanceCategories unrated;
            if (CategoriesByRating.TryGetValue(null, out unrated)) {
                unrated &= ~all;
                if (unrated == DanceCategories.None) {
                    CategoriesByRating.Remove(null);
                }
                else {
                    CategoriesByRating[null] = unrated;
                    all |= unrated;
                }
            }

            Categories = all;
        }

        protected string ExtractTag(string tag) {
            uint? rating;
            tag = ExtractRating(tag, out rating);

            DanceCategories categories;
            Dance dance;
            if (TryGetCategoryMask(tag, out categories)) {
                AddOrUpdateCategories(categories, rating);
                return String.Empty;
            }
            else if (TryGetDance(tag, out dance)) {
                AddOrUpdateRatingForDance(dance, rating);
                return String.Empty;
            }
            else if (tag.Equals("ballroom", StringComparison.InvariantCultureIgnoreCase)) {
                IsBallroom = true;
                return String.Empty;
            }
            return tag;
        }

        public DanceInfoTagParser(uint? defaultRating, DanceCategories defaultCategories, IEnumerable<string> tags) {
            DefaultRating = defaultRating;
            DefaultCategories = defaultCategories;

            UnusedTags = tags.Normalize().Select<string, string>(ExtractTag).Where((s) => !String.IsNullOrEmpty(s)).ToList().AsReadOnly();

            UpdateAllCategories();
            Dances = RatingsByDance.Keys.ToList();
        }
    }
}

