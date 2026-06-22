using System.Reflection;
using Mafi;
using Mafi.Base;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;

namespace Storable;

public sealed class Storable : DataOnlyMod
{
    public override string Name => nameof(Storable);
    public override int Version => 1;

    public Storable(
        ModManifest manifest,
        CoreMod coreMod,
        BaseMod baseMod)
        : base(manifest)
    {
    }

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        ProductProto.ID[] ids = { Ids.Products.Exhaust };

        FieldInfo? fieldInfo = typeof(ProductProto).GetField(
            nameof(ProductProto.IsStorable),
            BindingFlags.Public | BindingFlags.Instance
        );

        if (fieldInfo is null)
        {
            Log.Warning("Storable: IsStorable field not found");
            return;
        }

        foreach (ProductProto.ID id in ids)
        {
            if (registrator.PrototypesDb.TryGetProto(id, out ProductProto productProto))
            {
                fieldInfo.SetValue(productProto, true);
            }
            else
            {
                Log.Warning($"Storable: Product not found {id}");
            }
        }
    }
}
