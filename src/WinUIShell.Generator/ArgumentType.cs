namespace WinUIShell.Generator;

internal class ArgumentType
{
    public string Name { get; internal set; } = "";
    public bool IsNullable { get; internal set; }

    public ArgumentType(Api.ArgumentType apiArgumentType)
    {
        var serverTypeName = apiArgumentType.Name;
        bool isReferenceType;

        if (serverTypeName.StartsWith("WinUIShell.Server"))
        {
            Name = serverTypeName.Replace("WinUIShell.Server", "WinUIShell");
            isReferenceType = !apiArgumentType.IsEnum;
        }
        else
        if (serverTypeName == "System.Object")
        {
            Name = "object";
            isReferenceType = true;
        }
        else
        {
            Dictionary<string, string> systemTypeMap = new()
            {
                { "System.Boolean", "bool" },
                { "System.Byte", "byte" },
                { "System.SByte", "sbyte" },
                { "System.Char", "char" },
                { "System.Decimal", "decimal" },
                { "System.Double", "double" },
                { "System.Single", "float" },
                { "System.Int32", "int" },
                { "System.UInt32", "uint" },
                { "System.Int64", "long" },
                { "System.UInt64", "ulong" },
                { "System.Int16", "short" },
                { "System.UInt16", "ushort" },
                { "System.String", "string" },
                { "System.Void", "void" }
            };

            if (systemTypeMap.TryGetValue(serverTypeName, out var typeName))
            {
                Name = typeName;
                isReferenceType = false;
            }
            else
            {
                Name = $"WinUIShell.{serverTypeName}";
                isReferenceType = !apiArgumentType.IsEnum;
            }
        }

        IsNullable = IsNullable || isReferenceType;
    }
}
