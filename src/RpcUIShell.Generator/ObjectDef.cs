using System.Text;
using RpcUIShell.Core;

namespace RpcUIShell.Generator;

internal class ObjectDef
{
    private readonly Api.ObjectDef _apiObjectDef;

    private readonly List<PropertyDef> _staticProperties = [];
    private readonly List<PropertyDef> _instanceProperties = [];
    private readonly List<MethodDef> _constructors = [];
    private readonly List<MethodDef> _staticMethods = [];
    private readonly List<MethodDef> _instanceMethods = [];
    private readonly List<EventDef> _staticEvents = [];
    private readonly List<EventDef> _instanceEvents = [];
    private readonly List<ObjectDef> _nestedObjects = [];

    private Dictionary<string, GetterSetter>? _explicitInterfaceImplementationGetterSetters;

    public TypeDef Type { get; }
    public TypeDef? BaseType { get; }
    public List<TypeDef> Interfaces = [];

    public ObjectDef(Api.ObjectDef apiObjectDef)
    {
        _apiObjectDef = apiObjectDef;

        Type = new TypeDef(_apiObjectDef.Type);
        Type.AlwaysReturnGlobalSystemInterfaceName = Type.IsGlobalSystemInterface;

        if (_apiObjectDef.BaseType is not null)
        {
            BaseType = new TypeDef(_apiObjectDef.BaseType);
        }

        if (_apiObjectDef.Interfaces is not null)
        {
            foreach (var interfaceType in _apiObjectDef.Interfaces)
            {
                Interfaces.Add(new TypeDef(interfaceType));
            }
        }
        if (_apiObjectDef.StaticProperties is not null)
        {
            foreach (var property in _apiObjectDef.StaticProperties)
            {
                _staticProperties.Add(new PropertyDef(property, this, MemberDefType.Static));
            }
        }
        if (_apiObjectDef.InstanceProperties is not null)
        {
            foreach (var property in _apiObjectDef.InstanceProperties)
            {
                _instanceProperties.Add(new PropertyDef(property, this, MemberDefType.Instance));
            }
        }
        if (_apiObjectDef.Constructors is not null)
        {
            foreach (var constructor in _apiObjectDef.Constructors)
            {
                _constructors.Add(new MethodDef(constructor, this, MemberDefType.Constructor));
            }
        }
        if (_apiObjectDef.StaticMethods is not null)
        {
            foreach (var method in _apiObjectDef.StaticMethods)
            {
                _staticMethods.Add(new MethodDef(method, this, MemberDefType.Static));
            }
        }

        InitExplicitInterfaceImplementationGetterSetters();

        if (_apiObjectDef.InstanceMethods is not null)
        {
            foreach (var method in _apiObjectDef.InstanceMethods)
            {
                if (IsExplicitInterfaceImplementationGetterSetter(method))
                    continue;
                _instanceMethods.Add(new MethodDef(method, this, MemberDefType.Instance));
            }
        }
        if (_apiObjectDef.StaticEvents is not null)
        {
            foreach (var eventDef in _apiObjectDef.StaticEvents)
            {
                _staticEvents.Add(new EventDef(eventDef, this, MemberDefType.Static));
            }
        }
        if (_apiObjectDef.InstanceEvents is not null)
        {
            foreach (var eventDef in _apiObjectDef.InstanceEvents)
            {
                _instanceEvents.Add(new EventDef(eventDef, this, MemberDefType.Instance));
            }
        }
        if (_apiObjectDef.NestedTypes is not null)
        {
            foreach (var nestedType in _apiObjectDef.NestedTypes)
            {
                _nestedObjects.Add(new ObjectDef(nestedType));
            }
        }
    }

    private class GetterSetter
    {
        public Api.MethodDef? Getter { get; set; }
        public Api.MethodDef? Setter { get; set; }
        public string PropertyName { get; set; } = "";
    }

