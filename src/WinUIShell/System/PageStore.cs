﻿using System.Collections.Concurrent;
using WinUIShell.Common;

namespace WinUIShell;

internal sealed class PageStore : Singleton<PageStore>
{
    private readonly ConcurrentDictionary<string, EventCallback> _loadedCallbacks = new();

    public void RegisterLoaded(string pageName, EventCallback callback)
    {
        var copiedCallback = callback.Copy();
        _ = _loadedCallbacks.AddOrUpdate(pageName, copiedCallback, (key, oldValue) => copiedCallback);
    }

    public static void OnLoaded(string pageName, Page page, RoutedEventArgs eventArgs)
    {
        Get().CallLoaded(pageName, page, eventArgs);
    }

    private void CallLoaded(string pageName, Page page, RoutedEventArgs eventArgs)
    {
        if (_loadedCallbacks.TryGetValue(pageName, out EventCallback? callback))
        {
            callback.Invoke(page, eventArgs);
        }
    }
}
