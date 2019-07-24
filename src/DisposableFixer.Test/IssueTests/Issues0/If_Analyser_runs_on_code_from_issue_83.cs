using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_83 : IssueSpec
    {
        private const string Code = @"
using System.Data.SqlClient;

namespace Demo
{
    internal class Program
    {
        public async void Test()
        {
            var reader = new SqlDataReader();
            reader.Close();
        }
    }
}

namespace System.Data.SqlClient
{
    internal class SqlDataReader : IDisposable
    {
        public void Dispose()
        {
        }

        public void Close()
        {
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            Console.WriteLine(Code);

            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}