    // Depending on the compiler, some classes have "InterfaceName.get_PropertyName" and "InterfaceName.set_PropertyName" methods
    // instead of explicitly implemented properties. Look for those methods and turn them into properties.
    private void InitExplicitInterfaceImplementationGetterSetters()
    {
        if (_apiObjectDef.InstanceMethods is null)
            return;

        Dictionary<string, GetterSetter>? getterSetters = null;
        foreach (var method in _apiObjectDef.InstanceMethods)
        {
            if (method.ExplicitInterfaceType is null)
                continue;

            bool isGetter = (method.Name is not null) && method.Name.StartsWith("get_");
            bool isSetter = (method.Name is not null) && method.Name.StartsWith("set_");

            if (!isGetter && !isSetter)
                continue;

            string propertyName = "";
            if (isGetter)
            {
                propertyName = method.Name!.Substring("get_".Length);
            }
            else if (isSetter)
            {
                propertyName = method.Name!.Substring("set_".Length);
            }

            if (getterSetters is null)
            {
                getterSetters = [];
            }

            string interfaceName = method.ExplicitInterfaceType.Name;
            string id = $"{interfaceName}.{propertyName}";
            if (!getterSetters.TryGetValue(id, out var getterSetter))
            {
                getterSetter = new GetterSetter
                {
                    PropertyName = propertyName
                };
                getterSetters.Add(id, getterSetter);
            }

            if (isGetter)
            {
                getterSetter.Getter = method;
            }
            else if (isSetter)
            {
                getterSetter.Setter = method;
            }
        }

        _explicitInterfaceImplementationGetterSetters = getterSetters;

        if (_explicitInterfaceImplementationGetterSetters is not null)
        {
            foreach (var getterSetter in _explicitInterfaceImplementationGetterSetters.Values)
            {
                _instanceProperties.Add(new PropertyDef(
                    getterSetter.PropertyName,
                    getterSetter.Getter,
                    getterSetter.Setter,
                    this,
                    MemberDefType.Instance));
            }
        }
    }

