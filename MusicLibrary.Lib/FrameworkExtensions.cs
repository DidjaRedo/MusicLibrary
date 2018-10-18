using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace MusicLibrary.Lib
{
    public static class FrameworkExtensions
    {
        public static void AddUnique<T>(this List<T> list, T value) {
            if (!list.Contains(value)) {
                list.Add(value);
            }
        }

        public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> values) {
            foreach (var value in values) {
                if (!list.Contains(value)) {
                    list.Add(value);
                }
            }
        }

        public static string Normalize(this string s) {
            return s.Trim().ToLowerInvariant();
        }

        public static IEnumerable<string> Normalize(this IEnumerable<string> strings) {
            return strings.Select<string, string>((s) => s.Normalize());
        }

        public static IEnumerable<string> SplitAndNormalize(this string str, char separator) {
            if (String.IsNullOrEmpty(str)) {
                return new string[0];
            }

            return str.Split(separator).Normalize();
        }
    }
}
