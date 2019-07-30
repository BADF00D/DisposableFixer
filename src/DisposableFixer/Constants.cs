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
        public const string ForAssignmentFromObjectCreationToStaticFieldNotDisposed = "DF0024";
        public const string ForAssignmentFromMethodInvocationToStaticFieldNotDisposed = "DF0025";
        public const string ForAssignmentFromObjectCreationToStaticPropertyNotDisposed = "DF0026";
        public const string ForAssignmentFromMethodInvocationToStaticPropertyNotDisposed = "DF0027";
    }

    internal static class NotDisposed
    {
        internal static class Assignment
        {
            public static class FromObjectCreation
            {
                private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedTitle =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromObjectCreationToPropertyNotDisposedDescription =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                public static readonly DiagnosticDescriptor ToPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: ForAssignmentFromObjectCreationToPropertyNotDisposed,
                        title: AssignmendFromObjectCreationToPropertyNotDisposedTitle,
                        messageFormat: AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat,
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning, 
                        isEnabledByDefault: true, 
                        description: AssignmentFromObjectCreationToPropertyNotDisposedDescription);

                private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedTitle =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedMessageFormat =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromObjectCreationToFieldNotDisposedDescription =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                public static readonly DiagnosticDescriptor ToFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(id: ForAssignmentFromObjectCreationToFieldNotDisposed,
                        title: AssignmentFromObjectCreationToFieldNotDisposedTitle,
                        messageFormat: AssignmentFromObjectCreationToFieldNotDisposedMessageFormat,
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning, 
                        isEnabledByDefault: true, 
                        description: AssignmentFromObjectCreationToFieldNotDisposedDescription);

                public const string ForAssignmentFromObjectCreationToFieldNotDisposed = Id.ForAssignmentFromObjectCreationToFieldNotDisposed;
                public const string ForAssignmentFromObjectCreationToPropertyNotDisposed = Id.ForAssignmentFromObjectCreationToPropertyNotDisposed;
                public const string ForAssignmentFromObjectCreationToStaticFieldNotDisposed = Id.ForAssignmentFromObjectCreationToStaticFieldNotDisposed;
                public const string ForAssignmentFromObjectCreationToStaticPropertyNotDisposed = Id.ForAssignmentFromObjectCreationToStaticPropertyNotDisposed;
            }

            public static class FromMethodInvocation
            {
                private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedTitle =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromMethodInvocationToFieldNotDisposedDescription =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                public static readonly DiagnosticDescriptor ToFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: ForAssignmentFromMethodInvocationToFieldNotDisposed,
                        title: AssignmentFromMethodInvocationToFieldNotDisposedTitle,
                        messageFormat: AssignmentFromMethodInvocationToFieldNotDisposedMessageFormat,
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: AssignmentFromMethodInvocationToFieldNotDisposedDescription);

                private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedTitle =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                private static readonly LocalizableString AssignmentFromMethodInvocationToPropertyNotDisposedDescription =
                    new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources));

                public static readonly DiagnosticDescriptor ToPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: ForAssignmentFromMethodInvocationToPropertyNotDisposed,
                        title: AssignmentFromMethodInvocationToPropertyNotDisposedTitle,
                        messageFormat: AssignmentFromMethodInvocationToPropertyNotDisposedMessageFormat,
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: AssignmentFromMethodInvocationToPropertyNotDisposedDescription);

                public const string ForAssignmentFromMethodInvocationToFieldNotDisposed = Id.ForAssignmentFromMethodInvocationToFieldNotDisposed;
                public const string ForAssignmentFromMethodInvocationToPropertyNotDisposed = Id.ForAssignmentFromMethodInvocationToPropertyNotDisposed;
                public const string ForAssignmentFromMethodInvocationToStaticFieldNotDisposed = Id.ForAssignmentFromMethodInvocationToStaticFieldNotDisposed;
                public const string ForAssignmentFromMethodInvocationToStaticPropertyNotDisposed = Id.ForAssignmentFromMethodInvocationToStaticPropertyNotDisposed;
            }
        }

        internal static class AnonymousObject
        {
            private static readonly LocalizableString AnonymousObjectFromMethodInvocationTitle =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            private static readonly LocalizableString AnonymousObjectFromMethodInvocationMessageFormat =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            private static readonly LocalizableString AnonymousObjectFromMethodInvocationDescription =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            public static readonly DiagnosticDescriptor FromMethodInvocationDescriptor =
                new DiagnosticDescriptor(
                    id: ForAnonymousObjectFromMethodInvocation,
                    title: AnonymousObjectFromMethodInvocationTitle,
                    messageFormat: AnonymousObjectFromMethodInvocationMessageFormat,
                    category: Category.WrongUsage,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true,
                    description: AnonymousObjectFromMethodInvocationDescription);

            private static readonly LocalizableString AnonymousObjectFromObjectCreationMessageFormat =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            private static readonly LocalizableString AnonymousObjectFromObjectCreationTitle =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            private static readonly LocalizableString AnonymousObjectFromObjectCreationDescription =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            public static readonly DiagnosticDescriptor FromObjectCreationDescriptor =
                new DiagnosticDescriptor(id: ForAnonymousObjectFromObjectCreation,
                    title: AnonymousObjectFromObjectCreationTitle,
                    messageFormat: AnonymousObjectFromObjectCreationMessageFormat,
                    category: Category.WrongUsage,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true,
                    description: AnonymousObjectFromObjectCreationDescription);

            public const string ForAnonymousObjectFromObjectCreation = Id.ForAnonymousObjectFromObjectCreation;
            public const string ForAnonymousObjectFromMethodInvocation = Id.ForAnonymousObjectFromMethodInvocation;
        }

        internal static class LocalVariable
        {
            internal static readonly LocalizableString NotDisposedLocalVariableMessageFormat =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            internal static readonly LocalizableString NotDisposedLocalVariableTitle =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            internal static readonly LocalizableString NotDisposedLocalVariableDescription =
                new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources));

            public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
                id: ForNotDisposedLocalVariable,
                title: NotDisposedLocalVariableTitle,
                messageFormat: NotDisposedLocalVariableMessageFormat,
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: NotDisposedLocalVariableDescription);

            public const string ForNotDisposedLocalVariable = Id.ForNotDisposedLocalVariable;
        }
    }

    
}