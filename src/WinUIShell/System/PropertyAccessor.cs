using WinUIShell.Common;

namespace WinUIShell;

internal static class PropertyAccessor
{
    public static TReturn? Get<TReturn, TCreate>(ObjectId id, string propertyName)
    {
        try
        {
            return CommandClient.Get().GetProperty<TReturn, TCreate>(id, propertyName);
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

    public static void SetIndexer(ObjectId id, string indexerName, object? value, params object?[] indexArguments)
    {
        CommandClient.Get().SetIndexerProperty(id, indexerName, value, indexArguments);
    }

    public static TReturn? GetIndexer<TReturn, TCreate>(ObjectId id, string indexerName, params object?[] indexArguments)
    {
        try
        {
            return CommandClient.Get().GetIndexerProperty<TReturn, TCreate>(id, indexerName, indexArguments);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            throw;
        }
    }

    public static void SetStatic(string className, string propertyName, object? value)
    {
        CommandClient.Get().SetStaticProperty(className, propertyName, value);
    }

    public static TReturn? GetStatic<TReturn, TCreate>(string className, string propertyName)
    {
        try
        {
            return CommandClient.Get().GetStaticProperty<TReturn, TCreate>(className, propertyName);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            throw;
        }
    }
}
