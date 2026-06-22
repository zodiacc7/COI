using System.Reflection;
using Mafi;
using Mafi.Base;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;

namespace Storable;

public sealed class Storable : DataOnlyMod
{
    public Storable(ModManifest manifest, CoreMod coreMod, BaseMod baseMod)
        : base(manifest, coreMod, baseMod)
    {
    }

    public string Name => "Storable";
    public int Version => 1;

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        var ids = new[] { Ids.Products.Exhaust };

        var field = typeof(ProductProto).GetField(
            nameof(ProductProto.IsStorable),
            BindingFlags.Public | BindingFlags.Instance
        );

        if (field == null)
        {
            Log.Warning("IsStorable field not found");
            return;
        }

        foreach (var id in ids)
        {
            if (registrator.PrototypesDb.TryGetProto(id, out ProductProto proto))
            {
                field.SetValue(proto, true);
            }
        }
    }
}
