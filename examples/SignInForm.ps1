using namespace WinUIShell
Import-Module "$PSScriptRoot\..\module\WinUIShell"

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.Title = 'Hello from PowerShell!'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(420, 420)

$icon = [SymbolIcon]::new([Symbol]::Globe)

$title = [TextBlock]::new()
$title.Text = 'Sign In'
$title.Style = $resources['TitleTextBlockStyle']

$titlePanel = [StackPanel]::new()
$titlePanel.Orientation = 'Horizontal'
$titlePanel.Spacing = 16
$titlePanel.Margin = [Thickness]::new(0, 0, 0, 24)
$titlePanel.Children.Add($icon)
$titlePanel.Children.Add($title)

$name = [TextBox]::new()
$name.Header = 'Account Name'
$name.PlaceHolderText = 'Your account name'
$name.Margin = [Thickness]::new(0, 0, 0, 24)

$password = [PasswordBox]::new()
$password.Header = 'Password'
$password.Description = 'Use a fake password. Do not enter your real password.'

$forgotPassword = [HyperlinkButton]::new()
$forgotPassword.Content = 'Forgot your password?'
$forgotPassword.NavigateUri = 'https://github.com/'
$forgotPassword.Margin = [Thickness]::new(0, 0, 0, 24)
$forgotPassword.Padding = [Thickness]::new(0, 5, 0, 6)

$status = [TextBlock]::new()
$status.Text = ''
$status.VerticalAlignment = 'Center'

$button = [Button]::new()
$button.HorizontalAlignment = 'Right'
$button.Content = 'Login'
$button.Style = $resources['AccentButtonStyle']
$buttonCallback = [EventCallback]@{
    ScriptBlock = {
        param ($argumentList, $s, $e)
        $status.Text = '{0} - Logging in...' -f $name.Text
        Start-Sleep -Milliseconds 3000
        $status.Text = 'Success!'
    }
    DisabledControlsWhileProcessing = @($button, $name, $password, $forgotPassword)
}
$button.AddClick($buttonCallback)

$buttonPanel = [StackPanel]::new()
$buttonPanel.Orientation = 'Horizontal'
$buttonPanel.Spacing = 16
$buttonPanel.horizontalAlignment = 'Right'
$buttonPanel.Children.Add($status)
$buttonPanel.Children.Add($button)

$panel = [StackPanel]::new()
$panel.Margin = 32

$panel.Children.Add($titlePanel)
$panel.Children.Add($name)
$panel.Children.Add($password)
$panel.Children.Add($forgotPassword)
$panel.Children.Add($buttonPanel)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
