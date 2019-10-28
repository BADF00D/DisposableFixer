using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using DisposableFixer.Extensions;

namespace DisposableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class If_ContainsUsingsOfVariableNamed_is_called_on_Class_with_using_statement_for_another_disposable :
        Spec
    {
        private bool _containsUsing;
        private const string Code = @"
using System.IO;
namespace DisFixerTest.Misc
{
    public class ClassWithOtherUsing {
        public ClassWithOtherUsing()
        {
            using(var memstream = new MemoryStream()) { }
        }
        public string Dis { get; set; }
        private void Do() { }
        private string Bla;
    }
}
";

        protected override void BecauseOf()
        {
            var fieldDeclarationSyntax = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .LastOrDefault();

            _containsUsing = fieldDeclarationSyntax.ContainsUsingOfVariableNamed("test");
        }

        [Test]
        public void Then_result_should_false()
        {
            _containsUsing.Should().BeFalse();
        }
    }
}