using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class Window : WinUIShellObject
{
    private const string _accessorClassName = "WinUIShell.Server.WindowAccessor, WinUIShell.Server";
    private readonly EventCallbackList _closedCallbacks = new();
    private bool _isActivateCalled;
    private bool _isCloseCalled;
    private bool IsTerminated { get => _isActivateCalled && (_isCloseCalled || IsClosed); }

    private int _isClosed = 1;
    internal bool IsClosed
    {
        get => Interlocked.CompareExchange(ref _isClosed, 0, 0) > 0;
        private set => Interlocked.Exchange(ref _isClosed, value ? 1 : 0);
    }

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
            ObjectTypeMapping.Get().GetTargetTypeName<Window>(),
            this);

        CommandClient.Get().InvokeStaticMethod(_accessorClassName, "RegisterWindow", Id);
    }

    internal Window(ObjectId id)
        : base(id)
    {
        CommandClient.Get().InvokeStaticMethod(_accessorClassName, "RegisterWindow", Id);
    }

    public void Activate()
    {
        if (IsTerminated)
            return;

        _isActivateCalled = true;
        IsClosed = false;
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
            Id,
            "Closed",
            ObjectTypeMapping.Get().GetTargetTypeName<WindowEventArgs>(),
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
        while (!IsClosed || !IsAllClosedCallbacksInvoked())
        {
            Engine.UpdateRunspace();
            Thread.Sleep(Constants.ClientCommandPolingIntervalMillisecond);
        }
    }

    private bool IsAllClosedCallbacksInvoked()
    {
        return _closedCallbacks.IsAllInvoked();
    }
}
