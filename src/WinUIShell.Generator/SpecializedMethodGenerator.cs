namespace WinUIShell.Generator;

internal class SpecializedMethodGenerator
{
    public static bool Generate(CodeWriter codeWriter, MethodDef methodDef)
    {
        var methodName = methodDef.GetName();
        if (methodName.EndsWith("CopyTo") &&
            methodDef.Parameters.Count == 2 &&
            methodDef.Parameters[0].Type.IsArray &&
            methodDef.Parameters[1].Type.GetName() == "int")
        {
            var arrayName = methodDef.Parameters[0].Name;
            var indexName = methodDef.Parameters[1].Name;
            codeWriter.Append($$"""
                {{methodDef.GetSignatureExpression()}}
                {
                    ArgumentNullException.ThrowIfNull({{arrayName}});
                    ArgumentOutOfRangeException.ThrowIfNegative({{indexName}});
                    if ({{arrayName}}.Length <= {{indexName}} && Count > 0)
                    {
                        throw new ArgumentOutOfRangeException("{{indexName}}");
                    }
                
                    if ({{arrayName}}.Length - {{indexName}} < Count)
                    {
                        throw new ArgumentException("Insufficient space to copy.");
                    }
                
                    int num = Count;
                    for (int i = 0; i < num; i++)
                    {
                        {{arrayName}}[i + {{indexName}}] = this[i];
                    }
                }
                """);
            return true;
        }

        return false;
    }
}
