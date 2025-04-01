﻿using WinUIShell.Common;

namespace WinUIShell;

public class Grid : Panel
{
    public BackgroundSizing BackgroundSizing
    {
        get => PropertyAccessor.Get<BackgroundSizing>(Id, nameof(BackgroundSizing))!;
        set => PropertyAccessor.Set(Id, nameof(BackgroundSizing), value);
    }

    //public Brush BorderBrush

    public Thickness BorderThickness
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(BorderThickness))!;
        set => PropertyAccessor.Set(Id, nameof(BorderThickness), value?.Id);
    }

    public ColumnDefinitionCollection ColumnDefinitions
    {
        get => PropertyAccessor.Get<ColumnDefinitionCollection>(Id, nameof(ColumnDefinitions))!;
    }

    public double ColumnSpacing
    {
        get => PropertyAccessor.Get<double>(Id, nameof(ColumnSpacing))!;
        set => PropertyAccessor.Set(Id, nameof(ColumnSpacing), value);
    }

    //public CornerRadius CornerRadius

    public Thickness Padding
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(Padding))!;
        set => PropertyAccessor.Set(Id, nameof(Padding), value?.Id);
    }

    public RowDefinitionCollection RowDefinitions
    {
        get => PropertyAccessor.Get<RowDefinitionCollection>(Id, nameof(RowDefinitions))!;
    }

    public double RowSpacing
    {
        get => PropertyAccessor.Get<double>(Id, nameof(RowSpacing))!;
        set => PropertyAccessor.Set(Id, nameof(RowSpacing), value);
    }

    public Grid()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            this);
    }

    public static int GetRow(FrameworkElement element)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<int>(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(GetRow),
            element?.Id);
    }

    public static void SetRow(FrameworkElement element, int value)
    {
        CommandClient.Get().InvokeStaticMethod(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(SetRow),
            element?.Id,
            value);
    }

    public static int GetColumn(FrameworkElement element)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<int>(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(GetColumn),
            element?.Id);
    }

    public static void SetColumn(FrameworkElement element, int value)
    {
        CommandClient.Get().InvokeStaticMethod(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(SetColumn),
            element?.Id,
            value);
    }

    public static int GetRowSpan(FrameworkElement element)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<int>(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(GetRowSpan),
            element?.Id);
    }

    public static void SetRowSpan(FrameworkElement element, int value)
    {
        CommandClient.Get().InvokeStaticMethod(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(SetRowSpan),
            element?.Id,
            value);
    }

    public static int GetColumnSpan(FrameworkElement element)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<int>(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(GetColumnSpan),
            element?.Id);
    }

    public static void SetColumnSpan(FrameworkElement element, int value)
    {
        CommandClient.Get().InvokeStaticMethod(
            "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI",
            nameof(SetColumnSpan),
            element?.Id,
            value);
    }
}
