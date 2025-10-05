namespace WinUIShell.Server;

public class Api
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1002 // Do not expose generic lists
    public List<EnumDef> Enums { get; } = [];
    public List<ObjectDef> Objects { get; } = [];

    public class TypeDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
    }

    public class EnumDef : TypeDef
    {
        public string UnderlyingType { get; set; } = "";
        public List<EnumEntryDef> Entries { get; } = [];
    }

    public class EnumEntryDef
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
    }

    public class ObjectDef : TypeDef
    {
        public List<PropertyDef> StaticProperties { get; } = [];
        public List<PropertyDef> InstanceProperties { get; } = [];
    }

    public class PropertyDef
    {
        public string Name { get; set; } = "";
        public ArgumentType Type { get; set; } = new();
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }

    public class ArgumentType
    {
        public string Name { get; set; } = "";
        public bool IsNullable { get; set; }
        public bool IsEnum { get; set; }
    }
#pragma warning restore CA1002
#pragma warning restore CA1034
}
