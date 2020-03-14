using DisposableFixer.CodeFix;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue129_CreateField_in_VS2019 : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System.IO;
using System.Xml.Serialization;

namespace SomeNamespace
{
    internal class Base
    {
        private object DeserializeSubscriptionResponse(string resp)
        {
            var serializer = new XmlSerializer(typeof(object));
            var stream = GenerateStreamFromString(resp);
            return null;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(s);
                writer.Flush();
            }

            stream.Position = 0;
            return stream;
        }
    }
}";

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForLocal.Variable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }
}