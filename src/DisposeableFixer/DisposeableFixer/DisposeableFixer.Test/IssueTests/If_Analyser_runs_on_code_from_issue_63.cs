using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_63 : IssueSpec
    {
        private const string Code = @"

using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

public class UsingWindsorContainer
{
    public UsingWindsorContainer()
    {
        var container = new WindsorContainer();

        container.Register();
        container.Install();

        container.Dispose();
    }
}



namespace Castle.Windsor
{
    public interface IWindsorContainer : IDisposable
    {
        IKernel Kernel { get; }
        string Name { get; }
        IWindsorContainer Parent { get; set; }
        void AddChildContainer(IWindsorContainer childContainer);
        IWindsorContainer AddFacility(IFacility facility);
        IWindsorContainer AddFacility<TFacility>() where TFacility : IFacility, new();
        IWindsorContainer AddFacility<TFacility>(Action<TFacility> onCreate) where TFacility : IFacility, new();
        IWindsorContainer GetChildContainer(string name);
        IWindsorContainer Install(params IWindsorInstaller[] installers);
        IWindsorContainer Register(params IRegistration[] registrations);
    }

    class WindsorContainer : IWindsorContainer
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IKernel Kernel { get; }
        public string Name { get; }
        public IWindsorContainer Parent { get; set; }
        public void AddChildContainer(IWindsorContainer childContainer)
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer AddFacility(IFacility facility)
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer AddFacility<TFacility>() where TFacility : IFacility, new()
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer AddFacility<TFacility>(Action<TFacility> onCreate) where TFacility : IFacility, new()
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer GetChildContainer(string name)
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer Install(params IWindsorInstaller[] installers)
        {
            throw new NotImplementedException();
        }

        public IWindsorContainer Register(params IRegistration[] registrations)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Castle.MicroKernel.Registration
{
    public interface IRegistration
    {
    }
}

namespace Castle.MicroKernel.Registration
{
    public interface IWindsorInstaller
    {
    }
}

namespace Castle.MicroKernel
{
    public interface IFacility
    {
    }
}

namespace Castle.MicroKernel
{
    public interface IKernel
    {
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