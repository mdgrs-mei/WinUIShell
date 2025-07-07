using System.Diagnostics.CodeAnalysis;
using WinUIShell.Common;

namespace WinUIShell;

public class Uri : WinUIShellObject
{
    public Uri([StringSyntax(StringSyntaxAttribute.Uri)] string uriString)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Uri>(),
            this,
            uriString);
    }

    internal Uri(ObjectId id)
        : base(id)
    {
    }
}
