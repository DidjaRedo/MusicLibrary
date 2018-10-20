using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Linq;

namespace MusicLibrary.Lib
{
    public class DanceInfoTagParser
    {
        public bool IsBallroom { get; protected set; }
        public DanceCategories Categories { get; protected set; }
        public IReadOnlyList<Dance> Dances { get; }
        public IReadOnlyList<string> UnusedTags { get; }

        protected uint? DefaultRating { get; }
        protected DanceCategories DefaultCategories { get; }

        protected DanceCategories UnratedCategories { get; set; } = DanceCategories.None;
        protected Dictionary<uint, DanceCategories> CategoriesByRating = new Dictionary<uint, DanceCategories>();
        protected Dictionary<Dance, uint?> RatingsByDance = new Dictionary<Dance, uint?>();

        public IEnumerable<TrackDanceInfo> GetTagDanceInfo() {
            var tdis = new List<TrackDanceInfo>();

            foreach (var kvp in CategoriesByRating) {
                if (kvp.Value != DanceCategories.None) {
                    foreach (var dance in Dances) {
                        var categories = kvp.Value & dance.Categories;
                        if (categories != DanceCategories.None) {
                            tdis.Add(new TrackDanceInfo(dance, categories, kvp.Key));
                        }
                    }
                }
            }

            if (UnratedCategories != DanceCategories.None) {
                foreach (var dance in Dances) {
                    var categories = UnratedCategories & dance.Categories;
                    if (categories != DanceCategories.None) {
                        var rating = RatingsByDance[dance] ?? DefaultRating;
                        tdis.Add(new TrackDanceInfo(dance, categories, rating));
                    }
                }
            }

            return tdis;
        }

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
                    bool valid = double.TryParse(split[1], NumberStyles.Any, CultureInfo.InvariantCulture, out val);
                    if (valid && (val >= 0.0) && (val <= 5.0)) {
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
            if (!rating.HasValue) {
                UnratedCategories |= categories;
            }
            else {
                if (CategoriesByRating.TryGetValue(rating.Value, out existing)) {
                    var newCategories = categories | existing;
                    CategoriesByRating[rating.Value] = newCategories;
                }
                else {
                    CategoriesByRating[rating.Value] = categories;
                }
            }
        }

        protected void UpdateAllCategories() {
            var all = DanceCategories.None;

            foreach (var kvp in CategoriesByRating) {
                if ((all & kvp.Value) != 0) {
                    throw new ApplicationException($"Multiple ratings for {kvp.Value}");
                }
                all |= kvp.Value;
            }

            if (UnratedCategories != DanceCategories.None) {
                UnratedCategories &= ~all;
            }

            Categories = all | UnratedCategories;
        }

        protected string ExtractTag(string rawTag) {
            uint? rating;
            var tag = ExtractRating(rawTag.Normalize(), out rating);

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
            return rawTag;
        }

        public DanceInfoTagParser(uint? defaultRating, DanceCategories defaultCategories, IEnumerable<string> tags) {
            DefaultRating = defaultRating;
            DefaultCategories = defaultCategories;

            UnusedTags = tags.Select<string, string>(ExtractTag).Where((s) => !String.IsNullOrEmpty(s)).ToList().AsReadOnly();

            UpdateAllCategories();
            Dances = RatingsByDance.Keys.ToList();
        }

        public DanceInfoTagParser(uint? defaultRating, DanceCategories defaultCategories, params string[] tags) 
            : this(defaultRating, defaultCategories, (IEnumerable<string>)tags)
        {
        }

        public DanceInfoTagParser(params string[] tags) : this(null, DanceCategories.None, tags) {

        }
    }
}

