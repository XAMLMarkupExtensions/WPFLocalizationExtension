#region Copyright information
// <copyright file="BitmapSourceTypeConverter.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Konrad Mattheis</author>
#endregion

namespace WPFLocalizeExtension.ValueConverters
{
    #region Usings
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    #endregion

    /// <summary>
    /// Takes the first value as StringFormat and the other values as Parameter for the StringFormat
    /// </summary>
    public class StringFormatConverter : TypeValueConverterBase, IMultiValueConverter
    {
        private static MethodInfo miFormat = null;

        #region IMultiValueConverter
        /// <inheritdoc/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (miFormat == null)
            {
                try
                {
                    // try to load SmartFormat Assembly
                    var asSmartFormat = Assembly.Load("SmartFormat");
                    var tt = asSmartFormat.GetType("SmartFormat.Smart");
                    miFormat = tt.GetMethod("Format", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(object) }, null);
                }
                catch
                {
                    // fallback just take String.Format
                    miFormat = typeof(string).GetMethod("Format", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(object) }, null);
                }
            }

            if (!targetType.IsAssignableFrom(typeof(string)))
                throw new Exception("TargetType is not supported strings");

            if (values == null || values.Length < 1)
                throw new Exception("Not enough parameters");

            if (values[0] == null)
                return null;

            if (values.Length > 1 && values[1] == DependencyProperty.UnsetValue)
                return null;

            return (string)miFormat.Invoke(null, values);
        }

        /// <inheritdoc/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        } 
        #endregion
    }
}
