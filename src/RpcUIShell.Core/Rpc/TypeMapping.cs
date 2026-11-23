namespace RpcUIShell.Core;

public sealed class TypeMapping : Singleton<TypeMapping>
{
    private readonly Dictionary<string, string> _map = [];
    private MappingDirection _direction = MappingDirection.ClientToServer;
    internal string ClientNamespace { get; set; } = "";

    public enum MappingDirection
    {
        ServerToClient,
        ClientToServer,
    };

    public void Init(
        MappingDirection direction,
        string clientNamespace)
    {
        _direction = direction;
        ClientNamespace = clientNamespace;
    }

    internal void Term()
    {
    }

    public void InitMapping(IList<(string, string)> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        foreach (var map in list)
        {
            _map.Add(map.Item1, map.Item2);
            _ = _map.TryAdd(map.Item2, map.Item1);
        }
    }

    public string GetTargetTypeName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        _ = TryGetTargetTypeName(type, out string? targetTypeName);
        if (targetTypeName is null)
        {
            throw new InvalidOperationException($"Object type mapping not found for [{type.FullName}].");
        }
        return targetTypeName;
    }

    internal bool TryGetTargetTypeName(Type sourceType, out string? targetTypeName)
    {
        var assemblyName = sourceType.Assembly.GetName().Name;
        if (sourceType.IsGenericType)
        {
            if (!TryGetValue($"{sourceType.FullName!.Split('[')[0]}, {assemblyName}", out string? thisName))
            {
                targetTypeName = null;
                return false;
            }

            string[] thisNameAndAssembly = thisName!.Split(", ");

            List<string> argNames = [];
            foreach (var argType in sourceType.GetGenericArguments())
            {
                if (!TryGetTargetTypeName(argType, out string? argTargetTypeName))
                {
                    targetTypeName = null;
                    return false;
                }
                argNames.Add($"[{argTargetTypeName}]");
            }

            targetTypeName = $"{thisNameAndAssembly[0]}[{string.Join(", ", argNames)}], {thisNameAndAssembly[1]}";
            return true;
        }
        else if (sourceType.IsEnum)
        {
            return TryGetValueEnum($"{sourceType.FullName}, {assemblyName}", out targetTypeName);
        }
        else
        {
            return TryGetValue($"{sourceType.FullName}, {assemblyName}", out targetTypeName);
        }
    }

    private bool TryGetValue(string sourceTypeName, out string? targetTypeName)
    {
        if (_direction == MappingDirection.ClientToServer && !sourceTypeName.StartsWith(ClientNamespace, StringComparison.Ordinal))
        {
            // If the client requests a type that is not in the client namespace, which means non-wrapper types like system types, return it as-is.
            targetTypeName = sourceTypeName;
            return true;
        }
        else
        {
            return _map.TryGetValue(sourceTypeName, out targetTypeName);
        }
    }

    public bool TryGetValueEnum(string sourceEnumName, out string? targetEnumName)
    {
        return _map.TryGetValue(sourceEnumName, out targetEnumName);
    }
}
