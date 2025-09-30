using System.Xml.Serialization;
using WinUIShell.Common;

namespace WinUIShell.Server;

public class ApiExporter : Singleton<ApiExporter>
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1002 // Do not expose generic lists
    public class Api
    {
        public List<EnumDef> Enums { get; } = [];
    }

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
#pragma warning restore CA1002
#pragma warning restore CA1034

    private readonly Api _api = new();

    public void Export(string apiFilePath)
    {
        AddEnums();
        ExportToFile(apiFilePath);
    }

    private void AddEnums()
    {
        AddEnumsInAssembly(typeof(Microsoft.UI.Xaml.Controls.BackgroundSizing)); // Microsoft.WinUI
        AddEnumsInAssembly(typeof(Microsoft.UI.Windowing.CompactOverlaySize)); // Microsoft.InteractiveExperiences.Projection
        AddEnumsInAssembly(typeof(Windows.UI.Text.FontStretch)); // Microsoft.Windows.SDK.NET
        AddEnum(typeof(EventCallbackRunspaceMode));
    }

    private void AddEnumsInAssembly(Type representativeTypeInAssembly)
    {
        var assembly = representativeTypeInAssembly.Assembly;
        var enumTypes = assembly.GetTypes().Where(type => type.IsEnum).OrderBy(type => type.FullName);

        foreach (var enumType in enumTypes)
        {
            AddEnum(enumType);
        }
    }

    private void AddEnum(Type enumType)
    {
        var assembly = enumType.Assembly;
        var underlyingType = Enum.GetUnderlyingType(enumType);

        var def = new EnumDef
        {
            Name = enumType.Name,
            FullName = $"{enumType.FullName}, {assembly.GetName().Name}",
            Namespace = enumType.Namespace!,
            UnderlyingType = underlyingType.Name
        };

        foreach (var entryName in Enum.GetNames(enumType))
        {
            var value = Enum.Parse(enumType, entryName);
            var underlyingTypeValue = Convert.ChangeType(value, underlyingType);
            def.Entries.Add(new EnumEntryDef
            {
                Name = entryName,
                Value = underlyingTypeValue
            });
        }

        _api.Enums.Add(def);
    }

    private void ExportToFile(string filePath)
    {
        var streamWriter = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(Api));
        serializer.Serialize(streamWriter, _api);
        streamWriter.Close();
    }
}
