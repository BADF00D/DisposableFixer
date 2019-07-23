using Microsoft.CodeAnalysis;

namespace DisposableFixer
{
    internal static class Constants
    {
        public const string Variablename = "variable";
        public const string Dispose = "Dispose";
        public const string SystemIDisposable = "System.IDisposable";
        public const string System = "System";
        public const string IDisposable = "IDisposable";
        public const string Interlocked = "Interlocked";
        public const string Exchange = "Exchange";
        public const string Var = "var";
    }

    internal static class Category {
        public const string WrongUsage = "Wrong Usage";
    }

    internal static class ActionTitle
    {
        public const string WrapInUsing = "Wrap in using";
        public const string DisposePropertyInDisposeMethod = "Dispose property in Dispose() method";
        public const string DisposeFieldInDisposeMethod = "Dispose field in Dispose() method";
        public const string CreateFieldAndDisposeInDisposeMethod = "Create field and dispose in Dispose() method.";
        public const string DisposeAfterLastUsage = "Dispose after last usage";
    }

    internal static class Id
    {
        public const string ForAnonymousObjectFromObjectCreation = "DF0000";
        public const string ForAnonymousObjectFromMethodInvocation = "DF0001";
        public const string ForNotDisposedLocalVariable = "DF0010";
        public const string ForAssignmentFromObjectCreationToFieldNotDisposed = "DF0020";
        public const string ForAssignmentFromMethodInvocationToFieldNotDisposed = "DF0021";
        public const string ForAssignmentFromObjectCreationToPropertyNotDisposed = "DF0022";
        public const string ForAssignmentFromMethodInvocationToPropertyNotDisposed = "DF0023";
    }

    internal static class Descriptor
    {
        private static readonly LocalizableString AnonymousObjectFromMethodInvocationTitle =
            new LocalizableResourceString(
                nameof(Resources.AnonymousObjectFromMethodInvocationTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromMethodInvocationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromMethodInvocationDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromMethodInvocationDescriptor =
            new DiagnosticDescriptor(Id.ForAnonymousObjectFromMethodInvocation, AnonymousObjectFromMethodInvocationTitle, AnonymousObjectFromMethodInvocationMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AnonymousObjectFromMethodInvocationDescription);

        private static readonly LocalizableString AnonymousObjectFromObjectCreationMessageFormat =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationTitle =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AnonymousObjectFromObjectCreationDescription =
            new LocalizableResourceString(nameof(Resources.AnonymousObjectFromObjectCreationDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AnonymousObjectFromObjectCreationDescriptor =
            new DiagnosticDescriptor(Id.ForAnonymousObjectFromObjectCreation, AnonymousObjectFromObjectCreationTitle, AnonymousObjectFromObjectCreationMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AnonymousObjectFromObjectCreationDescription);

        private static readonly LocalizableString NotDisposedLocalVariableMessageFormat =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableTitle =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NotDisposedLocalVariableDescription =
            new LocalizableResourceString(nameof(Resources.NotDisposedLocalVariableDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor NotDisposedLocalVariableDescriptor = new DiagnosticDescriptor(Id.ForNotDisposedLocalVariable, NotDisposedLocalVariableTitle, NotDisposedLocalVariableMessageFormat, Category.WrongUsage,
            DiagnosticSeverity.Warning, true, NotDisposedLocalVariableDescription);

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToPropertyNotDisposedDescription =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromObjectCreationToPropertyNotDisposedDescriptor =
            new DiagnosticDescriptor(Id.ForAssignmentFromObjectCreationToPropertyNotDisposed, AssignmendFromObjectCreationToPropertyNotDisposedTitle, AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AssignmentFromObjectCreationToPropertyNotDisposedDescription);

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromObjectCreationToFieldNotDisposedDescriptor =
            new DiagnosticDescriptor(Id.ForAssignmentFromObjectCreationToFieldNotDisposed, AssignmentFromObjectCreationToFieldNotDisposedTitle, AssignmentFromObjectCreationToFieldNotDisposedMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AssignmentFromObjectCreationToFieldNotDisposedDescription);

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedDescription =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromMethodInvocationToFieldNotDisposedDescriptor =
            new DiagnosticDescriptor(Id.ForAssignmentFromMethodInvocationToFieldNotDisposed, AssignmentFromMethodInvocationToFieldNotDisposedTitle, AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AssignmentFromMethodInvocationToFieldNotDisposedDescription);

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedTitle =
            new LocalizableResourceString(nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedDescription =
            new LocalizableResourceString(
                nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedDescription),
                Resources.ResourceManager,
                typeof(Resources));

        public static readonly DiagnosticDescriptor AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor =
            new DiagnosticDescriptor(Id.ForAssignmentFromMethodInvocationToPropertyNotDisposed, AssignmentFromMethodInvocationToPropertyNotDisposedTitle, AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat, Category.WrongUsage,
                DiagnosticSeverity.Warning, true, AssignmentFromMethodInvocationToPropertyNotDisposedDescription);
    }
}