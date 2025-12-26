namespace WinUIShell.Generator;

internal class SpecializedMethodGenerator
{
    public static bool Generate(CodeWriter codeWriter, MethodDef methodDef)
    {
        return Generate(codeWriter, methodDef, isInterfaceImpl: false, genericTypeParametersOverride: null, signatureStore: null);
    }

    public static bool GenerateForInterfaceImpl(
        CodeWriter codeWriter,
        MethodDef methodDef,
        List<TypeDef>? genericTypeParametersOverride,
        SignatureStore signatureStore)
    {
        return Generate(
            codeWriter,
            methodDef,
            isInterfaceImpl: true,
            genericTypeParametersOverride: genericTypeParametersOverride,
            signatureStore: signatureStore);
    }

    private static bool Generate(
        CodeWriter codeWriter,
        MethodDef methodDef,
        bool isInterfaceImpl,
        List<TypeDef>? genericTypeParametersOverride,
        SignatureStore? signatureStore)
    {
        var methodName = methodDef.GetName();
        if (methodName.EndsWith("CopyTo") &&
            methodDef.Parameters.Count == 2 &&
            methodDef.Parameters[0].Type.IsArray &&
            methodDef.Parameters[1].Type.GetName() == "int")
        {
            string signature;
            if (isInterfaceImpl && signatureStore != null)
            {
                bool isExplicit = signatureStore.ContainsSignature(methodDef);
                signature = methodDef.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride);
            }
            else
            {
                signature = methodDef.GetSignatureExpression();
            }

            var arrayName = methodDef.Parameters[0].Name;
            var indexName = methodDef.Parameters[1].Name;

            codeWriter.AppendAndReserveNewLine($$"""
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
