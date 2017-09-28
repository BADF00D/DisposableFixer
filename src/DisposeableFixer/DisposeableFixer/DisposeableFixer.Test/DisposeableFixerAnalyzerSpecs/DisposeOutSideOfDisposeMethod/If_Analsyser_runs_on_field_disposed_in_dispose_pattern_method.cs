using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod
{
    [TestFixture]
    internal class If_Analsyser_runs_on_field_disposed_in_dispose_pattern_method :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }
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

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}