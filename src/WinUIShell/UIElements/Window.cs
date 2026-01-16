using System.Management.Automation;
using WinUIShell.Common;
using WinUIShell.Generator;

namespace WinUIShell.Microsoft.UI.Xaml;

public partial class Window : IWinUIShellObject
{
    private const string _accessorClassName = "WinUIShell.Server.WindowAccessor, WinUIShell.Server";
    private readonly EventCallbackList _closedCallbacks = new();
    private readonly EventCallbackList _callbacks = new();
    private bool _isActivateCalled;
    private bool _isCloseCalled;
    private bool IsTerminated { get => _isActivateCalled && (_isCloseCalled || IsClosed); }

    private int _isClosed = 1;
    internal bool IsClosed
    {
        get => Interlocked.CompareExchange(ref _isClosed, 0, 0) > 0;
        private set => Interlocked.Exchange(ref _isClosed, value ? 1 : 0);
    }

    [SurpressGeneratorByName]
    public Window()
    {
        WinUIShellObjectId = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Window>(),
            this);

        CommandClient.Get().InvokeStaticMethod(_accessorClassName, "RegisterWindow", WinUIShellObjectId);
    }

    internal Window(ObjectId id)
    {
        WinUIShellObjectId = id;
        CommandClient.Get().InvokeStaticMethod(_accessorClassName, "RegisterWindow", WinUIShellObjectId);
    }

    [SurpressGeneratorByName]
    public void Activate()
    {
        if (IsTerminated)
            return;

        _isActivateCalled = true;
        IsClosed = false;
        CommandClient.Get().InvokeMethod(WinUIShellObjectId, nameof(Activate));
    }

    public void AddActivated(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddActivated(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddActivated(EventCallback eventCallback)
    {
        _callbacks.Add(
            WinUIShellObjectId,
            "Activated",
            ObjectTypeMapping.Get().GetTargetTypeName<WindowActivatedEventArgs>(),
            eventCallback);
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
            WinUIShellObjectId,
            "Closed",
            ObjectTypeMapping.Get().GetTargetTypeName<WindowEventArgs>(),
            eventCallback);
    }

    public void AddSizeChanged(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddSizeChanged(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddSizeChanged(EventCallback eventCallback)
    {
        _callbacks.Add(
            WinUIShellObjectId,
            "SizeChanged",
            ObjectTypeMapping.Get().GetTargetTypeName<WindowSizeChangedEventArgs>(),
            eventCallback);
    }

    public void AddVisibilityChanged(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddVisibilityChanged(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddVisibilityChanged(EventCallback eventCallback)
    {
        _callbacks.Add(
            WinUIShellObjectId,
            "VisibilityChanged",
            ObjectTypeMapping.Get().GetTargetTypeName<WindowVisibilityChangedEventArgs>(),
            eventCallback);
    }

    [SurpressGeneratorByName]
    public void Close()
    {
        if (IsTerminated || IsClosed)
            return;

        _isCloseCalled = true;
        CommandClient.Get().InvokeMethod(WinUIShellObjectId, nameof(Close));
    }

    public void WaitForClosed()
    {
        if (!_isActivateCalled)
            return;

        while (true)
        {
            if (IsClosed && IsAllClosedCallbacksInvoked())
                return;

            Engine.Get().UpdateRunspace();
            Thread.Sleep(Constants.ClientCommandPolingIntervalMillisecond);
        }
    }

    private bool IsAllClosedCallbacksInvoked()
    {
        return _closedCallbacks.IsAllInvoked();
    }
}
