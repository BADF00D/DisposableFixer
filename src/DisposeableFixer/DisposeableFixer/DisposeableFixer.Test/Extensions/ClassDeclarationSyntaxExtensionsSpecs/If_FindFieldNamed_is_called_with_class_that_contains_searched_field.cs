using System.Linq;
using DisposeableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposeableFixer.Test.Extensions.ClassDeclarationSyntaxExtensionsSpecs
{
    [TestFixture]
    internal class If_FindFieldNamed_is_called_with_class_that_contains_searched_field : Spec
    {
        private FieldDeclarationSyntax _fieldDeclaration;
        private const string Code = @"
using System;
namespace DisFixerTest.Misc
{
    public class ClassWithFieldBla
    {
        public ClassWithFieldBla()
        {
            Console.WriteLine("");
        }

        public string Dis { get; set; }

        private void Do()
        {
        }

        private string Bla;
    }
}
";

        protected override void BecauseOf() {
            var classDeclaration = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            _fieldDeclaration = classDeclaration.FindFieldNamed("Bla");
        }

        [Test]
        public void Then_result_should_not_be_null() {
            _fieldDeclaration.Should().NotBeNull();
        }

        [Test]
        public void Then_result_should_be_VariableDeclaration_with_Identifier_Bla()
        {
            var variableDeclaratorSyntax = _fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            variableDeclaratorSyntax.Identifier.Text.Should().Be("Bla");
        } 
    }
}