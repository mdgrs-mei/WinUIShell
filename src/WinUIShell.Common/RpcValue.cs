
namespace WinUIShell.Common;

public class RpcValue
{
    public ObjectId? IdValue { get; set; }
    public bool? BoolValue { get; set; }
    public char? CharValue { get; set; }

    public int Bytes { get; set; }
    public long? LongValue { get; set; }
    public ulong? ULongValue { get; set; }

    public float? FloatValue { get; set; }
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
            char or
            sbyte or
            short or
            int or
            long or
            byte or
            ushort or
            uint or
            ulong or
            float or
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
        return StringValue is not null && (LongValue is not null || ULongValue is not null);
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
            case char value:
                CharValue = value;
                break;

            case sbyte value:
                LongValue = value;
                Bytes = 1;
                break;
            case short value:
                LongValue = value;
                Bytes = 2;
                break;
            case int value:
                LongValue = value;
                Bytes = 4;
                break;
            case long value:
                LongValue = value;
                Bytes = 8;
                break;

            case byte value:
                ULongValue = value;
                Bytes = 1;
                break;
            case ushort value:
                ULongValue = value;
                Bytes = 2;
                break;
            case uint value:
                ULongValue = value;
                Bytes = 4;
                break;
            case ulong value:
                ULongValue = value;
                Bytes = 8;
                break;

            case float value:
                FloatValue = value;
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
                    var assemblyName = type.Assembly.GetName().Name;
                    StringValue = $"{typeName}, {assemblyName}";
                    if (Enum.GetUnderlyingType(type) == typeof(int))
                    {
                        LongValue = (int)obj;
                    }
                    else
                    {
                        ULongValue = (uint)obj;
                    }
                    Bytes = 4;
                    break;
                }
            case Array array:
                {
                    ArrayValue = RpcValueConverter.ConvertObjectArrayToRpcArray(array);
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
            if (LongValue is not null)
            {
                return (int)LongValue;
            }
            else
            {
                return (uint)ULongValue!;
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
        if (CharValue is not null)
        {
            return CharValue;
        }

        if (LongValue is not null)
        {
            return Bytes switch
            {
                1 => (sbyte)LongValue,
                2 => (short)LongValue,
                4 => (int)LongValue,
                8 => (object)LongValue,
                _ => throw new InvalidOperationException($"Invalid number of bytes [{Bytes}] for signed integer."),
            };
        }
        if (ULongValue is not null)
        {
            return Bytes switch
            {
                1 => (byte)ULongValue,
                2 => (ushort)ULongValue,
                4 => (uint)ULongValue,
                8 => (object)ULongValue,
                _ => throw new InvalidOperationException($"Invalid number of bytes [{Bytes}] for unsigned integer."),
            };
        }

        if (FloatValue is not null)
        {
            return FloatValue;
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
