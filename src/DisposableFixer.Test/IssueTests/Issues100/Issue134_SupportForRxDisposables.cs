using DisposableFixer.Test.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    [CategoryReactiveExtension]
    internal class Issue134_SupportForRxDisposables : IssueSpec
    {

        private const string Code = @"
using System.Reactive.Disposables;
using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Threading;

namespace DisFixerTest.ReactiveProperty
{
    public class RxStuff : IDisposable
    {
        public BooleanDisposable BooleanDisposable { get; } = new BooleanDisposable();
        public CancellationDisposable CancellationDisposable { get; } = new CancellationDisposable();
        public CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();
        public ContextDisposable ContextDisposable { get; } = new ContextDisposable(SynchronizationContext.Current, new MemoryStream());// no warning for MemoryStream because tracked by ContextDisposable
        public MultipleAssignmentDisposable MultipleAssignmentDisposable { get; } = new MultipleAssignmentDisposable();
        public RefCountDisposable RefCountDisposable { get; } = new RefCountDisposable(new MemoryStream());// no warning for MemoryStream because tracked by RefCountDisposable
        public ScheduledDisposable ScheduledDisposable { get; } = new ScheduledDisposable(TaskPoolScheduler.Default, new MemoryStream());// no warning for MemoryStream because tracked by ScheduledDisposable
        public SerialDisposable SerialDisposable { get; } = new SerialDisposable();
        public SingleAssignmentDisposable SingleAssignmentDisposable { get; } = new SingleAssignmentDisposable();

        public RxStuff()
        {
            CompositeDisposable.Add(new MemoryStream());
            MultipleAssignmentDisposable.Disposable = new MemoryStream();// no warning because tracked my MultipleAssignmentDisposable
            SerialDisposable.Disposable = new MemoryStream();// no warning because tracked my SerialDisposable
            SingleAssignmentDisposable.Disposable = new MemoryStream();// no warning because tracked my SingleAssignmentDisposable

        }

        public void Dispose()
        {
            BooleanDisposable?.Dispose();
            CancellationDisposable?.Dispose();
            CompositeDisposable?.Dispose();
            ContextDisposable?.Dispose();
            MultipleAssignmentDisposable?.Dispose();
            RefCountDisposable?.Dispose();
            ScheduledDisposable?.Dispose();
            SerialDisposable?.Dispose();
            SingleAssignmentDisposable?.Dispose();
        }
    }
}

namespace System.Reactive.Concurrency
{
    public interface IScheduler { }
    public class TaskPoolScheduler : IScheduler
    {
        public static TaskPoolScheduler Default => throw new NotImplementedException();
    }
}

namespace System.Reactive.Disposables
{
    public abstract class SomeDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
    public class BooleanDisposable : SomeDisposable { }
    public class CancellationDisposable : SomeDisposable { }

    public class CompositeDisposable : SomeDisposable
    {
        public void Add(IDisposable disposable) { }
    }

    public class ContextDisposable : SomeDisposable
    {
        public ContextDisposable(SynchronizationContext context, IDisposable disposable)
        {
        }
    }

    public class MultipleAssignmentDisposable : SomeDisposable
    {
        public IDisposable Disposable { get; set; }
    }

    public class RefCountDisposable : SomeDisposable
    {
        public RefCountDisposable(IDisposable disposable){}
    }

    public class ScheduledDisposable : SomeDisposable
    {
        public ScheduledDisposable(IScheduler scheduler, IDisposable disposable)
        {
        }
    }

    public class SerialDisposable : SomeDisposable
    {
        public IDisposable Disposable { get; set; }
    }

    public class SingleAssignmentDisposable : SomeDisposable
    {
        public IDisposable Disposable { get; set; }
    }
}
";

        [Test]
        public void Then_there_should_one_diagnostic_with_correct_message()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}