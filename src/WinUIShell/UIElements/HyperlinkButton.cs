﻿using WinUIShell.Common;

namespace WinUIShell;

public class HyperlinkButton : ButtonBase
{
    public Uri NavigateUri
    {
        get => PropertyAccessor.Get<Uri>(Id, nameof(NavigateUri))!;
        set => PropertyAccessor.Set(Id, nameof(NavigateUri), value?.Id);
    }

    public HyperlinkButton()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.HyperlinkButton, Microsoft.WinUI",
            this);
    }
}
