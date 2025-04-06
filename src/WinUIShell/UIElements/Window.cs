using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class Window : WinUIShellObject
{
    private readonly EventCallbackList _closedCallbacks = new();
    private bool _isActivateCalled;
    private bool _isCloseCalled;
    private bool IsTerminated { get => _isActivateCalled && (_isCloseCalled || IsClosed); }
    internal bool IsClosed { get; set; } = true;

    public AppWindow AppWindow
    {
        get => PropertyAccessor.Get<AppWindow>(Id, nameof(AppWindow))!;
    }

    public UIElement? Content
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(Content));
        set => PropertyAccessor.Set(Id, nameof(Content), value?.Id);
    }

    public bool ExtendsContentIntoTitleBar
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(ExtendsContentIntoTitleBar));
        set => PropertyAccessor.Set(Id, nameof(ExtendsContentIntoTitleBar), value);
    }

    public SystemBackdrop? SystemBackdrop
    {
        get => PropertyAccessor.Get<SystemBackdrop>(Id, nameof(SystemBackdrop));
        set => PropertyAccessor.Set(Id, nameof(SystemBackdrop), value?.Id);
    }

    public string Title
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Title))!;
        set => PropertyAccessor.Set(Id, nameof(Title), value);
    }

    public Window()
    {
        Id = CommandClient.Get().CreateObject(
            "WinUIShell.Server.Window, WinUIShell.Server",
            this);
    }

    public void Activate()
    {
        if (IsTerminated)
            return;

        _isActivateCalled = true;
        IsClosed = false;
        ClearClosedCallbackStates();
        CommandClient.Get().InvokeMethod(Id, nameof(Activate));
    }

    public void AddClosed(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddClosed(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddClosed(EventCallback eventCallback)
    {
        _closedCallbacks.Add(
            "WinUIShell.Server.WindowAccessor, WinUIShell.Server",
            nameof(AddClosed),
            Id,
            eventCallback);
    }

    public void Close()
    {
        if (IsTerminated || IsClosed)
            return;

        _isCloseCalled = true;
        CommandClient.Get().InvokeMethod(Id, nameof(Close));
    }

    public void SetTitleBar(UIElement titleBar)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(SetTitleBar), titleBar?.Id);
    }

    public void WaitForClosed()
    {
        CommandQueueId queueId = new(Environment.CurrentManagedThreadId);
        while (!IsClosed || !IsAllClosedCallbacksInvoked())
        {
            CommandServer.Get().ProcessCommands(queueId);
            Thread.Sleep(8);
        }
    }

    internal void OnClosed(int eventId, WindowEventArgs eventArgs)
    {
        _closedCallbacks.Invoke(eventId, eventArgs);
    }

    private void ClearClosedCallbackStates()
    {
        _closedCallbacks.ClearIsInvoked();
    }

    private bool IsAllClosedCallbacksInvoked()
    {
        return _closedCallbacks.IsAllInvoked();
    }
}
