using WinUIShell.Common;

namespace WinUIShell;

public sealed class FontWeights
{
    private static readonly string s_serverClassName = ObjectTypeMapping.Get().GetTargetTypeName<FontWeights>();

    public static FontWeight Black
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Black))!;
    }

    public static FontWeight Bold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Bold))!;
    }

    public static FontWeight ExtraBlack
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(ExtraBlack))!;
    }

    public static FontWeight ExtraBold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(ExtraBold))!;
    }

    public static FontWeight ExtraLight
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(ExtraLight))!;
    }

    public static FontWeight Light
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Light))!;
    }

    public static FontWeight Medium
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Medium))!;
    }

    public static FontWeight Normal
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Normal))!;
    }

    public static FontWeight SemiBold
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(SemiBold))!;
    }

    public static FontWeight SemiLight
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(SemiLight))!;
    }

    public static FontWeight Thin
    {
        get => PropertyAccessor.GetStatic<FontWeight>(
            s_serverClassName,
            nameof(Thin))!;
    }
}
