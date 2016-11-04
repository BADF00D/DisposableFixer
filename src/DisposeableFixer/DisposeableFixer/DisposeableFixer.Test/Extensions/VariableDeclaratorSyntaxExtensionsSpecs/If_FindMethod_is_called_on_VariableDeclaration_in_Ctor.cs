using System.Linq;
using DisposeableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposeableFixer.Test.Extensions.VariableDeclaratorSyntaxExtensionsSpecs
{
    [TestFixture]
    internal class If_FindMethod_is_called_on_VariableDeclaration_in_Ctor : Spec {
        private MethodDeclarationSyntax _methodDeclarationSyntax;
        private const string Code = @"
using System;
namespace DisFixerTest.Misc{
    public class ClassWithtoutFields    {
        private ClassWithtoutFields()
        {
            var integer = 3;
        }
    }
}
";
        protected override void BecauseOf() {
            var variableDeclaratorSyntax = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();

            _methodDeclarationSyntax = variableDeclaratorSyntax.FindContainingMethod();
        }


        [Test]
        public void Then_result_should_be_null()
        {
            _methodDeclarationSyntax.Should().BeNull();
        }
    }
}