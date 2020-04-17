using System;

using Windows.UI.Xaml.Data;

namespace MapApp.Helpers
{
    /// <summary>
    /// Checks if an enum is of a certain value. For XAML usage.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>Gets or sets the type of checked enum.</summary>
        public Type EnumType { get; set; }

        /// <summary>
        /// Checks if a string represents an enum value. 
        /// </summary>
        /// <param name="value">Enum value represented.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Checked string representation.</param>
        /// <param name="language"></param>
        /// <returns><b>True</b> if the checked string represents the enum value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
                }

                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }

        /// <summary>
        /// Converts string representation of enum value to enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>Enum value represented</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }
    }
}