    private bool IsExplicitInterfaceImplementationGetterSetter(Api.MethodDef apiMethodDef)
    {
        if (_explicitInterfaceImplementationGetterSetters is null)
            return false;

        foreach (var getterSetter in _explicitInterfaceImplementationGetterSetters.Values)
        {
            if (apiMethodDef == getterSetter.Getter || apiMethodDef == getterSetter.Setter)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSupported()
    {
        return Type.IsSupported() && !Type.IsRpcSupportedType && !Type.IsObject;
    }

    public bool ContainsSignature(PropertyDef propertyDef)
    {
        string signature = propertyDef.GetSignatureId();
        foreach (var property in _staticProperties)
        {
            if (property.GetSignatureId() == signature)
            {
                return true;
            }
        }
        foreach (var property in _instanceProperties)
        {
            if (property.GetSignatureId() == signature)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsSignature(MethodDef methodDef)
    {
        string signature = methodDef.GetSignatureId();
        foreach (var method in _constructors)
        {
            if (method.GetSignatureId() == signature)
            {
                return true;
            }
        }
        foreach (var method in _staticMethods)
        {
            if (method.GetSignatureId() == signature)
            {
                return true;
            }
        }
        foreach (var method in _instanceMethods)
        {
            if (method.GetSignatureId() == signature)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsSignature(EventDef eventDef)
    {
        string signature = eventDef.GetSignatureId();
        foreach (var e in _staticEvents)
        {
            if (e.GetSignatureId() == signature)
            {
                return true;
            }
        }
        foreach (var e in _instanceEvents)
        {
            if (e.GetSignatureId() == signature)
            {
                return true;
            }
        }
        return false;
    }

    public string GetSourceCodeFileName()
    {
        var fullName = _apiObjectDef.FullName.Split(',')[0];
        return $"{Generator.Api!.ModuleName}.{fullName}.g.cs";
    }

    public string Generate()
    {
        var codeWriter = new CodeWriter();

        var ns = Generator.GetTargetNamespace(_apiObjectDef.Namespace);
        codeWriter.Append($$"""
            // <auto-generated/>
            #nullable enable

            using System.Management.Automation;
            using {{Generator.Api!.ModuleName}};
            using RpcUIShell.Core;

            namespace {{ns}};

            """);

        Generate(codeWriter);
        return codeWriter.ToString();
    }

    private void Generate(CodeWriter codeWriter)
    {
        if (Type.IsInterface)
        {
            GenerateInterface(codeWriter);
            GenerateInterfaceImpl(codeWriter);
        }
        else
        {
            GenerateClass(codeWriter);
        }
    }

    private void GenerateInterface(CodeWriter codeWriter)
    {
        string genericArgumentsExpression = Type.GetGenericArgumentsExpression();
        StringBuilder baseTypeExpression = new();
        foreach (var interfaceType in Interfaces)
        {
            if (interfaceType.IsSupported())
            {
                if (baseTypeExpression.Length == 0)
                {
                    _ = baseTypeExpression.Append(" : ");
                }
                else
                {
                    _ = baseTypeExpression.Append(", ");
                }
                _ = baseTypeExpression.Append(interfaceType.GetName());
            }
        }

        if (baseTypeExpression.Length == 0)
        {
            _ = baseTypeExpression.Append($" : {Generator.Api!.ObjectInterfaceName}");
        }

        if (Type.IsGlobalSystemInterface)
        {
            _ = baseTypeExpression.Append($", {Type.GetGlobalSystemInterfaceName()}");
        }

        codeWriter.Append($$"""
            public partial interface {{_apiObjectDef.Name}}{{genericArgumentsExpression}}{{baseTypeExpression}}
            {
            """);

        if (Type.IsGlobalSystemInterface)
        {
            codeWriter.IncrementIndent();
            foreach (var method in _instanceMethods)
            {
                if (AttributeGenerator.IsSurpressed(method))
                    continue;

                _ = SpecializedMethodGenerator.Generate(codeWriter, method);
            }
            codeWriter.DecrementIndent();
        }
        else
        {
            codeWriter.IncrementIndent();

            foreach (var method in _constructors)
            {
                if (!method.IsSupported() || AttributeGenerator.IsSurpressed(method))
                    continue;

                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetConstructorSignatureExpression(_apiObjectDef.Name)}}
                    """);
            }

            foreach (var property in _staticProperties)
            {
                if (!property.IsSupported() || AttributeGenerator.IsSurpressed(property))
                    continue;

                codeWriter.Append($$"""
                    {{property.GetSignatureExpression()}}
                    {
                    """);
                codeWriter.IncrementIndent();

                if (property.CanRead)
                {
                    codeWriter.Append($$"""
                        get;
                        """);
                }

                if (property.CanWrite)
                {
                    codeWriter.Append($$"""
                        set;
                        """);
                }

                codeWriter.DecrementIndent();
                codeWriter.AppendAndReserveNewLine("}");
            }

            foreach (var property in _instanceProperties)
            {
                if (!property.IsSupported() || AttributeGenerator.IsSurpressed(property))
                    continue;

                codeWriter.Append($$"""
                    {{property.GetSignatureExpression()}}
                    {
                    """);
                codeWriter.IncrementIndent();

                if (property.IsAbstract)
                {
                    if (property.CanRead)
                    {
                        codeWriter.Append($$"""
                            get;
                            """);
                    }

                    if (property.CanWrite)
                    {
                        codeWriter.Append($$"""
                            set;
                            """);
                    }
                }
                else
                {
                    GeneratePropertyGetterSetter(codeWriter, property);
                }

                codeWriter.DecrementIndent();
                codeWriter.AppendAndReserveNewLine("}");
            }

            foreach (var method in _staticMethods)
            {
                if (!method.IsSupported() || AttributeGenerator.IsSurpressed(method))
                    continue;

                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetSignatureExpression()}};
                    """);
            }

            foreach (var method in _instanceMethods)
            {
                if (AttributeGenerator.IsSurpressed(method))
                    continue;

                if (SpecializedMethodGenerator.Generate(codeWriter, method))
                    continue;

                if (!method.IsSupported())
                    continue;

                if (method.IsAbstract)
                {
                    codeWriter.AppendAndReserveNewLine($$"""
                        {{method.GetSignatureExpression()}};
                        """);
                }
                else
                {
                    GenerateMethod(codeWriter, method);
                }
            }

            foreach (var eventDef in _staticEvents)
            {
                if (!eventDef.IsSupported() || AttributeGenerator.IsSurpressed(eventDef))
                    continue;

                codeWriter.AppendAndReserveNewLine(eventDef.GetScriptBlockMethodExpression());

                codeWriter.AppendAndReserveNewLine($$"""
                    {{eventDef.GetEventCallbackMethodSignatureExpression()}};
                    """);
            }

            foreach (var eventDef in _instanceEvents)
            {
                if (!eventDef.IsSupported() || AttributeGenerator.IsSurpressed(eventDef))
                    continue;

                codeWriter.AppendAndReserveNewLine(eventDef.GetScriptBlockMethodExpression());

                codeWriter.AppendAndReserveNewLine($$"""
                    {{eventDef.GetEventCallbackMethodSignatureExpression()}};
                    """);
            }

            foreach (var nestedObject in _nestedObjects)
            {
                nestedObject.Generate(codeWriter);
            }

            codeWriter.DecrementIndent();
        }
        codeWriter.AppendAndReserveNewLine("}");
    }

    private void GeneratePropertyGetterSetter(CodeWriter codeWriter, PropertyDef property)
    {
        string typeNameExpression = property.ExplicitInterfaceType is not null ? $"ObjectTypeMapping.Get().GetTargetTypeName(typeof({Type.GetName()}))" : "null";
        if (property.CanRead)
        {
            if (property.IsIndexer)
            {
                codeWriter.Append($$"""
                    get => PropertyAccessor.GetIndexer<{{property.Type.GetName()}}>(
                        {{Generator.Api!.ObjectIdMemberName}},
                        {{typeNameExpression}},
                        "{{property.GetOriginalName()}}"{{property.GetIndexerArgumentsExpression(genericTypeParametersOverride: null)}}){{(property.Type.IsNullable ? "" : "!")}};
                    """);
            }
            else
            {
                codeWriter.Append($$"""
                    get => PropertyAccessor.Get<{{property.Type.GetName()}}>(
                        {{Generator.Api!.ObjectIdMemberName}},
                        {{typeNameExpression}},
                        {{property.GetNameOfExpression()}}){{(property.Type.IsNullable ? "" : "!")}};
                    """);
            }
        }

        // Make the property readonly if it's struct. We cannot update value type objects in the ObjectStore.
        if (property.CanWrite && (Type.IsClass || property.ImplementsInterface))
        {
            if (property.IsIndexer)
            {
                codeWriter.Append($$"""
                    set => PropertyAccessor.SetIndexer(
                        {{Generator.Api!.ObjectIdMemberName}},
                        {{typeNameExpression}},
                        "{{property.GetOriginalName()}}", {{property.Type.GetValueExpression()}}{{property.GetIndexerArgumentsExpression(genericTypeParametersOverride: null)}});
                    """);
            }
            else
            {
                codeWriter.Append($$"""
                    set => PropertyAccessor.Set(
                        {{Generator.Api!.ObjectIdMemberName}},
                        {{typeNameExpression}},
                        {{property.GetNameOfExpression()}},
                        {{property.Type.GetValueExpression()}});
                    """);
            }
        }
    }

    private void GenerateMethod(CodeWriter codeWriter, MethodDef method)
    {
        var returnType = method.ReturnType!;
        string typeNameExpression = method.ExplicitInterfaceType is not null ? $"ObjectTypeMapping.Get().GetTargetTypeName(typeof({Type.GetName()}))" : "null";

        if (returnType.IsVoid)
        {
            codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetSignatureExpression()}}
                    {
                        CommandClient.Get().InvokeMethod(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression()}}{{method.GetArgumentsExpression(genericTypeParametersOverride: null)}});
                    }
                    """);
        }
        else
        {
            codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetSignatureExpression()}}
                    {
                        return CommandClient.Get().InvokeMethodAndGetResult<{{returnType.GetName()}}>(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression()}}{{method.GetArgumentsExpression(genericTypeParametersOverride: null)}}){{(returnType.IsNullable ? "" : "!")}};
                    }
                    """);
        }
    }

