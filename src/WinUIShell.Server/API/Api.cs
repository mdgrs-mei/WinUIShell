namespace WinUIShell.Server;

public class Api
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1002 // Do not expose generic lists
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
        public TypeDef? BaseType { get; set; }
        public List<TypeDef> Interfaces { get; } = [];
        public List<PropertyDef> StaticProperties { get; } = [];
        public List<PropertyDef> InstanceProperties { get; } = [];
        public List<MethodDef> Constructors { get; } = [];
        public List<MethodDef> StaticMethods { get; } = [];
        public List<MethodDef> InstanceMethods { get; } = [];
        public List<ObjectDef> NestedTypes { get; } = [];
    }

    public class PropertyDef
    {
        public string Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();
        public TypeDef? ExplicitInterfaceType { get; set; }
        public List<ParameterDef> IndexParameters { get; } = [];
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsOverride { get; set; }
        public bool HidesBase { get; set; }
    }

    public class TypeDef
    {
        public string Name { get; set; } = "";
        public bool IsNullable { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsEnum { get; set; }
        public bool IsArray { get; set; }
        public bool IsDelegate { get; set; }
        public bool IsPointer { get; set; }
        public bool IsFunctionPointer { get; set; }
        public bool IsByRef { get; set; }
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
        public bool IsGenericType { get; set; }
        public bool IsGenericTypeParameter { get; set; }
        public bool IsGenericMethodParameter { get; set; }
        public bool IsInterface { get; set; }
        public bool IsSystemObject { get; set; }
        public TypeDef? ElementType { get; set; }
        public List<TypeDef> GenericTypeArguments { get; } = [];
    }

    public class MethodDef
    {
        public string? Name { get; set; }
        public TypeDef? ReturnType { get; set; }
        public List<ParameterDef> Parameters { get; } = [];
        public TypeDef? ExplicitInterfaceType { get; set; }
        public bool IsGenericMethod { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsOverride { get; set; }
        public bool HidesBase { get; set; }
    }

    public class ParameterDef
    {
        public string? Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();
    }

    private static readonly List<string> _supportedSystemInterfaces =
    [
        "System.Collections.Generic.ICollection",
        "System.Collections.Generic.IList",
        "System.Collections.IEnumerable",
        "System.Collections.Generic.IEnumerable",
        "System.Collections.IEnumerator",
        "System.Collections.Generic.IEnumerator",
    ];
    public static bool IsSupportedSystemInterface(string typeDefName)
    {
        return _supportedSystemInterfaces.Contains(typeDefName);
    }

#pragma warning restore CA1002
#pragma warning restore CA1034
}
