using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace InfiniteRoleplay;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public bool AlwaysOpenDefaultImport { get; set; } = false;
    public string username { get; set; } = "";
    public string password { get; set; } = "";
    public bool rememberInformation { get; set; }

    //Config options
    public bool showKofi { get; set; } = true;
    public bool showWIP { get; set; } = true;
    public bool showDisc { get; set; } = true;
    public bool showProfilesPublicly { get; set; } = false;
    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
    }

    public void Save()
    {
        PluginInterface!.SavePluginConfig(this);
    }
}
