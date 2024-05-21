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
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.Havok;
using System.Text.RegularExpressions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Utility;
using InfiniteRoleplay.Helpers;

namespace InfiniteRoleplay.Windows
{
    public class BookmarksWindow : Window, IDisposable
    {
        private Plugin plugin;
        private GameFontHandle _nameFont;
        private GameFontHandle _infoFont;
        private float _modVersionWidth;
        public static SortedList<string, string> profiles = new SortedList<string, string>();
        private DalamudPluginInterface pg;
        public static bool DisableBookmarkSelection = false;
        public BookmarksWindow(Plugin plugin) : base(
       "BOOKMARKS", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(300, 500),
                MaximumSize = new Vector2(500, 800)
            };
            this.plugin = plugin;
        }
        public override void Draw()
        {

            Misc.SetTitle(plugin, false, "Profiles");
            using var defInfFontDen = ImRaii.DefaultFont();
            using var DefaultColor = ImRaii.DefaultColors();

            if (ImGui.BeginChild("Profiles", new Vector2(290, 380), true))
            {
                for (int i = 1; i < profiles.Count; i++)
                {
                    if (DisableBookmarkSelection == true)
                    {
                        ImGui.BeginDisabled();
                    }
                    if (ImGui.Button(profiles.Keys[i] + " @ " + profiles.Values[i]))
                    {
                        ReportWindow.reportCharacterName = profiles.Keys[i];
                        ReportWindow.reportCharacterWorld = profiles.Values[i];
                        TargetWindow.characterNameVal = profiles.Keys[i];
                        TargetWindow.characterWorldVal = profiles.Values[i];
                        //DisableBookmarkSelection = true;
                        plugin.TargetWindow.IsOpen = true;
                        DataSender.RequestTargetProfile(profiles.Keys[i], profiles.Values[i], plugin.Configuration.username);

                    }
                    ImGui.SameLine();
                    using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                    {
                        if (ImGui.Button("Remove##Removal" + i))
                        {
                            DataSender.RemoveBookmarkedPlayer(plugin.Configuration.username.ToString(), profiles.Keys[i], profiles.Values[i]);
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                    {
                        ImGui.SetTooltip("Ctrl Click to Enable");
                    }




                    if (DisableBookmarkSelection == true)
                    {
                        ImGui.EndDisabled();
                    }

                }

            }
            ImGui.EndChild();

        }

        public void Dispose()
        {

        }
        public override void Update()
        {

        }
    }
}
