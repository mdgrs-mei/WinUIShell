using System.Text.Json;
using WinUIShell.Common;

namespace WinUIShell.Server;

internal class ApiExporter : Singleton<ApiExporter>
{
    private class Api
    {
        public List<EnumDef> Enums { get; set; } = [];
    }

    private class TypeDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
    }

    private class EnumDef : TypeDef
    {
        public string UnderlyingType { get; set; } = "";
        public List<EnumEntryDef> Items { get; set; } = [];
    }

    private class EnumEntryDef
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
    }

    private readonly Api _api = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

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

        foreach (var value in Enum.GetValues(enumType))
        {
            var underlyingTypeValue = Convert.ChangeType(value, underlyingType);
            def.Items.Add(new EnumEntryDef
            {
                Name = value.ToString()!,
                Value = underlyingTypeValue
            });
        }

        _api.Enums.Add(def);
    }

    private void ExportToFile(string filePath)
    {
        string jsonString = JsonSerializer.Serialize(_api, _jsonOptions);
        File.WriteAllText(filePath, jsonString);
    }
}
