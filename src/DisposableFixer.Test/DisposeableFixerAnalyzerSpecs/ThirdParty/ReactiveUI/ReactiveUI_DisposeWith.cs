using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.ReactiveUI
{
    [TestFixture]
    internal class ReactiveUI_DisposeWith : Spec
    {

        private const string Code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

namespace MyNamespace
{
    class MyClass : IDisposable
    {
        private CompositeDisposable _disposeables = new CompositeDisposable();
        private IDisposable Property { get; }
        public MyClass()
        {
            Property = new MemoryStream().DisposeWith(_disposeables);
        }

        public void Dispose()
        {
            _disposeables?.Dispose();
        }
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
            throw new NotImplementedException();
        }
    }
}

namespace System.Reactive.Disposables
{
    public static class DisposableMixins
    {
        public static T DisposeWith<T>(this T item, CompositeDisposable compositeDisposable) where T : IDisposable
        {
            throw new NotImplementedException();
        }
    }
}";
        [Test]
        public void Then_there_should_be_no()
        {
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}