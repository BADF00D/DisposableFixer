using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue142_Rx_Mutlicast : IssueSpec
    {

        private const string Code = @"
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SomeNamespace
{
    internal class TestClass
    {
        public TestClass(IObservable<int> source)
        {
            source.Multicast(new Subject<int>());
        }
    }
}

namespace System.Reactive.Linq
{
    public static class Observable
    {
        public static IConnectableObservable<TResult> Multicast<TSource, TResult>(this IObservable<TSource> source,
            ISubject<TSource, TResult> subject)
        {
            throw new NotImplementedException();
        }
    }
}

namespace System.Reactive.Subjects
{
    public interface ISubject<T,R>{}

    public sealed class Subject<T> : ISubject<T>, IDisposable
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public interface ISubject<T> : ISubject<T, T>, IObserver<T>, IObservable<T>
    {
    }
    public interface IConnectableObservable<out T> : IObservable<T>
    {
        IDisposable Connect();
    }
}


";
        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}