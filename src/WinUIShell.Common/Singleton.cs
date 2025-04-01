namespace WinUIShell.Common;

public abstract class Singleton<T> where T : class, new()
{
    private static T? s_instance;
    public static T Get()
    {
        Create();
        return s_instance!;
    }

    public static void Create()
    {
        if (s_instance is null)
        {
            s_instance = new T();
        }
    }

    public static void Destroy()
    {
        s_instance = null;
    }
}
