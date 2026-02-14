namespace WinUIShell.Generator;

internal class SignatureStore
{
    private readonly List<ObjectDef> _objectDefs = [];
    private readonly List<string> _strings = [];

    public void AddObjectDef(ObjectDef objectDef)
    {
        _objectDefs.Add(objectDef);
    }

    public void AddString(string str)
    {
        _strings.Add(str);
    }

    public bool ContainsObject(ObjectDef objectDef)
    {
        return _objectDefs.Contains(objectDef);
    }

    public bool ContainsSignature(PropertyDef propertyDef)
    {
        foreach (var objectDef in _objectDefs)
        {
            if (objectDef.ContainsSignature(propertyDef))
                return true;
        }
        return false;
    }

    public bool ContainsSignature(MethodDef methodDef)
    {
        foreach (var objectDef in _objectDefs)
        {
            if (objectDef.ContainsSignature(methodDef))
                return true;
        }
        return false;
    }

    public bool ContainsSignature(EventDef eventDef)
    {
        foreach (var objectDef in _objectDefs)
        {
            if (objectDef.ContainsSignature(eventDef))
                return true;
        }
        return false;
    }

    public bool ContainsString(string str)
    {
        return _strings.Contains(str);
    }
}
