using WinUIShell.Common;

namespace WinUIShell;

public class FontFamily : WinUIShellObject
{
    public static FontFamily XamlAutoFontFamily
    {
        get => PropertyAccessor.GetStatic<FontFamily>(
            "Microsoft.UI.Xaml.Media.FontFamily, Microsoft.WinUI",
            nameof(XamlAutoFontFamily))!;
    }

    public string Source
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Source))!;
        set => PropertyAccessor.Set(Id, nameof(Source), value);
    }

    internal FontFamily(ObjectId id)
        : base(id)
    {
    }

    public FontFamily(string familyName)
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.FontFamily, Microsoft.WinUI",
            this,
            familyName);
    }
}
