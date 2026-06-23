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

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        ApplyPatch(registrator);
    }

    private void ApplyPatch(ProtoRegistrator registrator)
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

        // فقط روی آیتم‌های مشخص (safe + supported)
        var ids = new[]
        {
            Ids.Products.Exhaust,
            Ids.Products.SteamDepleted
        };

        foreach (var id in ids)
        {
            if (registrator.PrototypesDb.TryGetProto(id, out ProductProto proto))
            {
                field.SetValue(proto, true);
            }
        }

        Log.Info("Storable: patch applied safely");
    }
}
