namespace WPFLocalizeExtension.UnitTests.ValueConvertersTests
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using Xunit;
    using WPFLocalizeExtension.ValueConverters;
    #endregion
    
    /// <summary>
    /// Tests for converter <see cref="StringFormatConverter" />.
    /// </summary>
    public class StringFormatConverterTests
    {
        private const string CONVERTED_VALUE = "hello world";
        private readonly object[] _values = new object[] { "{0} {1}", "hello", "world" };
        
        /// <summary>
        /// Test data for <see cref="Convert_SpecifiedValues_ValueConverted" />.
        /// </summary>
        public static IReadOnlyList<object[]> ExpectedStringAndValuesData =>
            new List<object[]>
            {
                new object[] { "hello world", "{0} {1}", "hello", "world" },
                new object[] { "12345", "{0}{1}{2}{3}{4}", 1, 2, 3, 4, 5 },
                new object[] { "hello world", "hello world" },
                new object[] { "01.01.1970", "{0:dd.MM.yyyy}", new DateTime(1970, 1, 1, 10, 0, 0) },
                new object[] { "hello world", new StringBuilder("{0} {1}"), "hello", "world" },
            };
        
        /// <summary>
        /// Check that converter is supported <see cref="object" /> and <see cref="string" /> as target type.
        /// </summary>
        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        public void Convert_SupportedTargetType_ValueConverted(Type targetType)
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT.
            var convertedValue = converter.Convert(_values, targetType, null, CultureInfo.InvariantCulture);
            
            // ASSERT.
            Assert.Equal(CONVERTED_VALUE, convertedValue);
        }
        
        /// <summary>
        /// Check that converter is not supported target types others except <see cref="object" /> and <see cref="string" />.
        /// </summary>
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(StringBuilder))]
        public void Convert_UnsupportableTargetTypes_ExceptionThrown(Type targetType)
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT + ASSERT.
            var exception = Assert.Throws<Exception>(() => converter.Convert(_values, targetType, null, CultureInfo.InvariantCulture));
            Assert.Equal("TargetType is not supported strings", exception.Message);
        }

        /// <summary>
        /// Check that exception is thrown if passed null as values parameter.
        /// </summary>
        [Fact]
        public void Convert_ValuesIsNull_ExceptionThrown()
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT + ASSERT.
            var exception = Assert.Throws<Exception>(() => converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal("Not enough parameters", exception.Message);
        }
        
        /// <summary>
        /// Check that exception is thrown if passed empty array as values parameter.
        /// </summary>
        [Fact]
        public void Convert_ValuesIsEmpty_ExceptionThrown()
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT + ASSERT.
            var exception = Assert.Throws<Exception>(() => converter.Convert(Array.Empty<object>(), typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal("Not enough parameters", exception.Message);
        }
        
        /// <summary>
        /// Check that returns null if passed null format string.
        /// </summary>
        [Fact]
        public void Convert_FormatStringIsNull_ReturnsNull()
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT.
            var convertedValue = converter.Convert(new object[] { null, "hello", "world" }, typeof(string), null, CultureInfo.InvariantCulture);
            
            // ASSERT.
            Assert.Null(convertedValue);
        }
        
        /// <summary>
        /// Check that returns null if passed UnsetValue as second value.
        /// </summary>
        [Fact]
        public void Convert_SecondValueIsUnsetValue_ReturnsNull()
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT.
            var convertedValue = converter.Convert(new[] { CONVERTED_VALUE, DependencyProperty.UnsetValue }, typeof(string), null, CultureInfo.InvariantCulture);
            
            // ASSERT.
            Assert.Null(convertedValue);
        }
        
        /// <summary>
        /// Check different combinations of input values.
        /// </summary>
        [Theory]
        [MemberData(nameof(ExpectedStringAndValuesData))]
        public void Convert_SpecifiedValues_ValueConverted(string expectedConvertedValue, params object[] values)
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT.
            var convertedValue = converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);
            
            // ASSERT.
            Assert.Equal(expectedConvertedValue, convertedValue);
        }

        /// <summary>
        /// Check that ConvertBack just return null value without throw exceptions.
        /// </summary>
        [Fact]
        public void ConvertBack_AnyValue_ReturnsNull()
        {
            // ARRANGE.
            var converter = new StringFormatConverter();
            
            // ACT.
            var originalValues = converter.ConvertBack(CONVERTED_VALUE, new []{ typeof(string) }, null, CultureInfo.InvariantCulture);
            
            // ASSERT.
            Assert.Null(originalValues);
        }
    }
}