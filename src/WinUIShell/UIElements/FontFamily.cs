using WinUIShell.Common;

namespace WinUIShell;

public class FontFamily : WinUIShellObject
{
    public static FontFamily XamlAutoFontFamily
    {
        get => PropertyAccessor.GetStatic<FontFamily>(
            ObjectTypeMapping.Get().GetTargetTypeName<FontFamily>(),
            nameof(XamlAutoFontFamily))!;
    }

    public string Source
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Source))!;
        set => PropertyAccessor.Set(Id, nameof(Source), value);
    }

    public FontFamily(string familyName)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<FontFamily>(),
            this,
            familyName);
    }

    internal FontFamily(ObjectId id)
        : base(id)
    {
    }
}
