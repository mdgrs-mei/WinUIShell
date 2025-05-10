using System.Collections.Concurrent;
using WinUIShell.Common;

namespace WinUIShell;

internal sealed class PageStore : Singleton<PageStore>
{
    private readonly ConcurrentDictionary<string, EventCallback> _loadedCallbacks = new();

    public void RegisterLoaded(string pageName, EventCallback callback)
    {
        _ = _loadedCallbacks.AddOrUpdate(pageName, callback, (key, oldValue) => callback);
    }

    public static void OnLoaded(string pageName)
    {
        Get().CallLoaded(pageName);
    }

    private void CallLoaded(string pageName)
    {
        if (_loadedCallbacks.TryGetValue(pageName, out EventCallback? callback))
        {
            callback.Invoke(null, null);
        }
    }
}
