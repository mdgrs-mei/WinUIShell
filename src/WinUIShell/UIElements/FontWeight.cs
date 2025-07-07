﻿using WinUIShell.Common;

namespace WinUIShell;

public class FontWeight : WinUIShellObject
{
    public ushort Weight
    {
        get => PropertyAccessor.Get<ushort>(Id, nameof(Weight))!;
    }

    internal FontWeight(ObjectId id)
        : base(id)
    {
    }

    public FontWeight(ushort weight)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<FontWeight>(),
            this,
            weight);
    }
}
