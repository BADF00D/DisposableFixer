using System;
using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Configuration
{
    internal sealed class TrackingTypeDetector : IDetector
    {
        private readonly IConfiguration _configuration = ConfigurationManager.Instance;

        public bool IsIgnoredInterface(INamedTypeSymbol namedType)
        {
            var name = namedType.GetFullNamespace();
            return _configuration.IgnoredInterfaces.Contains(name);
        }

        public bool IsIgnoredType(INamedTypeSymbol namedType)
        {
            var name = namedType.GetFullNamespace();
            return _configuration.IgnoredTypes.Contains(name);
        }

        public bool IsTrackedType(INamedTypeSymbol namedType, ObjectCreationExpressionSyntax node,
            SemanticModel semanticModel)
        {
            if (node == null) return false;
            var name = namedType.GetFullNamespace();

            //check if this type is tracked at all
            if (!_configuration.TrackingTypes.Contains(name)) return false;


            IReadOnlyCollection<CtorCall> nonTrackingCtorsForThisType;
            if (!_configuration.IgnoredTrackingTypeCtorCalls.TryGetValue(name, out nonTrackingCtorsForThisType))
                return true;

            var nonTrackingCtorsWithSameParameterCount = nonTrackingCtorsForThisType
                .Where(c => c.Parameter.Length == node.ArgumentList.Arguments.Count)
                .ToArray();

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
            if (literal.IsKind(SyntaxKind.TrueLiteralExpression))
                return !nonTrackingCtorUsed.FlagIndicationNonDisposedResource;
            return !literal.IsKind(SyntaxKind.FalseLiteralExpression)
                   || nonTrackingCtorUsed.FlagIndicationNonDisposedResource;
        }

        public bool IsTrackingMethodCall(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel)
        {
            var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return false; //something like Create() this cant be a tracking call

            var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression);
            if (symbolInfo.Symbol == null) return false;

            var method = symbolInfo.Symbol as IMethodSymbol;

            return method.IsExtensionMethod
                ? AnalyzeExtensionMethodCall(method)
                : AnalyzeNonExtensionMethodCall(method);
        }

        public bool IsIgnoredFactoryMethod(InvocationExpressionSyntax methodInvocation, SemanticModel semanticModel)
        {
            var memberAccessExpression = methodInvocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return false; //something like Create() this cant be a tracking call

            var symbolInfo = semanticModel.GetSymbolInfo(methodInvocation);

            var method = symbolInfo.Symbol as IMethodSymbol;
            if (method == null) return false;
            
            foreach (var typeName in GetTypeAndInterfaces(method.ReceiverType).Select(type => type.GetFullNamespace()))
            {
                IReadOnlyCollection<MethodCall> methodCalls;
                if (!_configuration.TrackingFactoryMethods.TryGetValue(typeName, out methodCalls)) continue;

                var ifTracked =
                    methodCalls.Any(mc => mc.Name == method.Name && mc.Parameter.Length == method.Parameters.Length);
                if (ifTracked) return true;
            }

            return false;
        }

        public bool IsTrackedSetter(string fullQualifiedPropertyName, TrackingMode? trackingMode = null) =>
            _configuration.TrackedSet.TryGetValue(fullQualifiedPropertyName, out var mode)
            && (!trackingMode.HasValue || mode == trackingMode);

        private IEnumerable<ITypeSymbol> GetTypeAndInterfaces(ITypeSymbol type)
        {
            yield return type;
            foreach (var @interface in type.Interfaces)
            {
                yield return @interface;
            }
        }

        private bool AnalyzeNonExtensionMethodCall(IMethodSymbol method)
        {
            //todo merge with AnalyzeExtensionMethodCall
            var originalDefinition = method.OriginalDefinition;
            var methodName = method.Name;
            var @namespace = originalDefinition.ContainingType.GetFullNamespace();

            return _configuration.TrackingMethods
                .Where(tm => tm.Key == @namespace)
                .Select(tm => tm.Value)
                .Any(tm => tm.Any(mc => mc.Name == methodName));
        }

        private bool AnalyzeExtensionMethodCall(IMethodSymbol method)
        {
            var originalDefinition = method.OriginalDefinition;
            var methodName = method.Name;
            var @namespace = originalDefinition.ContainingType.GetFullNamespace(); //does not work classname missing

            var numberOfParameter = originalDefinition.Parameters.Count();
            return _configuration.TrackingMethods
                .Where(tm => tm.Key == @namespace)
                .Select(tm => tm.Value)
                .Any(tm => tm.Any(mc => mc.Name == methodName && mc.IsStatic && mc.Parameter.Length -1 == numberOfParameter));
        }

        private static CtorCall GetCtorConfiguration(IMethodSymbol ctorInUse,
            IEnumerable<CtorCall> nonTrackingCtorsWithSameParameterCount)
        {
            foreach (var ctorCall in nonTrackingCtorsWithSameParameterCount)
            {
                for (var i = 0; i < ctorCall.Parameter.Length; i++)
                {
                    var parameterForCtorInUse = ctorInUse.Parameters[i].Type;
                    var nameOfParameterType = ctorCall.Parameter[i];

                    if (parameterForCtorInUse.Name == nameOfParameterType) return ctorCall;
                }
            }

            return null;
        }
    }
}