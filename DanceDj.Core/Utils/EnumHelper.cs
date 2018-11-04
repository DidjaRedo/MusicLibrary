using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.ComponentModel;

namespace DanceDj.Mvvm.Utils
{
    public static class EnumHelper
    {
        public class ValueDescription
        {
            public Enum Value { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        public static string Description(this Enum value) {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t) {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }
}
