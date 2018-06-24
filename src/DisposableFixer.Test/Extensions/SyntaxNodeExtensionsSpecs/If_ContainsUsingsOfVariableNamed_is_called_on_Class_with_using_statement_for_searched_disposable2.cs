using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class If_ContainsUsingsOfVariableNamed_is_called_on_Class_with_using_statement_for_searched_disposable2 :
        Spec
    {
        private bool _containsUsing;
        private const string Code = @"
using System.IO;
using System.IO;
namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public void SomeMethod() {
            var memstream = new MemoryStream();
            using (memstream) { }
        }
    }
}
";

        protected override void BecauseOf()
        {
            var fieldDeclarationSyntax = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .LastOrDefault();

            _containsUsing = fieldDeclarationSyntax.ContainsUsingsOfVariableNamed("memstream");
        }

        [Test]
        public void Then_result_should_true()
        {
            _containsUsing.Should().BeTrue();
        }
    }
}