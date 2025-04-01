using WinUIShell.Common;

namespace WinUIShell;

internal static class PropertyAccessor
{
    public static T? Get<T>(ObjectId id, string propertyName)
    {
        try
        {
            return CommandClient.Get().GetProperty<T>(id, propertyName);
        }
        catch (Exception e)
        {
            // Exceptions in Property getters are not displayed by PowerShell.
            // Manually show them here.
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            throw;
        }
    }

    public static void Set(ObjectId id, string propertyName, object? value)
    {
        CommandClient.Get().SetProperty(id, propertyName, value);
    }

    public static void SetAndWait(ObjectId id, string propertyName, object? value)
    {
        CommandClient.Get().SetPropertyWait(id, propertyName, value);
    }

    public static void SetIndexer(ObjectId id, object index, object? value)
    {
        CommandClient.Get().SetIndexerProperty(id, index, value);
    }

    public static T? GetIndexer<T>(ObjectId id, object index)
    {
        try
        {
            return CommandClient.Get().GetIndexerProperty<T>(id, index);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            throw;
        }
    }

    public static T? GetStatic<T>(string className, string propertyName)
    {
        try
        {
            return CommandClient.Get().GetStaticProperty<T>(className, propertyName);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            throw;
        }
    }
}
