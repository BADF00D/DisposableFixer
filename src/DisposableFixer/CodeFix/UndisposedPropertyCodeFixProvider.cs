using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class UndisposedPropertyCodeFixProvider : UndisposedMemberCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SyntaxNodeAnalysisContextExtension.IdForAssignmendFromMethodInvocationToPropertyNotDisposed,
                SyntaxNodeAnalysisContextExtension.IdForAssignmendFromObjectCreationToPropertyNotDisposed
            );

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == SyntaxNodeAnalysisContextExtension.IdForAssignmendFromObjectCreationToPropertyNotDisposed
                || id == SyntaxNodeAnalysisContextExtension.IdForAssignmendFromMethodInvocationToPropertyNotDisposed)
            {
                context.RegisterCodeFix(
                    CodeAction.Create("Dispose property in Dispose() method", c => CreateDisposeCallInParameterlessDisposeMethod(context, c)),
                    context.Diagnostics);
            }

            return Task.FromResult(1);
        }

        protected override TypeSyntax GetTypeOfMemberDeclarationOrDefault(ClassDeclarationSyntax @class, string memberName)
        {
            return @class
                .DescendantNodes<PropertyDeclarationSyntax>()
                .FirstOrDefault(pds => pds.Identifier.Text == memberName)
                ?.Type;
        }
    }
}