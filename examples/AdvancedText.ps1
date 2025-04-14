using namespace WinUIShell
Import-Module "$PSScriptRoot\..\module\WinUIShell"

$win = [Window]::new()
$win.Title = 'Advanced Text'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(600, 380)

$title = [TextBlock]::new()
$title.Text = 'This is a Title😉'
$title.HorizontalAlignment = 'Center'
$title.FontFamily = 'Comic Sans MS'
$title.FontSize = 36

$longString = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ac mi ipsum. Phasellus vel malesuada mauris. Donec pharetra, enim sit amet mattis tincidunt, felis nisi semper lectus, vel porta diam nisi in augue. Pellentesque lacus tortor, aliquam et faucibus id, rhoncus ut justo. Sed id lectus odio, eget pulvinar diam. Suspendisse eleifend ornare libero, in luctus purus aliquet non. Sed interdum, sem vitae rutrum rhoncus, felis ligula ultrices sem, in eleifend eros ante id neque.'
$label1 = [TextBlock]::new()
$label1.Text = 'Trimming'
$label1.TextDecorations = 'Underline'
$label1.FontWeight = [FontWeights]::Bold

$text1 = [TextBlock]::new()
$text1.Text = $longString
$text1.TextTrimming = 'CharacterEllipsis'

$label2 = [TextBlock]::new()
$label2.Text = 'Wrap'
$label2.TextDecorations = 'Underline'
$label2.FontWeight = [FontWeights]::Bold

$text2 = [TextBlock]::new()
$text2.Text = $longString + "`n" + $longString
$text2.TextWrapping = 'Wrap'
$text2.IsTextSelectionEnabled = $true
$scroll2 = [ScrollView]::new()
$scroll2.Content = $text2
$scroll2.Height = 110

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Spacing = 16
$panel.Children.Add($title)
$panel.Children.Add($label1)
$panel.Children.Add($text1)
$panel.Children.Add($label2)
$panel.Children.Add($scroll2)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
