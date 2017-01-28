using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Configuration {
	internal sealed class Detector : IDetector {
		private readonly IConfiguration _configuration = ConfigurationManager.Instance;
		public bool IsIgnoredInterface(INamedTypeSymbol named_type) {
			var name = named_type.GetFullNamespace();
			return _configuration.IgnoredInterfaces.Contains(name);
		}

		public bool IsIgnoredType(INamedTypeSymbol namedType) {
			var name = namedType.GetFullNamespace();
			return _configuration.IgnoredTypes.Contains(name);
		}

		public bool IsTrackedType(INamedTypeSymbol namedType, ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
		{
		    if (node == null) return false;
			var name = namedType.GetFullNamespace();

            //check if this type is tracked at all
            if (!_configuration.TrackingTypes.Contains(name)) return false;

            
            IReadOnlyCollection<CtorCall> nonTrackingCtorsForThisType;
		    if(!_configuration.IgnoredTrackingTypeCtorCalls.TryGetValue(name, out nonTrackingCtorsForThisType)) return true;

		    var nonTrackingCtorsWithSameParameterCount =
		        nonTrackingCtorsForThisType.Where(c => c.Parameter.Length == node.ArgumentList.Arguments.Count).ToArray();

		    if (!nonTrackingCtorsWithSameParameterCount.Any()) return true;

            //get INamedTypeSymbol for this ObjectCreation
            var typeInfo = semanticModel.GetTypeInfo(node);
            var type = typeInfo.Type as INamedTypeSymbol;
            if (type == null) return false;

		    var ctorInUseSymbolInfo = semanticModel.GetSymbolInfo(node);
		    var ctorInUse = ctorInUseSymbolInfo.Symbol as IMethodSymbol;
		    if (ctorInUse == null) return true;

		    var nonTrackingCtorUsed = GetCtorConfiguration(ctorInUse, nonTrackingCtorsWithSameParameterCount);

		    var token = node.GetContentArgumentAtPosition(nonTrackingCtorUsed.PositionOfFlagParameter);
		    var literal = token as LiteralExpressionSyntax;
		    if (literal.IsKind(SyntaxKind.TrueLiteralExpression))return !nonTrackingCtorUsed.FlagIndicationNonDisposedResource;
		    if (literal.IsKind(SyntaxKind.FalseLiteralExpression))
		        return nonTrackingCtorUsed.FlagIndicationNonDisposedResource;
		    return true;
		}

	    private CtorCall GetCtorConfiguration(IMethodSymbol ctorInUse, CtorCall[] nonTrackingCtorsWithSameParameterCount) {
	        foreach (var ctorCall in nonTrackingCtorsWithSameParameterCount)
	        {
	            for (var i = 0; i < ctorCall.Parameter.Length; i++)
	            {
	                var parameterForCtorInUse = ctorInUse.Parameters[i].Type;
	                var nameOfParameterType = ctorCall.Parameter[i];

                    if(parameterForCtorInUse.Name == nameOfParameterType) return ctorCall;
	            }
	        }

	        return null;
	    }

	    private static ArgumentSyntax GetArgumentOfDefault(SyntaxNode node, int positionOfFlagParameter)
	    {
	        var argumentsList = node.DescendantNodes<ArgumentListSyntax>().FirstOrDefault();
	        var arguments = argumentsList?.DescendantNodes<ArgumentSyntax>().Where(arg => arg.Parent == argumentsList);

	        return arguments?.Skip(positionOfFlagParameter - 1).FirstOrDefault();
	    }
	}
}