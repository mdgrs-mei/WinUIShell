namespace WinUIShell;

public static class FontWeights
{
    private const string _serverClassName = "Microsoft.UI.Text.FontWeights, Microsoft.WinUI";

    public static FontWeight Black
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Black))!;
    }

    public static FontWeight Bold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Bold))!;
    }

    public static FontWeight ExtraBlack
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(ExtraBlack))!;
    }

    public static FontWeight ExtraBold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(ExtraBold))!;
    }

    public static FontWeight ExtraLight
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(ExtraLight))!;
    }

    public static FontWeight Light
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Light))!;
    }

    public static FontWeight Medium
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Medium))!;
    }

    public static FontWeight Normal
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Normal))!;
    }

    public static FontWeight SemiBold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(SemiBold))!;
    }

    public static FontWeight SemiLight
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(SemiLight))!;
    }

    public static FontWeight Thin
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            _serverClassName,
            nameof(Thin))!;
    }
}
