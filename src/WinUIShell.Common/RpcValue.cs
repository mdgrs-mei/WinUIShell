
namespace WinUIShell.Common;

public class RpcValue
{
    public ObjectId? IdValue { get; set; }
    public bool? BoolValue { get; set; }
    public byte? ByteValue { get; set; }
    public int? IntValue { get; set; }
    public uint? UIntValue { get; set; }
    public double? DoubleValue { get; set; }
    public string? StringValue { get; set; }
    public RpcValue[]? ArrayValue { get; set; }

    public static bool IsSupportedType(object? obj)
    {
        if (obj is Array array)
        {
            foreach (var item in array)
            {
                if (!IsSupportedType(item))
                {
                    return false;
                }
            }
            return true;
        }

        return obj is null or
            ObjectId or
            bool or
            byte or
            int or
            uint or
            double or
            string or
            Enum;
    }

    public RpcValue(object? obj)
    {
        SetObject(obj);
    }

    private bool IsEnum()
    {
        return StringValue is not null && (IntValue is not null || UIntValue is not null);
    }

    public string? GetEnumTypeName()
    {
        if (IsEnum())
        {
            return StringValue;
        }
        return null;
    }

    public void SetObject(object? obj)
    {
        switch (obj)
        {
            case null:
                break;
            case ObjectId value:
                IdValue = value;
                break;
            case bool value:
                BoolValue = value;
                break;
            case byte value:
                ByteValue = value;
                break;
            case int value:
                IntValue = value;
                break;
            case uint value:
                UIntValue = value;
                break;
            case double value:
                DoubleValue = value;
                break;
            case string value:
                StringValue = value;
                break;
            case Enum value:
                {
                    var type = value.GetType();
                    var typeName = type.FullName;
                    var assemblyName = value.GetType().Assembly.GetName().Name;
                    StringValue = $"{typeName}, {assemblyName}";
                    if (Enum.GetUnderlyingType(type) == typeof(int))
                    {
                        IntValue = (int)obj;
                    }
                    else
                    {
                        UIntValue = (uint)obj;
                    }
                    break;
                }
            case Array array:
                {
                    ArrayValue = new RpcValue[array.Length];
                    for (int i = 0; i < array.Length; ++i)
                    {
                        ArrayValue[i] = new RpcValue(array.GetValue(i));
                    }
                    break;
                }
            default:
                throw new InvalidOperationException($"Unsupported RPC object type [{obj.GetType().Name}].");
        }
    }

    public object? GetObject()
    {
        if (IsEnum())
        {
            if (IntValue is not null)
            {
                return IntValue;
            }
            else
            {
                return UIntValue;
            }
        }
        if (IdValue is not null)
        {
            return IdValue;
        }
        if (BoolValue is not null)
        {
            return BoolValue;
        }
        if (ByteValue is not null)
        {
            return ByteValue;
        }
        if (IntValue is not null)
        {
            return IntValue;
        }
        if (UIntValue is not null)
        {
            return UIntValue;
        }
        if (DoubleValue is not null)
        {
            return DoubleValue;
        }
        if (StringValue is not null)
        {
            return StringValue;
        }
        if (ArrayValue is not null)
        {
            return ArrayValue;
        }
        return null;
    }
}
