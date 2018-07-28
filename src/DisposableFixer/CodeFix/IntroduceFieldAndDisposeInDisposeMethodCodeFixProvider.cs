using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider : CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnotic = context.Diagnostics.FirstOrDefault();
            if(diagnotic == null) return Task.CompletedTask;

            if (diagnotic.Id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation ||
                diagnotic.Id == SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
            {
                context.RegisterCodeFix(
                    CodeAction.Create("Create field and dispose in Dispose() method.", cancel => IntroduceFieldAndDisposeInDisposeMethod(context, cancel)),
                    diagnotic
                );
            }

            return Task.CompletedTask;
        }

        private static Task<Document> IntroduceFieldAndDisposeInDisposeMethod(CodeFixContext context, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation,
            SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation,
            SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable
        );
    }
}
