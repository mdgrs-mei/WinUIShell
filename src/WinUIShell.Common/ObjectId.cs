
namespace WinUIShell.Common;

public class ObjectId
{
    public string Id { get; set; } = "";

    public ObjectId()
    {
    }

    public ObjectId(string id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id;
    }
}
