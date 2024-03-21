using Dalamud.Interface.Colors;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using ImGuiScene;
using InfiniteRoleplay;
using OtterGui.Raii;
using OtterGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dalamud.Interface.GameFonts;
using Dalamud.Game.Gui.Dtr;
using Microsoft.VisualBasic;
using Networking;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Internal;
using System.Reflection.Metadata;

namespace InfiniteRoleplay.Windows
{
    public class OptionsWindow : Window, IDisposable
    {
        private GameFontHandle _nameFont;
        private float _modVersionWidth;
        public static Plugin pg;
        public static bool showTargetOptions;
        public static bool showWIP;
        public static bool showKofi;
        public static bool showDisc;
        public OptionsWindow(Plugin plugin, DalamudPluginInterface Interface) : base(
       "OPTIONS", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(300, 180),
                MaximumSize = new Vector2(300, 180)
            };
            pg = plugin;
            this._nameFont = plugin.PluginInterfacePub.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
            showWIP = plugin.Configuration.showWIP;
            showTargetOptions = plugin.Configuration.showTargetOptions;
            showKofi = plugin.Configuration.showKofi;
            showDisc = plugin.Configuration.showDisc;
        }
        public override void Draw()
        {
            using var col = ImRaii.PushColor(ImGuiCol.Border, ImGuiColors.DalamudViolet);
            using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameBorderSize, 2 * ImGuiHelpers.GlobalScale);
            using var font = ImRaii.PushFont(_nameFont.ImFont, _nameFont.Available);
            ImGuiUtil.DrawTextButton("Options", Vector2.Zero, 0);
            //set everything back
            using var defCol = ImRaii.DefaultColors();
            using var defStyle = ImRaii.DefaultStyle();
            using var defFont = ImRaii.DefaultFont();
            //okay that's done.
            ImGui.Spacing();
            //now for some simple toggles
            if(ImGui.Checkbox("Show target menu when selecting players", ref showTargetOptions))
            {
                pg.Configuration.showTargetOptions = showTargetOptions;
                pg.Configuration.Save();
            }
            if (ImGui.Checkbox("Show WIP modules", ref showWIP))
            {
                pg.Configuration.showWIP = showWIP;
                pg.Configuration.Save();
            }
            if (ImGui.Checkbox("Show Ko-fi Button", ref showKofi))
            {
                pg.Configuration.showKofi = showKofi;
                pg.Configuration.Save();
            }
            if (ImGui.Checkbox("Show Discord Button", ref showDisc))
            {
                pg.Configuration.showDisc = showDisc;
                pg.Configuration.Save();
            }
        }
        public void Dispose()
        {

        }
        public override void Update()
        {
            
        }
    }

}
