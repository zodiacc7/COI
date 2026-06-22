using System;
using System.Reflection;
using Mafi;
using Mafi.Base;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;

namespace Storable;

public sealed class Storable : DataOnlyMod
{
    public Storable(ModManifest manifest)
        : base(manifest)
    {
    }

    public string Name => "Storable";
    public int Version => 1;

    private static bool _initialized;

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        // فقط یک بار patch global
        if (_initialized)
            return;

        ApplyGlobalBehaviorPatch(registrator.PrototypesDb);
        _initialized = true;
    }

    private void ApplyGlobalBehaviorPatch(ProtosDb db)
    {
        var field = typeof(ProductProto).GetField(
            nameof(ProductProto.IsStorable),
            BindingFlags.Public | BindingFlags.Instance
        );

        if (field == null)
        {
            Log.Warning("Storable: field not found");
            return;
        }

        // ⚡ مهم: به جای retrofit save، همه proto ها را enforce می‌کنیم
        foreach (var proto in db.GetAll<ProductProto>())
        {
            try
            {
                field.SetValue(proto, true);
            }
            catch { }
        }

        Log.Info("Storable: global behavior override applied");
    }
}
