using WinUIShell.Common;

namespace WinUIShell;

public class BitmapSource : ImageSource
{
    public int PixelHeight
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PixelHeight))!;
        set => PropertyAccessor.Set(Id, nameof(PixelHeight), value);
    }

    public int PixelWidth
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PixelWidth))!;
        set => PropertyAccessor.Set(Id, nameof(PixelWidth), value);
    }

    internal BitmapSource()
    {
    }

    internal BitmapSource(ObjectId id)
        : base(id)
    {
    }

    //public void SetSource(IRandomAccessStream streamSource)
    //public IAsyncAction SetSourceAsync(IRandomAccessStream streamSource)
}
