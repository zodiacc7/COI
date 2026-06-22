using System.Reflection;
using Mafi;
using Mafi.Base;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Collections;

namespace Storable;

public sealed class Storable : DataOnlyMod
{
    public Storable(ModManifest manifest)
        : base(manifest)
    {
    }

    public string Name => "Storable";
    public int Version => 1;

    public override void Initialize(DependencyResolver resolver, bool gameWasLoaded)
    {
        // 1. patch prototypes
        ApplyPrototypePatch(resolver.ProtosDb);

        // 2. try retroactive fix (if world already loaded)
        if (gameWasLoaded)
        {
            TryRetroactiveFix(resolver);
        }
    }

    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        ApplyPrototypePatch(registrator.PrototypesDb);
    }

    // =========================
    // 1. PROTOTYPE PATCH (GLOBAL RULE)
    // =========================
    private void ApplyPrototypePatch(ProtosDb protosDb)
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
            // اینجا هر آیتم دیگه هم می‌تونی اضافه کنی
        };

        foreach (var id in ids)
        {
            if (protosDb.TryGetProto(id, out ProductProto proto))
            {
                field.SetValue(proto, true);
            }
        }

        Log.Info("Storable: prototype patch applied");
    }

    // =========================
    // 2. RETROACTIVE FIX (SAFE BEST-EFFORT)
    // =========================
    private void TryRetroactiveFix(DependencyResolver resolver)
    {
        try
        {
            var world = resolver.ResolveOptional<World>();
            if (world == null)
            {
                Log.Warning("Storable: World not available for retroactive patch");
                return;
            }

            var field = typeof(ProductProto).GetField(
                nameof(ProductProto.IsStorable),
                BindingFlags.Public | BindingFlags.Instance
            );

            if (field == null)
                return;

            // ⚠️ COI does NOT expose full inventory API reliably
            // So this part is best-effort depending on version

            var protosDb = resolver.ProtosDb;

            foreach (var proto in protosDb.GetAll<ProductProto>())
            {
                // فقط دوباره enforce کن
                if (proto.Id == Ids.Products.Exhaust)
                {
                    field.SetValue(proto, true);
                }
            }

            Log.Info("Storable: retroactive patch attempted");
        }
        catch (System.Exception ex)
        {
            Log.Warning("Storable: retroactive patch failed: " + ex.Message);
        }
    }
}
