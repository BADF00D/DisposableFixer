using System.IO;
using System.Linq;
using DisposeableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposeableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class If_ContainsUsingsOfVariableNamed_is_called_on_Class_without_using : Spec
    {
        private bool _containsUsing;
        private const string Code = @"
namespace DisFixerTest.Misc
{
    public class ClassWithoutUsing {
        public ClassWithoutUsing() { }
        public string Dis { get; set; }
        private void Do() { }
        private string Bla;
    }
}
";
        protected override void BecauseOf() {
            var fieldDeclarationSyntax = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .LastOrDefault();

            _containsUsing = fieldDeclarationSyntax.ContainsUsingsOfVariableNamed("test");
        }

        [Test]
        public void Then_result_should_false()
        {
            _containsUsing.Should().BeFalse();
        } 
    }
}