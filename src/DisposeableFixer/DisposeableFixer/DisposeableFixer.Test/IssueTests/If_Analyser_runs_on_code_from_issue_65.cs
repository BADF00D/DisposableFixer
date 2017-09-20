using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_65 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

public class ExampleManager : IDisposable {
    private readonly IDisposable _db;

    public ExampleManager() {
        _db = new MemoryStream(); // This is detected as not disposed.
    }

    private bool disposing;

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            _db.Dispose();

            //Dispose of unmanaged resouces here
            disposing = true;
        }
    }

    ~ExampleManager() {
        Dispose(false);
    }

    public void Dispose() {
        Dispose(true);
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