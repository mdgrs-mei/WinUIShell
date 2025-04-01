

namespace WinUIShell.Common;

public class ObjectIdGenerator : Singleton<ObjectIdGenerator>
{
    private uint _id;
    private string _prefix = "";

    public void SetPrefix(string prefix)
    {
        _prefix = prefix;
    }

    public ObjectId Generate()
    {
        var id = Interlocked.Increment(ref _id);
        return new ObjectId($"{_prefix}{id}");
    }
}
