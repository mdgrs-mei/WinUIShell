using System.ComponentModel;

namespace WinUIShell.ApiExporter;

public class Api
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

    public static readonly List<(string FullName, string ShortName)> SystemTypes =
    [
        ("System.Boolean", "bool"),
        ("System.Byte", "byte"),
        ("System.SByte", "sbyte"),
        ("System.Char", "char"),
        ("System.Decimal", "decimal"),
        ("System.Double", "double"),
        ("System.Single", "float"),
        ("System.Int32", "int"),
        ("System.UInt32", "uint"),
        ("System.Int64", "long"),
        ("System.UInt64", "ulong"),
        ("System.Int16", "short"),
        ("System.UInt16", "ushort"),
        ("System.String", "string"),
        ("System.Object", "object"),
        ("System.Void", "void"),
    ];

    private static readonly List<string> _unsupportedTypes =
    [
        "System.IntPtr",
        "WinRT.IWinRTObject",
        "WinRT.IObjectReference",
        "WinRT.ObjectReference",
    ];

    private static readonly List<string> _supportedSystemInterfaces =
    [
        "System.IDisposable",
        "System.Collections.Generic.ICollection",
        "System.Collections.Generic.IList",
        "System.Collections.IEnumerable",
        "System.Collections.Generic.IEnumerable",
        "System.Collections.IEnumerator",
        "System.Collections.Generic.IEnumerator",
        "System.Collections.Generic.IReadOnlyList",
        "System.Collections.Generic.IReadOnlyCollection",
    ];

    private static readonly List<string> _unsupportedMethodNames =
    [
        "Equals",
        "GetHashCode",
        "GetType",
    ];

    public static bool TryReplaceSystemTypeNameWithShortName(string fullTypeName, out string? shortTypeName)
    {
        foreach (var (fullName, shortName) in SystemTypes)
        {
            if (fullTypeName == fullName)
            {
                shortTypeName = shortName;
                return true;
            }
        }
        shortTypeName = null;
        return false;
    }

    public static bool IsUnsupportedType(string typeDefName)
    {
        return _unsupportedTypes.Contains(typeDefName);
    }

    public static bool IsSupportedSystemInterface(string typeDefName)
    {
        return _supportedSystemInterfaces.Contains(typeDefName);
    }

    public static bool IsUnsupportedMethod(string methodName)
    {
        return _unsupportedMethodNames.Contains(methodName);
    }

    public List<EnumDef> Enums { get; } = [];
    public List<ObjectDef> Objects { get; } = [];

    public class EnumDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
        public string UnderlyingType { get; set; } = "";
        public List<EnumEntryDef> Entries { get; } = [];
    }

    public class EnumEntryDef
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
    }

    public class ObjectDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
        public TypeDef Type { get; set; } = new();

        [DefaultValue(null)]
        public TypeDef? BaseType { get; set; }

        [DefaultValue(null)]
        public List<TypeDef>? Interfaces { get; set; }

        [DefaultValue(null)]
        public List<PropertyDef>? StaticProperties { get; set; }

        [DefaultValue(null)]
        public List<PropertyDef>? InstanceProperties { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? Constructors { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? StaticMethods { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? InstanceMethods { get; set; }

        [DefaultValue(null)]
        public List<EventDef>? StaticEvents { get; set; }

        [DefaultValue(null)]
        public List<EventDef>? InstanceEvents { get; set; }

        [DefaultValue(null)]
        public List<ObjectDef>? NestedTypes { get; set; }
    }

    public class PropertyDef
    {
        public string Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();

        [DefaultValue(null)]
        public TypeDef? ExplicitInterfaceType { get; set; }

        [DefaultValue(null)]
        public List<ParameterDef>? IndexParameters { get; set; }

        [DefaultValue(false)]
        public bool CanRead { get; set; }

        [DefaultValue(false)]
        public bool CanWrite { get; set; }

        [DefaultValue(false)]
        public bool IsVirtual { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        [DefaultValue(false)]
        public bool HidesBase { get; set; }

        [DefaultValue(false)]
        public bool ImplementsInterface { get; set; }

        [DefaultValue(false)]
        public bool ImplementsSystemInterface { get; set; }
    }

    public class TypeDef
    {
        public string Name { get; set; } = "";

        [DefaultValue(true)]
        public bool IsPublic { get; set; } = true;

        [DefaultValue(false)]
        public bool IsNullable { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsEnum { get; set; }

        [DefaultValue(true)]
        public bool IsClass { get; set; } = true;

        [DefaultValue(false)]
        public bool IsArray { get; set; }

        [DefaultValue(false)]
        public bool IsDelegate { get; set; }

        [DefaultValue(false)]
        public bool IsPointer { get; set; }

        [DefaultValue(false)]
        public bool IsFunctionPointer { get; set; }

        [DefaultValue(false)]
        public bool IsByRef { get; set; }

        [DefaultValue(false)]
        public bool IsIn { get; set; }

        [DefaultValue(false)]
        public bool IsOut { get; set; }

        [DefaultValue(false)]
        public bool IsGenericType { get; set; }

        [DefaultValue(false)]
        public bool IsGenericTypeParameter { get; set; }

        [DefaultValue(false)]
        public bool IsGenericMethodParameter { get; set; }

        [DefaultValue(0)]
        public int GenericParameterPosition { get; set; }

        [DefaultValue(false)]
        public bool IsInterface { get; set; }

        [DefaultValue(false)]
        public bool IsSystemObject { get; set; }

        [DefaultValue(null)]
        public TypeDef? ElementType { get; set; }

        // Parent type if this is a nested type.
        [DefaultValue(null)]
        public TypeDef? ParentType { get; set; }

        [DefaultValue(null)]
        public List<TypeDef>? GenericTypeArguments { get; set; }
    }

    public class MethodDef
    {
        public string? Name { get; set; }

        [DefaultValue(null)]
        public TypeDef? ReturnType { get; set; }

        [DefaultValue(null)]
        public List<ParameterDef>? Parameters { get; set; }

        [DefaultValue(null)]
        public TypeDef? ExplicitInterfaceType { get; set; }

        [DefaultValue(false)]
        public bool IsGenericMethod { get; set; }

        [DefaultValue(false)]
        public bool IsVirtual { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        [DefaultValue(false)]
        public bool HidesBase { get; set; }

        [DefaultValue(false)]
        public bool ImplementsInterface { get; set; }

        [DefaultValue(false)]
        public bool ImplementsSystemInterface { get; set; }
    }

    public class EventDef
    {
        public string Name { get; set; } = "";

        [DefaultValue(null)]
        public List<ParameterDef>? Parameters { get; set; }
    }

    public class ParameterDef
    {
        public string? Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();
    }

#pragma warning restore CA2227
#pragma warning restore CA1002
#pragma warning restore CA1034
}
