using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using System;
using Dalamud.Interface.GameFonts;
using InfiniteRoleplay;
using InfiniteRoleplay.Helpers;
using Networking;
namespace InfiniteRoleplay.Windows
{
    public class OptionsWindow : Window, IDisposable
    {
        private GameFontHandle _nameFont;
        private float _modVersionWidth;
        public static Plugin pg;
        public static bool showAllProfilesPublicly;
        public static bool showKofi;
        public static bool showDisc;
        public static bool showWIP;
        public OptionsWindow(Plugin plugin) : base(
       "OPTIONS", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(300, 180),
                MaximumSize = new Vector2(300, 180)
            };
            pg = plugin;
            this._nameFont = plugin.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
            showAllProfilesPublicly = plugin.Configuration.showProfilesPublicly;
            showWIP = plugin.Configuration.showWIP;
            showKofi = plugin.Configuration.showKofi;
            showDisc = plugin.Configuration.showDisc;
        }
        public override void Draw()
        {
            Misc.SetTitle(pg, false, "Options");
            //okay that's done.
            ImGui.Spacing();
            //now for some simple toggles
            if (ImGui.Checkbox("Show Ko-fi button", ref showKofi))
            {
                pg.Configuration.showKofi = showKofi;
                pg.Configuration.Save();
            }
            if (ImGui.Checkbox("Show Discord button.", ref showDisc))
            {
                pg.Configuration.showDisc = showDisc;
                pg.Configuration.Save();
            }
            if(pg.loggedIn)
            {
                if (ImGui.Checkbox("Show all my profiles publicly.", ref showAllProfilesPublicly))
                {
                    pg.Configuration.showProfilesPublicly = showAllProfilesPublicly;
                    pg.Configuration.Save();
                    DataSender.SaveUserConfiguration(showAllProfilesPublicly);
                }
            }
           
        }
        public void Dispose()
        {

        }
    }

}
