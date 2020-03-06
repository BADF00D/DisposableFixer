using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class UndisposedFieldCodeFixProvider : UndisposedMemberCodeFixProvider
    {
        private static readonly IConfiguration Configuration = ConfigurationManager.Instance;

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                Id.ForAssignment.FromMethodInvocation.ToField.OfSameType,
                Id.ForAssignment.FromObjectCreation.ToField.OfSameType
            );

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == Id.ForAssignment.FromObjectCreation.ToField.OfSameType
                || id == Id.ForAssignment.FromMethodInvocation.ToField.OfSameType)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(ActionTitle.DisposeFieldInDisposeMethod, c => CreateDisposeCallInParameterlessDisposeMethod(context, c)),
                    context.Diagnostics);

                var sm = await context.Document.GetSemanticModelAsync(context.CancellationToken);
                var memberSymbol = sm.GetEnclosingSymbol(context.Span.Start, context.CancellationToken);
                var containingSymbol = memberSymbol.ContainingSymbol;
                if (containingSymbol is ITypeSymbol typeSymbol)
                {
                    
                    var cleanupMethods = typeSymbol.GetOverrideableMethods()
                        .Where(m =>
                        {
                            if (Configuration.DisposingMethods.TryGetValue(m.Name, out var overloads))
                            {
                                return overloads
                                    .Where(o => !o.IsStatic)
                                    .Any(overload =>
                                    {
                                        var parameters = m.Parameters;
                                        if (parameters.Length != overload.Parameter.Length) return false;

                                        for (var i = 0; i < parameters.Length; i++)
                                        {
                                            var typeName = parameters[i].Name;
                                            if (overload.Parameter[i] != typeName) return false;
                                        }

                                        return true;
                                    });
                            }
                            return false;
                        }).ToArray();

                    foreach (var cleanupMethod in cleanupMethods)
                    {
                        //todo add parameter here
                        var title = string.Format(ActionTitle.DisposeFieldInXMethod, cleanupMethod.Name, string.Empty);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: title, 
                                createChangedDocument: c => CreateDisposeCallInParameterlessDisposeMethod(context, c)),
                            context.Diagnostics);
                    }
                }
            }
        }

        protected override TypeSyntax GetTypeOfMemberDeclarationOrDefault(ClassDeclarationSyntax @class, string memberName)
        {
            return @class
                .DescendantNodes<FieldDeclarationSyntax>()
                .SelectMany(fds => fds.DescendantNodes<VariableDeclaratorSyntax>())
                    .Where(vds => vds.Identifier.Text == memberName)
                    .Select(vds => vds.TryFindParent<VariableDeclarationSyntax>(@class, out var variableDeclaration) ? variableDeclaration : null)
                    .Select(vd => vd?.Type)
                    .FirstOrDefault();
        }
    }
}