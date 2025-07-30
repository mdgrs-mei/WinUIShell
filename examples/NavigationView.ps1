using namespace WinUIShell
if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.Title = 'Navigation View'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.TitleBar.PreferredTheme = 'UseDefaultAppMode'
$win.AppWindow.ResizeClient(1000, 520)

$frame = [Frame]::new()
$navigationView = [NavigationView]::new()
$navigationView.Content = $frame
$navigationView.PaneTitle = 'Menu'
$navigationView.ExpandedModeThresholdWidth = 800
$navigationView.CompactModeThresholdWidth = 400

$contentPageOnLoaded = {
    param ($pageName, $page, $e)
    if ($page.Content) {
        # Cached instance is used so we reuse the content.
        return
    }

    $title = [TextBlock]::new()
    $title.Text = "This is $pageName"
    $title.Style = $resources['TitleTextBlockStyle']

    $text = [TextBlock]::new()
    $text.Text = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ac mi ipsum. Phasellus vel malesuada mauris. Donec pharetra, enim sit amet mattis tincidunt, felis nisi semper lectus, vel porta diam nisi in augue. Pellentesque lacus tortor, aliquam et faucibus id, rhoncus ut justo. Sed id lectus odio, eget pulvinar diam. Suspendisse eleifend ornare libero, in luctus purus aliquet non. Sed interdum, sem vitae rutrum rhoncus, felis ligula ultrices sem, in eleifend eros ante id neque.'
    $text.Style = $resources['BodyTextBlockStyle']

    $button = [Button]::new()
    $button.HorizontalAlignment = 'Right'
    $button.Content = 'Click Me'
    $button.Style = $resources['AccentButtonStyle']

    $panel = [StackPanel]::new()
    $panel.Margin = 32
    $panel.Spacing = 16
    $panel.Children.Add($title)
    $panel.Children.Add($text)
    $panel.Children.Add($button)

    $page.Content = $panel
}

$settingsPageOnLoaded = {
    param ($pageName, $page, $e)
    if ($page.Content) {
        return
    }

    $toggle1 = [ToggleSwitch]::new()
    $toggle1.IsOn = $true
    $toggle1.Header = 'Badge Notification'

    $toggle2 = [ToggleSwitch]::new()
    $toggle2.Header = 'Banner Notification'

    $toggle3 = [ToggleSwitch]::new()
    $toggle3.Header = 'Brightness Control'

    $panel = [StackPanel]::new()
    $panel.Margin = 32
    $panel.Spacing = 16
    $panel.Children.Add($toggle1)
    $panel.Children.Add($toggle2)
    $panel.Children.Add($toggle3)

    $page.Content = $panel
}

function Navigate($pageName, $recommendedNavigationTransitionInfo) {
    if ($frame.SourcePageName -eq $pageName) {
        return
    }

    $onLoaded = if ($pageName -eq 'Settings') { $settingsPageOnLoaded } else { $contentPageOnLoaded }
    $onLoadedArgumentList = $pageName

    # Page instance is created only once per page name. Cached pages are used from the second navigation.
    $cacheMode = [NavigationCacheMode]::Enabled

    # You can change the transition animation by setting [DrillInNavigationTransitionInfo]::new() etc.
    $transition = $recommendedNavigationTransitionInfo

    # Page instance is created internally and onLoaded script block is called.
    $frame.Navigate($pageName, $transition, $cacheMode, $onLoaded, $onLoadedArgumentList) | Out-Null
}

# Called when NavigationViewItem is clicked.
$navigationView.AddItemInvoked({
        param($argumentList, $s, $e)

        if ($e.IsSettingsInvoked) {
            $pageName = 'Settings'
        } else {
            $pageName = $e.InvokedItemContainer.Tag
        }

        Navigate $pageName $e.RecommendedNavigationTransitionInfo
    })

# Called when Back button is clicked.
$navigationView.AddBackRequested({
        if (-not ($frame.CanGoBack)) {
            return
        }
        $frame.GoBack()
    })

$frame.AddNavigated({
        param($argumentList, $s, $e)

        # Property accesses are slow as they require communication with the server.
        # Use temporary variables to keep property access as few as possible.
        $pageName = $frame.SourcePageName

        # Navigation also happens when back button is pressed.
        # That's why we set these properties here instead of in the ItemInvoked handler.
        $navigationView.IsBackEnabled = $frame.CanGoBack
        $navigationView.Header = $pageName

        if ($pageName -eq 'Settings') {
            $menuItem = $navigationView.SettingsItem
        } else {
            $menuItem = $menuItemMap[$pageName]
        }
        $navigationView.SelectedItem = $menuItem
    })


$separator = [NavigationViewItemSeparator]::new()
$navigationView.MenuItems.Add($separator)

$menuItemMap = @{}
$item1 = [NavigationViewItem]::new()
$item1.Content = 'Home'
$item1.Icon = [SymbolIcon]::new('Home')
$item1.Tag = 'Home'
$menuItemMap[$item1.Tag] = $item1
$navigationView.MenuItems.Add($item1)

$item2 = [NavigationViewItem]::new()
$item2.Content = 'Favorite'
$item2.Icon = [SymbolIcon]::new('Favorite')
$item2.Tag = 'Favorite'
$menuItemMap[$item2.Tag] = $item2
$navigationView.MenuItems.Add($item2)

$header = [NavigationViewItemHeader]::new()
$header.Content = 'Actions'
$navigationView.MenuItems.Add($header)

$item3 = [NavigationViewItem]::new()
$item3.Content = 'Mail'
$item3.Icon = [SymbolIcon]::new('Mail')
$item3.Tag = 'Mail'
$menuItemMap[$item3.Tag] = $item3
$navigationView.MenuItems.Add($item3)

$item4 = [NavigationViewItem]::new()
$item4.Content = 'Chat'
$item4.Icon = [SymbolIcon]::new('Message')
$item4.Tag = 'Chat'
$menuItemMap[$item4.Tag] = $item4
$navigationView.MenuItems.Add($item4)

$win.Content = $navigationView
$win.Activate()
Navigate 'Home' $null
$win.WaitForClosed()
