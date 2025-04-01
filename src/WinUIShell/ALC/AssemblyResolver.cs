using System.Management.Automation;
using System.Reflection;
using System.Runtime.Loader;

namespace WinUIShell;
public class AssemblyResolver : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    private static readonly CustomAssemblyLoadContext _alc = CreateAssemblyLoadContext();

    private static CustomAssemblyLoadContext CreateAssemblyLoadContext()
    {
        string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

        var dependencyDirPath = Path.GetFullPath(
            Path.Combine(
                assemblyDir,
                "Dependencies"));

        return new CustomAssemblyLoadContext(dependencyDirPath);
    }

    public void OnImport()
    {
        AssemblyLoadContext.Default.Resolving += Resolve;
    }

    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        AssemblyLoadContext.Default.Resolving -= Resolve;
    }

    private static Assembly? Resolve(AssemblyLoadContext defaultAlc, AssemblyName assemblyToResolve)
    {
        if (assemblyToResolve.Name is null)
        {
            return null;
        }
        if (!assemblyToResolve.Name.Equals("WinUIShell.Common", StringComparison.Ordinal))
        {
            return null;
        }
        return _alc.LoadFromAssemblyName(assemblyToResolve);
    }
}
