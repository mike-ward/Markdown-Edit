using System.IO;
// <copyright file="UtilityTest.cs">Copyright ©  2015, Mike Ward</copyright>

using System;
using MarkdownEdit.Models;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarkdownEdit.Models
{
    [TestClass]
    [PexClass(typeof(Utility))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UtilityTest
    {
        [PexMethod]
        internal FileSystemWatcher WatchFile(string file, Action onChange)
        {
            FileSystemWatcher result = Utility.WatchFile(file, onChange);
            return result;
            // TODO: add assertions to method UtilityTest.WatchFile(String, Action)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        internal Func<TKey, TResult> Memoize<TKey, TResult>(Func<TKey, TResult> func)
        {
            Func<TKey, TResult> result = Utility.Memoize<TKey, TResult>(func);
            return result;
            // TODO: add assertions to method UtilityTest.Memoize(Func`2<!!0,!!1>)
        }
    }
}
