using Microsoft.CodeAnalysis;

namespace DisposableFixer
{
    internal static class Constants
    {
        public const string Variablename = "variable";
        public const string Dispose = "Dispose";
        public const string Disposable = "disposable";
        public const string SystemIDisposable = "System.IDisposable";
        public const string System = "System";
        public const string IDisposable = "IDisposable";
        public const string Task = "Task";
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
        public const string UseUsingDeclaration = "Use using declaration";
        public const string DeclareLocalVariableAndUseUsingDeclaration = "Declare local variable and use using declaration";
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

        public static class ForAssignment
        {
            public static class FromObjectCreation
            {
                public static class ToProperty {
                    public const string OfSameType = "DF0022";
                    public const string OfAnotherType = "DF0032";
                }

                public static class ToStaticProperty
                {
                    public const string OfSameType = "DF0026";
                    public const string OfAnotherType = "DF0036";
                }
                public static class ToField {
                    public const string OfSameType = "DF0020";
                    public const string OfAnotherType = "DF0030";
                }

                public static class ToStaticField
                {
                    public const string OfSameType = "DF0024";
                    public const string OfAnotherType = "DF0034";
                }
            }

            public static class FromMethodInvocation
            {
                public static class ToProperty {
                    public const string OfSameType = "DF0023";
                    public const string OfAnotherType = "DF0033";
                }
                public static class ToStaticProperty {
                    public const string OfSameType = "DF0027";
                    public const string OfAnotherType = "DF0037";
                }
                public static class ToField {
                    public const string OfSameType = "DF0021";
                    public const string OfAnotherType = "DF0031";
                }
                public static class ToStaticField {
                    public const string OfSameType = "DF0025";
                    public const string OfAnotherType = "DF0035";
                }
            }
        }

        public const string ForNotDisposedFactoryProperty = "DF0028";
        public const string ForNotDisposedStaticFactoryProperty = "DF0029";

        public const string ForHiddenIDisposable = "DF0100";
        public const string EmptyDisposable = "DF0110";
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
    internal static class Unused
    {
        public static readonly DiagnosticDescriptor DisposableDescriptor =
            new DiagnosticDescriptor(
                id: Id.EmptyDisposable,
                title: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.EmptyDisposableTitle),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                messageFormat: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.EmptyDisposableMessageFormat),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)),
                category: Category.WrongUsage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(
                    nameOfLocalizableResource: nameof(Resources.EmptyDisposableDescription),
                    resourceManager: Resources.ResourceManager,
                    resourceSource: typeof(Resources)));
    }

    internal static class NotDisposed
    {
        internal static class Assignment
        {
            public static class FromObjectCreation
            {
                public static class ToProperty
                {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromObjectCreation.ToProperty.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromObjectCreation.ToProperty.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToPropertyOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }

                public static class ToStaticProperty
                {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromObjectCreation.ToStaticProperty.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromObjectCreation.ToStaticProperty.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticPropertyOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }

                public static class ToField
                {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(id: Id.ForAssignment.FromObjectCreation.ToField.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(id: Id.ForAssignment.FromObjectCreation.ToField.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToFieldOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }

                public static class ToStaticField
                {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(id: Id.ForAssignment.FromObjectCreation.ToStaticField.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(id: Id.ForAssignment.FromObjectCreation.ToStaticField.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromObjectCreationToStaticFieldOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }
            }

            public static class FromMethodInvocation
            {
                public static class ToProperty {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToProperty.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToProperty.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToPropertyOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }
                public static class ToStaticProperty {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToStaticProperty.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToStaticProperty.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticPropertyOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }
                public static class ToField {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToField.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToField.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToFieldOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }
                public static class ToStaticField {
                    public static readonly DiagnosticDescriptor OfSameTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToStaticField.OfSameType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfSameTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfSameTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfSameTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                    public static readonly DiagnosticDescriptor OfAnotherTypeDescriptor =
                        new DiagnosticDescriptor(
                            id: Id.ForAssignment.FromMethodInvocation.ToStaticField.OfAnotherType,
                            title: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfAnotherTypeNotDisposedTitle),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            messageFormat: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfAnotherTypeNotDisposedMessageFormat),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)),
                            category: Category.WrongUsage,
                            defaultSeverity: DiagnosticSeverity.Warning,
                            isEnabledByDefault: true,
                            description: new LocalizableResourceString(
                                nameOfLocalizableResource: nameof(Resources.AssignmendFromMethodInvocationToStaticFieldOfAnotherTypeNotDisposedDescription),
                                resourceManager: Resources.ResourceManager,
                                resourceSource: typeof(Resources)));
                }
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
