using System.Reflection;
using Mafi;
using Mafi.Core;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Collections;

namespace Storable;

public sealed class Storable : IMod
{
    public string Name => "Storable";
    public int Version => 1;
    public bool IsUiOnly => false;

    public ModManifest Manifest { get; }

    public Option<IModConfig> ModConfig => Option<IModConfig>.None;

    public Storable(ModManifest manifest)
    {
        Manifest = manifest;
    }

    public void Initialize(DependencyResolver resolver, bool gameWasLoaded)
    {
    }

    public void RegisterPrototypes(ProtoRegistrator registrator)
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

    public void ChangeConfigs(Lyst<IModConfig> configs)
    {
    }

    public void RegisterDependencies(
        DependencyResolverBuilder depBuilder,
        Mafi.Core.Prototypes.ProtosDb protosDb,
        bool wasLoaded)
    {
    }

    public void EarlyInit(DependencyResolver resolver)
    {
    }

    public void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues)
    {
    }

    public void Dispose()
    {
    }
}
