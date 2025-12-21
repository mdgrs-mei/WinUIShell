namespace WinUIShell.Generator;

internal class SpecializedMethodGenerator
{
    public static bool Generate(CodeWriter codeWriter, MethodDef methodDef)
    {
        return Generate(codeWriter, methodDef, isInterfaceImpl: false);
    }

    public static bool GenerateForInterfaceImpl(CodeWriter codeWriter, MethodDef methodDef)
    {
        return Generate(codeWriter, methodDef, isInterfaceImpl: true);
    }

    private static bool Generate(CodeWriter codeWriter, MethodDef methodDef, bool isInterfaceImpl)
    {
        var methodName = methodDef.GetName();
        if (methodName.EndsWith("CopyTo") &&
            methodDef.Parameters.Count == 2 &&
            methodDef.Parameters[0].Type.IsArray &&
            methodDef.Parameters[1].Type.GetName() == "int")
        {
            var signature = isInterfaceImpl ?
                methodDef.GetInterfaceImplSignatureExpression() :
                methodDef.GetSignatureExpression();
            var arrayName = methodDef.Parameters[0].Name;
            var indexName = methodDef.Parameters[1].Name;

            codeWriter.Append($$"""
                {{signature}}
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
                
                    int i = 0;
                    foreach (var item in this)
                    {
                        {{arrayName}}[i + {{indexName}}] = item;
                        i++;
                    }
                }
                """);
            return true;
        }

        return false;
    }
}
