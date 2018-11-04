using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;

using DanceDj.Mvvm.Utils;

namespace DanceDj.WPF.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<EnumHelper.ValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return EnumHelper.GetAllValuesAndDescriptions(value.GetType());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
