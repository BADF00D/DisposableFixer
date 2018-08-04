
using System;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_78 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace MyNs {
    public interface IDoSomething : IDisposable {
    }
    internal class Dummy : IDoSomething {
        private MemoryStream Stream = new MemoryStream();

        public void Dispose() { }
    }
}";

        private const string ExpectedCode = @"
using System;
using System.IO;

namespace MyNs {
    public interface IDoSomething : IDisposable {
    }
    internal class Dummy : IDoSomething {
        private MemoryStream Stream = new MemoryStream();

        public void Dispose() {
            Stream?.Dispose();
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostic_with_correct_ID()
        {
            _diagnostics.Length.Should().Be(1, "this is the prerequisite");
            _diagnostics[0].Id.Should().Be(SyntaxNodeAnalysisContextExtension
                .IdForAssignmendFromObjectCreationToFieldNotDisposed);

            Console.WriteLine("Soure code");
            Console.WriteLine(Code);
            Console.WriteLine("Expected code");
            Console.WriteLine(ExpectedCode);

            var fixedCode =MyHelper.ApplyCSharpCodeFix(Code, _diagnostics[0],
                new UndisposedFieldCodeFixProvider());

            Console.WriteLine("Fix code");
            Console.WriteLine(fixedCode);

            fixedCode.Should().Be(ExpectedCode);
        }
    }
}