using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Configuration {
	internal sealed class TrackingTypeDetector : IDetector {
		private readonly IConfiguration _configuration = ConfigurationManager.Instance;
		public bool IsIgnoredInterface(INamedTypeSymbol namedType) {
			var name = namedType.GetFullNamespace();
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
		    return !literal.IsKind(SyntaxKind.FalseLiteralExpression) 
                || nonTrackingCtorUsed.FlagIndicationNonDisposedResource;
		}

	    public bool IsTrackingMethodCall(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel)
	    {
            var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
	        if (memberAccessExpression == null) return false; //something like Create() this cant be a tracking call
            var methodName = memberAccessExpression.Name.Identifier.Text;

            var symbols = semanticModel.LookupSymbols(0);
	        var s = semanticModel.LookupNamespacesAndTypes(0);
            var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression);
            if (symbolInfo.Symbol == null) return false;

            var method = symbolInfo.Symbol as IMethodSymbol;
            if (method.IsExtensionMethod)
            {
                return AnalyseExtensionMethodCall(method, semanticModel);
            }
            else
            {
                return AnalyseNonExtensionMethodCall(method, semanticModel);
            }
	    }

        private bool AnalyseNonExtensionMethodCall(IMethodSymbol method, SemanticModel semanticModel)
        {
            return false;
        }

        private bool AnalyseExtensionMethodCall(IMethodSymbol method, SemanticModel semanticModel)
        {
            var originalDefinition = method.OriginalDefinition;
            var methodName = method.Name;
            var @namespace = originalDefinition.ContainingType.GetFullNamespace(); //does not work classname missing

            var extensionMethods = _configuration.TrackingMethods.Where(tm => tm.Key == @namespace).Select(tm => tm.Value).ToArray();
            if (!extensionMethods.Any()) return false;

            return extensionMethods.Any(tm => tm.Any(mc => mc.Name == methodName));
        }

        private static CtorCall GetCtorConfiguration(IMethodSymbol ctorInUse, CtorCall[] nonTrackingCtorsWithSameParameterCount) {
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
	}
}