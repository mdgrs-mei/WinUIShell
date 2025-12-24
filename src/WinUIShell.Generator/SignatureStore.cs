namespace WinUIShell.Generator;

internal class SignatureStore
{
    private readonly List<ObjectDef> _objectDefs = [];

    public void AddObjectDef(ObjectDef objectDef)
    {
        _objectDefs.Add(objectDef);
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
}
