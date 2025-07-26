using namespace WinUIShell
if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

# Xaml example was taken from these blog posts:
#   Build your first WinUI 3 app (Part 1)
#     https://blogs.windows.com/windowsdeveloper/2022/01/28/build-your-first-winui-3-app-part-1/
#     https://github.com/jingwei-a-zhang/WinAppSDK-DrumPad/tree/UI_Layout
#
#   Adding event handlers: Sounds, Dark Mode & Windowing (Part 2)
#     https://blogs.windows.com/windowsdeveloper/2022/01/28/adding-event-handlers-sounds-dark-mode-windowing-part-2/
#     https://github.com/jingwei-a-zhang/WinAppSDK-DrumPad/tree/App_Logic

$xamlString = @'
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <Grid Margin="12">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            <DropDownButton Content="Display" Grid.Column="0" VerticalAlignment="Center" HorizontalAlignment="Left" Width="118" >
                <DropDownButton.Flyout>
                    <MenuFlyout x:Name="MenuFlyout" Placement="Bottom">
                        <MenuFlyoutItem Text="Default"/>
                        <MenuFlyoutItem Text="Compact Overlay"/>
                        <MenuFlyoutItem Text="Fullscreen"/>
                    </MenuFlyout>
                </DropDownButton.Flyout>
            </DropDownButton>

            <ToggleSwitch AutomationProperties.Name="simple ToggleSwitch" x:Name="dark_switch" Grid.Column="1" CornerRadius="3" VerticalAlignment="Center"  HorizontalAlignment="Right" MinWidth="0" HorizontalContentAlignment="Center" VerticalContentAlignment="Center" />
        </Grid>

        <Grid x:Name="Control1" Grid.Row="1" ColumnSpacing="30" RowSpacing="12"
          VerticalAlignment="Center" HorizontalAlignment="Center">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>

            <Grid.RowDefinitions>
                <RowDefinition Height="*" />
                <RowDefinition Height="*" />
                <RowDefinition Height="*" />
                <RowDefinition Height="*" />
            </Grid.RowDefinitions>

            <Grid.Resources>
                <Style TargetType="Button">
                    <Setter Property="HorizontalAlignment" Value="Stretch" />
                    <Setter Property="VerticalAlignment" Value="Stretch" />
                    <Setter Property="CornerRadius" Value="5" />
                    <Setter Property="Foreground" Value="White" />
                    <Setter Property="FontSize" Value="36" />
                </Style>
            </Grid.Resources>

            <Button Background="#FF0000" Grid.Row="0" Grid.Column="0" Content="Kick 1"/>
            <Button Background="#C4425A" Grid.Row="0" Grid.Column="1" Content="Kick 2"/>
            <Button Background="#D41DD8" Grid.Row="0" Grid.Column="2" Content="Kick 3"/>
            <Button Background="#6A21A3" Grid.Row="0" Grid.Column="3" Content="Kick 4"/>

            <Button Background="#C4425A" Grid.Row="1" Grid.Column="0" Content="Clap 1"/>
            <Button Background="#D41DD8" Grid.Row="1" Grid.Column="1" Content="Clap 2"/>
            <Button Background="#6A21A3" Grid.Row="1" Grid.Column="2" Content="Clap 3"/>
            <Button Background="#473DB8" Grid.Row="1" Grid.Column="3" Content="Clap 4"/>

            <Button Background="#D41DD8" Grid.Row="2" Grid.Column="0" Content="Open Hat 1"/>
            <Button Background="#6A21A3" Grid.Row="2" Grid.Column="1" Content="Open Hat 2"/>
            <Button Background="#473DB8" Grid.Row="2" Grid.Column="2" Content="Open Hat 3"/>
            <Button Background="#26AAC7" Grid.Row="2" Grid.Column="3" Content="Open Hat 4"/>

            <Button Background="#6A21A3" Grid.Row="3" Grid.Column="0" Content="Snare 1"/>
            <Button Background="#473DB8" Grid.Row="3" Grid.Column="1" Content="Snare 2"/>
            <Button Background="#26AAC7" Grid.Row="3" Grid.Column="2" Content="Snare 3"/>
            <Button Background="#1A7BD6" Grid.Row="3" Grid.Column="3" Content="Snare 4"/>
        </Grid>
    </Grid>

</Window>
'@

$win = [XamlReader]::Load($xamlString)
$win.AppWindow.ResizeClient(1200, 600)

# Since only FrameworkElement has the FindName function and Window does not have it, we use the root Grid object.
$rootGrid = $win.Content

$menuFlyout = $rootGrid.FindName('MenuFlyout')
foreach ($flyoutItem in $menuFlyout.Items) {
    $flyoutItem.AddClick({
            param($argumentList, $s, $e)
            $flyoutItem = $s

            switch ($flyoutItem.Text) {
                'Compact Overlay' {
                    $presenter = [CompactOverlayPresenter]::new()
                }
                'Fullscreen' {
                    $presenter = [FullScreenPresenter]::new()
                }
                default {
                    $presenter = [OverlappedPresenter]::new()
                    $win.AppWindow.ResizeClient(1200, 600)
                }
            }
            $win.AppWindow.SetPresenter($presenter)
        })
}

$rootGrid.RequestedTheme = 'Dark'
$toggle = $rootGrid.FindName('dark_switch')
$toggle.AddToggled({
        if ($toggle.IsOn) {
            $rootGrid.RequestedTheme = 'Light'
        } else {
            $rootGrid.RequestedTheme = 'Dark'
        }
    })

$buttonGrid = $rootGrid.FindName('Control1')
foreach ($button in $buttonGrid.Children) {
    $button.AddClick({
            param($argumentList, $s, $e)
            $button = $s
            $button.Content = 'Clicked'
        })
}

$win.Activate()
$win.WaitForClosed()
