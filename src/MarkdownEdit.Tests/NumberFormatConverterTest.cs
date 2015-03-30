using System.Globalization;
// <copyright file="NumberFormatConverterTest.cs">Copyright ©  2015, Mike Ward</copyright>

using System;
using MarkdownEdit;
using MarkdownEdit.Converters;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarkdownEdit
{
    [TestClass]
    [PexClass(typeof(NumberFormatConverter))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class NumberFormatConverterTest
    {
        [PexMethod]
        public object Convert(
            [PexAssumeUnderTest]NumberFormatConverter target,
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            object result = target.Convert(value, targetType, parameter, culture);
            return result;
            // TODO: add assertions to method NumberFormatConverterTest.Convert(NumberFormatConverter, Object, Type, Object, CultureInfo)
        }
    }
}
