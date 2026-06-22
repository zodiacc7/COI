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
        if (_initialized)
            return;

        ApplyGlobalBehaviorPatch(registrator);
        _initialized = true;
    }

    private void ApplyGlobalBehaviorPatch(ProtoRegistrator registrator)
    {
        var db = registrator.PrototypesDb;

        var field = typeof(ProductProto).GetField(
            nameof(ProductProto.IsStorable),
            BindingFlags.Public | BindingFlags.Instance
        );

        if (field == null)
        {
            Log.Warning("Storable: field not found");
            return;
        }

        foreach (var proto in db.GetAll<ProductProto>())
        {
            try
            {
                field.SetValue(proto, true);
            }
            catch { }
        }

        Log.Info("Storable: retroactive behavior applied");
    }
}
