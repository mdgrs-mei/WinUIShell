using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed class WindowStore : Singleton<WindowStore>
{
    public sealed class WindowProperty
    {
        public bool IsTerminated { get; set; }
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
        lock (_windowProperties)
        {
            _windowProperties[window] = new();
        }
    }

    public WindowProperty GetWindowProperty(Window window)
    {
        lock (_windowProperties)
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
    }

    public void TerminateWindow(Window window)
    {
        lock (_windowProperties)
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
    }
}
