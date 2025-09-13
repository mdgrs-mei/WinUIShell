using WinUIShell.Common;

namespace WinUIShell;

public class BitmapImage : BitmapSource
{
    public bool AutoPlay
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AutoPlay))!;
        set => PropertyAccessor.Set(Id, nameof(AutoPlay), value);
    }

    public BitmapCreateOptions CreateOptions
    {
        get => PropertyAccessor.Get<BitmapCreateOptions>(Id, nameof(CreateOptions))!;
        set => PropertyAccessor.Set(Id, nameof(CreateOptions), value);
    }

    public int DecodePixelHeight
    {
        get => PropertyAccessor.Get<int>(Id, nameof(DecodePixelHeight))!;
        set => PropertyAccessor.Set(Id, nameof(DecodePixelHeight), value);
    }

    public DecodePixelType DecodePixelType
    {
        get => PropertyAccessor.Get<DecodePixelType>(Id, nameof(DecodePixelType))!;
        set => PropertyAccessor.Set(Id, nameof(DecodePixelType), value);
    }

    public int DecodePixelWidth
    {
        get => PropertyAccessor.Get<int>(Id, nameof(DecodePixelWidth))!;
        set => PropertyAccessor.Set(Id, nameof(DecodePixelWidth), value);
    }

    public bool IsAnimatedBitmap
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsAnimatedBitmap))!;
        set => PropertyAccessor.Set(Id, nameof(IsAnimatedBitmap), value);
    }

    public bool IsPlaying
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPlaying))!;
        set => PropertyAccessor.Set(Id, nameof(IsPlaying), value);
    }

    public Uri UriSource
    {
        get => PropertyAccessor.Get<Uri>(Id, nameof(UriSource))!;
        set => PropertyAccessor.Set(Id, nameof(UriSource), value);
    }

    public BitmapImage()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<BitmapImage>(),
            this);
    }

    public BitmapImage(Uri uriSource)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<BitmapImage>(),
            this,
            uriSource);
    }

    internal BitmapImage(ObjectId id)
        : base(id)
    {
    }

    //public event DownloadProgressEventHandler DownloadProgress
    //public event ExceptionRoutedEventHandler ImageFailed
    //public event RoutedEventHandler ImageOpened

}
