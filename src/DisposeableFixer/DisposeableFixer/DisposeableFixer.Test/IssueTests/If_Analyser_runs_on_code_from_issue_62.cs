using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_62 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace DisposeTests {
    public class TestClass {
        public TestClass() {
            var reader = new CsvReader(new StreamReader(new MemoryStream()), true);
            reader.Dispose();
        }
    }
}

namespace LumenWorks.Framework.IO.Csv {
    public class CsvReader : IDisposable {
        public const int DefaultBufferSize = 0x1000;
        public const char DefaultDelimiter = 'X';
        public const char DefaultQuote = 'X';
        public const char DefaultEscape = 'X';
        public const char DefaultComment = 'X';

        public CsvReader(TextReader reader, bool hasHeaders)
            : this(
                reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment,
                ValueTrimmingOptions.UnquotedOnly, DefaultBufferSize) {
        }

        public CsvReader(TextReader reader, bool hasHeaders, int bufferSize) : this(
                reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment,
                ValueTrimmingOptions.UnquotedOnly, bufferSize) {
        }

        public CsvReader(TextReader reader, bool hasHeaders, char delimiter)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment,
                ValueTrimmingOptions.UnquotedOnly, DefaultBufferSize) {
        }

        public CsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
            : this(
                reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment,
                ValueTrimmingOptions.UnquotedOnly, bufferSize) {
        }

        public CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment,
            ValueTrimmingOptions trimmingOptions, string nullValue = null)
            : this(reader, hasHeaders, delimiter, quote, escape, comment, trimmingOptions, DefaultBufferSize, nullValue) {
        }

        public CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment,
            ValueTrimmingOptions trimmingOptions, int bufferSize, string nullValue = null) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}

namespace LumenWorks.Framework.IO.Csv {
    [Flags]
    public enum ValueTrimmingOptions {
        None = 0,
        UnquotedOnly = 1,
        QuotedOnly = 2,
        All = UnquotedOnly | QuotedOnly
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}