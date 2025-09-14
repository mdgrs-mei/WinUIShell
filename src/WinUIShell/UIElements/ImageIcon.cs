using WinUIShell.Common;

namespace WinUIShell;

public class ImageIcon : IconElement
{
    public ImageSource? Source
    {
        get => PropertyAccessor.Get<ImageSource>(Id, nameof(Source));
        set => PropertyAccessor.Set(Id, nameof(Source), value?.Id);
    }

    public ImageIcon()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ImageIcon>(),
            this);
    }

    internal ImageIcon(ObjectId id)
        : base(id)
    {
    }
}
