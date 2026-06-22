using System.Reflection;
using Mafi;
using Mafi.Base;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;

namespace Storable;

public sealed class Storable : DataOnlyMod
{
    public Storable(ModManifest manifest)
        : base(manifest)
    {
    }

    public string Name => "Storable";
    public int Version => 1;

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        ApplyPatch(registrator.PrototypesDb);
    }

    private void ApplyPatch(ProtosDb protosDb)
    {
        var field = typeof(ProductProto).GetField(
            nameof(ProductProto.IsStorable),
            BindingFlags.Public | BindingFlags.Instance
        );

        if (field == null)
        {
            Log.Warning("Storable: IsStorable field not found");
            return;
        }

        var ids = new[]
        {
            Ids.Products.Exhaust
        };

        foreach (var id in ids)
        {
            if (protosDb.TryGetProto(id, out ProductProto proto))
            {
                field.SetValue(proto, true);
            }
        }

        Log.Info("Storable: patch applied");
    }
}
