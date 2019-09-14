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
        public const string ForNotDisposedFactoryProperty = "DF0028";
        public const string ForNotDisposedStaticFactoryProperty = "DF0029";

        public const string ForHiddenIDisposable = "DF0100";
    }

    internal static class Hidden
    {
        public static readonly DiagnosticDescriptor Disposable =
            new DiagnosticDescriptor(
                id: Id.ForHiddenIDisposable,
                title: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.HiddenDisposableTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                messageFormat: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.HiddenDisposableMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.HiddenDisposableDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)));
    }

    internal static class NotDisposed
    {
        internal static class Assignment
        {
            public static class FromObjectCreation
            {
                public static readonly DiagnosticDescriptor ToPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromObjectCreationToPropertyNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning, 
                        isEnabledByDefault: true, 
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));

                public static readonly DiagnosticDescriptor ToStaticPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromObjectCreationToStaticPropertyNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));

                public static readonly DiagnosticDescriptor ToFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(id: Id.ForAssignmentFromObjectCreationToFieldNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning, 
                        isEnabledByDefault: true, 
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));
                public static readonly DiagnosticDescriptor ToStaticFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(id: Id.ForAssignmentFromObjectCreationToStaticFieldNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));
            }

            public static class FromMethodInvocation
            {
                public static readonly DiagnosticDescriptor ToFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromMethodInvocationToFieldNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));

                public static readonly DiagnosticDescriptor ToStaticFieldNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromMethodInvocationToStaticFieldNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));

                public static readonly DiagnosticDescriptor ToPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromMethodInvocationToPropertyNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));

                public static readonly DiagnosticDescriptor ToStaticPropertyNotDisposedDescriptor =
                    new DiagnosticDescriptor(
                        id: Id.ForAssignmentFromMethodInvocationToStaticPropertyNotDisposed,
                        title: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyNotDisposedTitle),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        messageFormat: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyNotDisposedMessageFormat),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)),
                        category: Category.WrongUsage,
                        defaultSeverity: DiagnosticSeverity.Warning,
                        isEnabledByDefault: true,
                        description: new LocalizableResourceString(
                            nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyNotDisposedDescription),
                            resourceManager: Resources.ResourceManager,
                            resourceSource: typeof(Resources)));
            }
        }

        internal static class AnonymousObject
        {
            public static readonly DiagnosticDescriptor FromMethodInvocationDescriptor =
                new DiagnosticDescriptor(
                    id: Id.ForAnonymousObjectFromMethodInvocation,
                    title: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)),
                    messageFormat: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)),
                    category: Category.WrongUsage,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true,
                    description: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromMethodInvocationDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)));

            public static readonly DiagnosticDescriptor FromObjectCreationDescriptor =
                new DiagnosticDescriptor(id: Id.ForAnonymousObjectFromObjectCreation,
                    title: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationTitle),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)),
                    messageFormat: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationMessageFormat),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)),
                    category: Category.WrongUsage,
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true,
                    description: new LocalizableResourceString(
                        nameOfLocalizableResource: nameof(Resources.AnonymousObjectFromObjectCreationDescription),
                        resourceManager: Resources.ResourceManager,
                        resourceSource: typeof(Resources)));
        }

        internal static class LocalVariable
        {
            public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
                id: Id.ForNotDisposedLocalVariable,
                title: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                messageFormat: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedLocalVariableDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)));
        }

        internal static class FactoryProperty
        {
            public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
                id: Id.ForNotDisposedFactoryProperty,
                title: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryPropertyTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                messageFormat: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryPropertyMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryPropertyDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)));
        }

        internal static class StaticFactoryProperty
        {
            public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
                id: Id.ForNotDisposedStaticFactoryProperty,
                title: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryStaticPropertyTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                messageFormat: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryStaticPropertyMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.NotDisposedFactoryStaticPropertyDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)));
        }
    }

    
}