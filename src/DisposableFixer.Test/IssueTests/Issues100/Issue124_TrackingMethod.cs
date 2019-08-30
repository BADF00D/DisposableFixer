using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue124_TrackingMethod : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream field;
    public MemoryStream Property { get; set; }
    private CompositeDisposable disposables;
    public WithTrackingMethod()
    {
        disposables = new CompositeDisposable(); //no warning as expected
        field = new MemoryStream(); //Warning : Field not disposed, should not be
        Property = new MemoryStream(); //Warning : Property not disposed, should not be
        var local = new MemoryStream(); //no warning as expected

        disposables.Add(local);
        disposables.Add(field);
        disposables.Add(Property);
    }
    public void Dispose()
    {
        disposables.Dispose();
    }
}
namespace System.Reactive.Disposables
{
    public class CompositeDisposable : IDisposable
    {
        public void Add(IDisposable item)
        {
        }

        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}