    private void GenerateClass(CodeWriter codeWriter)
    {
        string genericArgumentsExpression = Type.GetGenericArgumentsExpression();

        StringBuilder baseTypeExpression = new();
        if (BaseType is not null && BaseType.IsSupported())
        {
            _ = baseTypeExpression.Append($" : {BaseType.GetName()}");
        }

        bool hasBaseType = baseTypeExpression.Length > 0;
        bool isStatic = Type.IsStatic;

        foreach (var interfaceType in GetImplementedInterfaces())
        {
            if (interfaceType.IsSupported())
            {
                if (baseTypeExpression.Length == 0)
                {
                    _ = baseTypeExpression.Append(" : ");
                }
                else
                {
                    _ = baseTypeExpression.Append(", ");
                }
                _ = baseTypeExpression.Append(interfaceType.GetName());
            }
        }

        if (baseTypeExpression.Length == 0 && !isStatic)
        {
            _ = baseTypeExpression.Append($" : {Generator.Api!.ObjectInterfaceName}");
        }

        codeWriter.Append($$"""
            public {{(isStatic ? "static " : "")}}partial class {{_apiObjectDef.Name}}{{genericArgumentsExpression}}{{baseTypeExpression}}
            {
            """);
        codeWriter.IncrementIndent();

        if (_staticEvents.Count > 0)
        {
            codeWriter.AppendAndReserveNewLine(EventDef.GetEventCallbackListExpression(MemberDefType.Static));
        }
        if (_instanceEvents.Count > 0)
        {
            codeWriter.AppendAndReserveNewLine(EventDef.GetEventCallbackListExpression(MemberDefType.Instance));
        }

        if (!hasBaseType && !isStatic)
        {
            codeWriter.AppendAndReserveNewLine($$"""
                public ObjectId {{Generator.Api!.ObjectIdMemberName}} { get; protected set; } = new();
                """);
        }

        string baseInitializer = hasBaseType ? " : base(ObjectId.Null)" : "";
        foreach (var method in _constructors)
        {
            if (!method.IsSupported() || AttributeGenerator.IsSurpressed(method))
                continue;

            codeWriter.AppendAndReserveNewLine($$"""
                {{method.GetConstructorSignatureExpression(_apiObjectDef.Name)}}{{baseInitializer}}
                {
                    {{Generator.Api!.ObjectIdMemberName}} = CommandClient.Get().CreateObject(
                        ObjectTypeMapping.Get().GetTargetTypeName(typeof({{Type.GetName()}})),
                        this{{method.GetArgumentsExpression(genericTypeParametersOverride: null)}});
                }
                """);
        }

        if (!AttributeGenerator.IsConstructorSurpressed(Type.GetName()) && !isStatic)
        {
            if (hasBaseType)
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    internal {{_apiObjectDef.Name}}(ObjectId id) : base(id)
                    {
                    }
                    """);
            }
            else
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    internal {{_apiObjectDef.Name}}(ObjectId id)
                    {
                        {{Generator.Api!.ObjectIdMemberName}} = id;
                    }
                    """);
            }
        }

        if (!hasBaseType && !isStatic)
        {
            codeWriter.AppendAndReserveNewLine($$"""
                ~{{_apiObjectDef.Name}}()
                {
                    CommandClient.Get().DestroyObject({{Generator.Api!.ObjectIdMemberName}});
                }
                """);
        }

        foreach (var property in _staticProperties)
        {
            if (!property.IsSupported() || AttributeGenerator.IsSurpressed(property))
                continue;

            codeWriter.Append($$"""
                {{property.GetSignatureExpression()}}
                {
                """);
            codeWriter.IncrementIndent();

            if (property.CanRead)
            {
                codeWriter.Append($$"""
                    get => PropertyAccessor.GetStatic<{{property.Type.GetName()}}>(
                        ObjectTypeMapping.Get().GetTargetTypeName(typeof({{Type.GetName()}})),
                        {{property.GetNameOfExpression()}}){{(property.Type.IsNullable ? "" : "!")}};
                    """);
            }

            if (property.CanWrite)
            {
                codeWriter.Append($$"""
                    set => PropertyAccessor.SetStatic(
                        ObjectTypeMapping.Get().GetTargetTypeName(typeof({{Type.GetName()}})),
                        {{property.GetNameOfExpression()}},
                        value);
                    """);
            }

            codeWriter.DecrementIndent();
            codeWriter.AppendAndReserveNewLine("}");
        }

        foreach (var property in _instanceProperties)
        {
            if (!property.IsSupported() || AttributeGenerator.IsSurpressed(property))
                continue;

            codeWriter.Append($$"""
                {{property.GetSignatureExpression()}}
                {
                """);
            codeWriter.IncrementIndent();

            GeneratePropertyGetterSetter(codeWriter, property);

            codeWriter.DecrementIndent();
            codeWriter.AppendAndReserveNewLine("}");
        }

        foreach (var method in _staticMethods)
        {
            if (!method.IsSupported() || AttributeGenerator.IsSurpressed(method))
                continue;

            var returnType = method.ReturnType!;
            if (returnType.IsVoid)
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetSignatureExpression()}}
                    {
                        CommandClient.Get().InvokeStaticMethod(
                            ObjectTypeMapping.Get().GetTargetTypeName(typeof({{Type.GetName()}})),
                            {{method.GetNameOfExpression()}}{{method.GetArgumentsExpression(genericTypeParametersOverride: null)}});
                    }
                    """);
            }
            else
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetSignatureExpression()}}
                    {
                        return CommandClient.Get().InvokeStaticMethodAndGetResult<{{returnType.GetName()}}>(
                            ObjectTypeMapping.Get().GetTargetTypeName(typeof({{Type.GetName()}})),
                            {{method.GetNameOfExpression()}}{{method.GetArgumentsExpression(genericTypeParametersOverride: null)}}){{(returnType.IsNullable ? "" : "!")}};
                    }
                    """);
            }
        }

        foreach (var method in _instanceMethods)
        {
            if (AttributeGenerator.IsSurpressed(method))
                continue;

            if (SpecializedMethodGenerator.Generate(codeWriter, method))
                continue;

            if (!method.IsSupported())
                continue;

            GenerateMethod(codeWriter, method);
        }

        foreach (var eventDef in _staticEvents)
        {
            if (!eventDef.IsSupported() || AttributeGenerator.IsSurpressed(eventDef))
                continue;

            if (eventDef.ExplicitInterfaceType is null)
            {
                codeWriter.AppendAndReserveNewLine(eventDef.GetScriptBlockMethodExpression());
            }
            codeWriter.AppendAndReserveNewLine(eventDef.GetEventCallbackMethodExpression());
        }

        foreach (var eventDef in _instanceEvents)
        {
            if (!eventDef.IsSupported() || AttributeGenerator.IsSurpressed(eventDef))
                continue;

            if (eventDef.ExplicitInterfaceType is null)
            {
                codeWriter.AppendAndReserveNewLine(eventDef.GetScriptBlockMethodExpression());
            }
            codeWriter.AppendAndReserveNewLine(eventDef.GetEventCallbackMethodExpression());
        }

        foreach (var nestedObject in _nestedObjects)
        {
            nestedObject.Generate(codeWriter);
        }

        codeWriter.DecrementIndent();
        codeWriter.AppendAndReserveNewLine("}");
    }

    // When properties or methods return an object with its interface type, and if we don't have the corresponding object type on the client side,
    // we have to create an accessor object that implements the interface. This function generates such implementation classes.
    private void GenerateInterfaceImpl(CodeWriter codeWriter)
    {
        string genericArgumentsExpression = Type.GetGenericArgumentsExpression();
        string baseTypeExpression = $" : {_apiObjectDef.Name}{genericArgumentsExpression}";
        string className = $"{_apiObjectDef.Name}_Impl";

        codeWriter.Append($$"""
            public partial class {{className}}{{genericArgumentsExpression}}{{baseTypeExpression}}
            {
            """);
        codeWriter.IncrementIndent();

        codeWriter.AppendAndReserveNewLine($$"""
            public ObjectId {{Generator.Api!.ObjectIdMemberName}} { get; protected set; } = new();
            """);

        if (!AttributeGenerator.IsConstructorSurpressed(Type.GetName()))
        {
            codeWriter.AppendAndReserveNewLine($$"""
                internal {{className}}(ObjectId id)
                {
                    {{Generator.Api!.ObjectIdMemberName}} = id;
                }
                """);
        }

        SignatureStore signatureStore = new();
        var ns = Generator.GetTargetNamespace(_apiObjectDef.Namespace);
        string rootClassName = $"{ns}.{className}{genericArgumentsExpression}";
        GenerateInterfaceImplBody(codeWriter, rootClassName, Type.GenericArguments, signatureStore);

        codeWriter.DecrementIndent();
        codeWriter.AppendAndReserveNewLine("}");
    }

    private void GenerateInterfaceImplBody(CodeWriter codeWriter, string rootClassName, List<TypeDef>? genericTypeParametersOverride, SignatureStore signatureStore)
    {
        if (signatureStore.ContainsObject(this))
            return;

        string typeName = Type.OverrideGenericTypeParameter(genericTypeParametersOverride).GetName();
        string typeNameExpression = $"ObjectTypeMapping.Get().GetTargetTypeName(typeof({typeName}))";

        foreach (var property in _staticProperties)
        {
            if (!property.IsSupported())
                continue;

            bool isExplicit = signatureStore.ContainsSignature(property);

            if (AttributeGenerator.IsSurpressed(rootClassName, property, isExplicit))
                continue;

            TypeDef propertyType = property.Type.OverrideGenericTypeParameter(genericTypeParametersOverride);

            codeWriter.Append($$"""
                {{property.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                {
                """);
            codeWriter.IncrementIndent();

            if (property.CanRead)
            {
                codeWriter.Append($$"""
                    get => PropertyAccessor.GetStatic<{{propertyType.GetName()}}>(
                        {{typeNameExpression}},
                        {{property.GetNameOfExpression()}}){{(property.Type.IsNullable ? "" : "!")}};
                    """);
            }

            if (property.CanWrite)
            {
                codeWriter.Append($$"""
                    set => PropertyAccessor.SetStatic(
                        {{typeNameExpression}},
                        {{property.GetNameOfExpression()}},
                        value);
                    """);
            }

            codeWriter.DecrementIndent();
            codeWriter.AppendAndReserveNewLine("}");
        }

        foreach (var property in _instanceProperties)
        {
            if (!property.IsSupported())
                continue;

            bool isExplicit = signatureStore.ContainsSignature(property);

            if (AttributeGenerator.IsSurpressed(rootClassName, property, isExplicit))
                continue;

            TypeDef propertyType = property.Type.OverrideGenericTypeParameter(genericTypeParametersOverride);

            codeWriter.Append($$"""
                {{property.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                {
                """);
            codeWriter.IncrementIndent();

            if (property.CanRead)
            {
                if (property.IsIndexer)
                {
                    codeWriter.Append($$"""
                        get => PropertyAccessor.GetIndexer<{{propertyType.GetName()}}>(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            "{{property.GetOriginalName(isExplicit, addInterfaceName: false)}}"{{property.GetIndexerArgumentsExpression(genericTypeParametersOverride)}}){{(propertyType.IsNullable ? "" : "!")}};
                        """);
                }
                else
                {
                    codeWriter.Append($$"""
                        get => PropertyAccessor.Get<{{propertyType.GetName()}}>(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{property.GetNameOfExpression(isExplicit, addInterfaceName: false)}}){{(propertyType.IsNullable ? "" : "!")}};
                        """);
                }
            }

            if (property.CanWrite)
            {
                if (property.IsIndexer)
                {
                    codeWriter.Append($$"""
                        set => PropertyAccessor.SetIndexer(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            "{{property.GetOriginalName(isExplicit, addInterfaceName: false)}}", {{propertyType.GetValueExpression()}}{{property.GetIndexerArgumentsExpression(genericTypeParametersOverride)}});
                        """);
                }
                else
                {
                    codeWriter.Append($$"""
                        set => PropertyAccessor.Set(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{property.GetNameOfExpression(isExplicit, addInterfaceName: false)}}, {{propertyType.GetValueExpression()}});
                        """);
                }
            }

            codeWriter.DecrementIndent();
            codeWriter.AppendAndReserveNewLine("}");
        }

        foreach (var method in _staticMethods)
        {
            if (!method.IsSupported())
                continue;

            bool isExplicit = signatureStore.ContainsSignature(method);

            if (AttributeGenerator.IsSurpressed(method, isExplicit))
                continue;

            var returnType = method.ReturnType!.OverrideGenericTypeParameter(genericTypeParametersOverride);
            if (returnType.IsVoid)
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                    {
                        CommandClient.Get().InvokeStaticMethod(
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression(isExplicit, addInterfaceName: false)}}{{method.GetArgumentsExpression(genericTypeParametersOverride)}});
                    }
                    """);
            }
            else
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                    {
                        return CommandClient.Get().InvokeStaticMethodAndGetResult<{{returnType.GetName()}}>(
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression(isExplicit, addInterfaceName: false)}}{{method.GetArgumentsExpression(genericTypeParametersOverride)}}){{(returnType.IsNullable ? "" : "!")}};
                    }
                    """);
            }
        }

        foreach (var method in _instanceMethods)
        {
            bool isExplicit = signatureStore.ContainsSignature(method);

            if (AttributeGenerator.IsSurpressed(method, isExplicit))
                continue;

            if (SpecializedMethodGenerator.GenerateForInterfaceImpl(codeWriter, method, genericTypeParametersOverride, signatureStore))
                continue;

            if (!method.IsSupported())
                continue;

            var returnType = method.ReturnType!.OverrideGenericTypeParameter(genericTypeParametersOverride);

            if (returnType.IsVoid)
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                    {
                        CommandClient.Get().InvokeMethod(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression(isExplicit, addInterfaceName: false)}}{{method.GetArgumentsExpression(genericTypeParametersOverride)}});
                    }
                    """);
            }
            else
            {
                codeWriter.AppendAndReserveNewLine($$"""
                    {{method.GetInterfaceImplSignatureExpression(isExplicit, genericTypeParametersOverride)}}
                    {
                        return CommandClient.Get().InvokeMethodAndGetResult<{{returnType.GetName()}}>(
                            {{Generator.Api!.ObjectIdMemberName}},
                            {{typeNameExpression}},
                            {{method.GetNameOfExpression(isExplicit, addInterfaceName: false)}}{{method.GetArgumentsExpression(genericTypeParametersOverride)}}){{(returnType.IsNullable ? "" : "!")}};
                    }
                    """);
            }
        }

        if (_staticEvents.Count > 0)
        {
            string eventCallbackList = EventDef.GetEventCallbackListExpression(MemberDefType.Static);
            if (!signatureStore.ContainsString(eventCallbackList))
            {
                codeWriter.AppendAndReserveNewLine(eventCallbackList);
                signatureStore.AddString(eventCallbackList);
            }
        }
        if (_instanceEvents.Count > 0)
        {
            string eventCallbackList = EventDef.GetEventCallbackListExpression(MemberDefType.Instance);
            if (!signatureStore.ContainsString(eventCallbackList))
            {
                codeWriter.AppendAndReserveNewLine(eventCallbackList);
                signatureStore.AddString(eventCallbackList);
            }
        }

        foreach (var eventDef in _instanceEvents)
        {
            if (!eventDef.IsSupported())
                continue;

            bool isExplicit = signatureStore.ContainsSignature(eventDef);

            if (AttributeGenerator.IsSurpressed(eventDef, isExplicit))
                continue;

            if (!isExplicit)
            {
                codeWriter.AppendAndReserveNewLine(
                    eventDef.GetInterfaceImplScriptBlockMethodExpression(isExplicit));
            }

            codeWriter.AppendAndReserveNewLine(
                eventDef.GetInterfaceImplEventCallbackMethodExpression(
                    typeName,
                    isExplicit,
                    genericTypeParametersOverride));
        }

        signatureStore.AddObjectDef(this);

        foreach (var interfaceType in Interfaces)
        {
            if (!interfaceType.IsSupported())
                continue;

            ObjectDef? interfaceObject = ObjectGenerator.GetObjectDef(interfaceType);
            if (interfaceObject is null)
                continue;

            TypeDef overridenInterfaceType = interfaceType.OverrideGenericTypeParameter(genericTypeParametersOverride);

            interfaceObject.GenerateInterfaceImplBody(
                codeWriter,
                rootClassName,
                overridenInterfaceType.GenericArguments,
                signatureStore);
        }
    }

    private List<TypeDef> GetImplementedInterfaces()
    {
        if (Type.IsGenericType && BaseType is not null && BaseType.IsSupported() && Interfaces.Count > 1)
        {
            // Interfaces property also includes interfaces that the base type implements which appear first in the list.
            // A class cannot implement muptiple interfaces that have the same generic type and one takes a normal type but another takes a generic parameter.
            // e.g.  class Foo<T> : BaseClass, IBar<string>, IBar<T>
            // We remove such duplicated invalid interfaces that are inherited from the base type.

            List<TypeDef> output = [];
            for (int i = 0; i < Interfaces.Count; ++i)
            {
                TypeDef interfaceI = Interfaces[i];
                bool valid = true;
                if (interfaceI.IsGenericType)
                {
                    var id = interfaceI.GetId();
                    for (int j = 0; j < Interfaces.Count; ++j)
                    {
                        if (i == j)
                            continue;

                        TypeDef interfaceJ = Interfaces[j];
                        if (interfaceJ.IsGenericType && (id == interfaceJ.GetId()))
                        {
                            if (interfaceJ.GenericArguments.Any(genericArgument => genericArgument.IsGenericTypeParameter))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                }

                if (valid)
                {
                    output.Add(interfaceI);
                }
            }
            return output;
        }
        else
        {
            return Interfaces;
        }
    }

}
