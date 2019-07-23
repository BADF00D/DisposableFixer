using DisposableFixer.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer.Utils
{
    internal class CustomAnalysisContext
    {
        public static CustomAnalysisContext WithOriginalNode(SyntaxNodeAnalysisContext context, DisposableSource source,
            INamedTypeSymbol type, IDetector detector)
        {
            return new CustomAnalysisContext(context, context.Node, context.Node, source, type, detector);
        }

        public static CustomAnalysisContext WithOtherNode(SyntaxNodeAnalysisContext context, SyntaxNode node,
            DisposableSource source,
            INamedTypeSymbol type, IDetector detector)
        {
            return new CustomAnalysisContext(context, node, context.Node, source, type, detector);
        }

        private CustomAnalysisContext(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxNode originalNode, DisposableSource source, INamedTypeSymbol type, IDetector detector)
        {
            Context = context;
            Source = source;
            Type = type;
            Detector = detector;
            Node = node;
            OriginalNode = originalNode;
        }

        public SyntaxNodeAnalysisContext Context { get; }
        public DisposableSource Source { get; }
        public INamedTypeSymbol Type { get; }
        public IDetector Detector { get; }
        public SyntaxNode Node { get; }
        public SyntaxNode OriginalNode { get; }
        public SemanticModel SemanticModel => Context.SemanticModel;
    }
}