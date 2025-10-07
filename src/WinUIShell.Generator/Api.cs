using System.Xml.Serialization;

namespace WinUIShell.Generator;

public class Api
{
    public List<EnumDef> Enums { get; set; } = [];
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
        public bool IsValueType { get; set; }
        public bool IsArray { get; set; }
    }

    public static Api Load(string content)
    {
        var stringReader = new StringReader(content);
        var serializer = new XmlSerializer(typeof(Api));
        var api = (Api)serializer.Deserialize(stringReader);
        return api;
    }
}
