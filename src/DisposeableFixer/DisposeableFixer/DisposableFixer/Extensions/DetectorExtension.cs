using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Configuration;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.Extensions
{
    internal static class DetectorExtension
    {
        public static bool IsIgnoredTypeOrImplementsIgnoredInterface(this IDetector detector, INamedTypeSymbol type)
        {
            if (!type.IsType) return false;
            if (detector.IsIgnoredType(type)) return true;
            /* maybe the given type symbol is a interface. We cannot check if a type
             * is a interface, so we simply take the brute force approach and check,
             * if this type is in list of ignored interfaces */
            if (detector.IsIgnoredInterface(type)) return true;

            return type.AllInterfaces
                .Select(ai => ai).Any(detector.IsIgnoredInterface);
        }
    }
}