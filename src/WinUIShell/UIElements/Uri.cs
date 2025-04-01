using System.Diagnostics.CodeAnalysis;
using WinUIShell.Common;

namespace WinUIShell;

public class Uri : WinUIShellObject
{
    public Uri([StringSyntax(StringSyntaxAttribute.Uri)] string uriString)
    {
        Id = CommandClient.Get().CreateObject(
            "System.Uri, System.Private.Uri",
            this,
            uriString);
    }

    internal Uri(ObjectId id)
        : base(id)
    {
    }
}
