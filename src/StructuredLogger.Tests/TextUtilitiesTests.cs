﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Logging.StructuredLogger;
using Xunit;

namespace StructuredLogger.Tests
{
    public class TextUtilitiesTests
    {
        [Fact]
        public void TestGetLines()
        {
            T("", "");
            T2("", "");
            T("a", "a");
            T2("a", "a");
            T("a\r\nb", "a", "b");
            T2("a\r\nb", "a\r\n", "b");
            T("a\nb", "a", "b");
            T2("a\nb", "a\n", "b");
            T("a\rb", "a", "b");
            T2("a\rb", "a\r", "b");
            T("\r", "", "");
            T("\r\r", "", "", "");
            T("\r\r\r", "", "", "", "");
            T2("\r", "\r", "");
            T2("\r\r", "\r", "\r", "");
            T2("\r\r\r", "\r", "\r", "\r", "");
            T("\n", "", "");
            T("\n\n", "", "", "");
            T("\n\n\n", "", "", "", "");
            T2("\n", "\n", "");
            T2("\n\n", "\n", "\n", "");
            T2("\n\n\n", "\n", "\n", "\n", "");
            T("\n\r", "", "", "");
            T2("\n\r", "\n", "\r", "");
            T("\r\n", "", "");
            T2("\r\n", "\r\n", "");
            T("\r\n\r", "", "", "");
            T2("\r\n\r", "\r\n", "\r", "");
            T("\r\r\n", "", "", "");
            T2("\r\r\n", "\r", "\r\n", "");
            T("a\r\na\r\na", "a", "a", "a");
            T2("a\r\na\r\na", "a\r\n", "a\r\n", "a");
            T("a\r\nb\nc\rd", "a", "b", "c", "d");
            T2("a\r\nb\nc\rd", "a\r\n", "b\n", "c\r", "d");
            T2("a\r\rb", "a\r", "\r", "b");
        }

        [Fact]
        public void TestGetNumberOfLeadingSpaces()
        {
            var text = "abcd   efghi";
            Assert.Equal(3, Utilities.GetNumberOfLeadingSpaces(text, new Span(4, 6)));
            Assert.Equal(2, Utilities.GetNumberOfLeadingSpaces(text, new Span(4, 2)));
            Assert.Equal(1, Utilities.GetNumberOfLeadingSpaces(text, new Span(5, 1)));
            Assert.Equal(0, Utilities.GetNumberOfLeadingSpaces(text, new Span(1, 3)));
        }

        [Fact]
        public void TestSkip()
        {
            Assert.Equal(3, new Span(1, 10).Skip(2).Start);
            Assert.Equal(8, new Span(1, 10).Skip(2).Length);
        }

        [Fact]
        public void TestStringOperations()
        {
            var text = "abcd   efghi";
            Assert.Equal("   efg", text.Substring(new Span(4, 6)));
            Assert.Equal(true, text.Contains(new Span(7, 4), 'e'));
            Assert.Equal(7, text.IndexOf(new Span(7, 4), 'e'));
        }

        [Fact]
        public void TestParseNameValue()
        {
            var text = "abcd1=2hi";
            var nameValue = Utilities.ParseNameValue(text, new Span(4, 3));
            Assert.Equal("1", nameValue.Key);
            Assert.Equal("2", nameValue.Value);
        }

        private static void T(string text, params string[] expectedLines)
        {
            var actualLines = text.GetLines();
            AssertEqual(expectedLines, actualLines, text);
        }

        private static void T2(string text, params string[] expectedLines)
        {
            var actualLines = text.GetLines(includeLineBreak: true);
            AssertEqual(expectedLines, actualLines, text);
        }

        private static void AssertEqual(IEnumerable<string> expectedLines, IEnumerable<string> actualLines, string message = "")
        {
            if (!expectedLines.SequenceEqual(actualLines))
            {
                message = $"{Escape(message)}\r\nExpected: {string.Join(", ", expectedLines.Select(Escape))}\r\nActual  : {string.Join(", ", actualLines.Select(Escape))}";
                throw new Exception(message);
            }
        }

        private static string Escape(string text)
        {
            return "\"" + text.Replace("\r", "\\r").Replace("\n", "\\n") + "\"";
        }
    }
}
