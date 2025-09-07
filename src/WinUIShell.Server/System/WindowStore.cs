using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed class WindowStore : Singleton<WindowStore>
{
    public sealed class WindowProperty
    {
        public bool IsTerminated { get; set; }
        public int RunningEventCallbackCount { get; set; }
    }

    private sealed class Comparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }
        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    private readonly Dictionary<Window, WindowProperty> _windowProperties = new(new Comparer());

    public WindowStore()
    {
    }

    public void RegisterWindow(Window window)
    {
        _windowProperties[window] = new();
    }

    public WindowProperty GetWindowProperty(Window window)
    {
        if (_windowProperties.TryGetValue(window, out var property))
        {
            return property;
        }
        else
        {
            throw new InvalidOperationException($"Window not found {window}.");
        }
    }

    public void TerminateWindow(Window window)
    {
        if (_windowProperties.TryGetValue(window, out var property))
        {
            property.IsTerminated = true;
        }
        else
        {
            throw new InvalidOperationException($"Window not found {window}.");
        }
    }

    public Window? EnterEventCallbackAndGetParentWindow(object sender)
    {
        Window? parentWindow = GetParentWindow(sender);
        if (parentWindow is null)
            return null;

        var property = _windowProperties[parentWindow];
        lock (property)
        {
            property.RunningEventCallbackCount++;
        }
        return parentWindow;
    }

    public void ExitEventCallback(Window? parentWindow)
    {
        if (parentWindow is null)
            return;

        var property = GetWindowProperty(parentWindow);
        lock (property)
        {
            property.RunningEventCallbackCount--;
            Monitor.Pulse(property);
        }
    }

    public Window? GetParentWindow(object sender)
    {
        if (sender is Window w)
            return w;

        if (sender is UIElement uiElement)
        {
            if (uiElement.XamlRoot is null)
                return null;

            foreach (var window in _windowProperties.Keys)
            {
                var property = _windowProperties[window];
                // Check IsTerminated first to avoid crash on accessing window properties.
                if (property.IsTerminated)
                    continue;

                if (window.Content != uiElement.XamlRoot.Content)
                    continue;

                return window;
            }
        }
        return null;
    }

    public async Task WaitForAllChildEventCallbacksFinishedAsync(Window window)
    {
        var property = GetWindowProperty(window);

        await Task.Run(() =>
        {
            lock (property)
            {
                while (property.RunningEventCallbackCount > 0)
                {
                    _ = Monitor.Wait(property);
                }
            }
        });
    }
}
