using DisposableFixer.Analyzers;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.UnusedDisposableAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyzer_runs_on_empty_Dispose_method : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Descriptor.Description.Should().Be(Unused.DisposableDescriptor.Description);
        }
    }

    [TestFixture]
    internal class If_Analyzer_runs_on_Dispose_method_with_empty_lines : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {

        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Descriptor.Description.Should().Be(Unused.DisposableDescriptor.Description);
        }
    }

    [TestFixture]
    internal class If_Analyzer_runs_on_Dispose_method_with_comments_in_statement_block : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
            // new MemoryStream().Dispose()
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Descriptor.Description.Should().Be(Unused.DisposableDescriptor.Description);
        }
    }

    [TestFixture]
    internal class If_Analyzer_runs_on_empty_dispose_method_inherited_from_an_interface : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal interface IHaveSomeResources : IDisposable
    {
    }
    internal class EmptyDisposable : IHaveSomeResources
    {
        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            // The interfaces requires a dispose method - we cannot change this here. So we expect no diagnostic
            diagnostics.Length.Should().Be(0);
        }
    }

    [TestFixture]
    internal class If_Analyzer_runs_on_empty_dispose_method_inherited_from_an_interface_and_a_base_class : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal interface IHaveSomeResources : IDisposable
    {
    }

    internal class Base : IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    internal class EmptyDisposable : Base, IDisposable
    {
        public override void Dispose()
        {
        }
    }
}";

            [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Descriptor.Description.Should().Be(Unused.DisposableDescriptor.Description);
        }
    }

    [TestFixture]
    internal class If_Analyzer_runs_on_dispose_method_inherited_from_an_interface_and_a_base_class_throwing_a_NotImplementedException : Spec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal interface IHaveSomeResources : IDisposable
    {
    }

    internal class Base : IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    internal class EmptyDisposable : Base, IDisposable
    {
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

        [TestFixture]
        internal class
            If_Analyzer_runs_on_dispose_method_inherited_from_an_interface_and_a_base_class_with_ThrowExpression_NotImplementedException :
                Spec
        {
            private const string Code = @"
using System;

namespace SomeNamespace
{
    internal interface IHaveSomeResources : IDisposable
    {
    }

    internal class Base : IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    internal class EmptyDisposable : Base, IDisposable
    {
        public override void Dispose() => throw new NotImplementedException();
    }
}";

            [Test]
            public void Apply_CodeFix_should_not_throw_Exception()
            {
                PrintCodeToAnalyze(Code);
                var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
                diagnostics.Length.Should().Be(0);
            }
        }

        [TestFixture]
        internal class
            If_Analyzer_runs_on_dispose_method_inherited_from_an_interface_and_a_base_class_with_ExpressionBody :
                Spec
        {
            private const string Code = @"
using System;

namespace SomeNamespace
{
    internal interface IHaveSomeResources : IDisposable
    {
    }

    internal class Base : IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class EmptyDisposable : Base, IDisposable
    {
        private IDisposable mem;
        public override void Dispose() => mem?.Dispose();
    }
}";

            [Test]
            public void Apply_CodeFix_should_not_throw_Exception()
            {
                PrintCodeToAnalyze(Code);
                var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
                diagnostics.Length.Should().Be(0);
            }
        }
    }
}