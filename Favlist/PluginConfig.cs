using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace Favlist;

internal class PluginConfig
{
    public virtual bool OverwriteExisting { get; set; } = true;
    public virtual bool CheckBeatSaver { get; set; } = true;
    public virtual bool ExcludeWip { get; set; } = true;
}