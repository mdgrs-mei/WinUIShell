using System.Reflection;
using System.Runtime.Loader;

namespace WinUIShell;

internal sealed class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private readonly string _directory;

    public CustomAssemblyLoadContext(string directory)
    {
        _directory = directory;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name is null)
        {
            return null;
        }

        var assemblyPath = Path.Combine(
            _directory,
            $"{assemblyName.Name}.dll");

        if (File.Exists(assemblyPath))
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }
}
