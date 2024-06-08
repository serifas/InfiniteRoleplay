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
using FFXIVClientStructs.FFXIV.Client.Game;

namespace InfiniteRoleplay.Windows
{
    public class ConnectionsWindow : Window, IDisposable
    {
        private Plugin plugin;
        public static List<Tuple<string, string>> receivedProfileRequests = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> sentProfileRequests = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> blockedProfileRequests = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> connetedProfileList = new List<Tuple<string, string>>();
        public static int currentListing = 0;
        private DalamudPluginInterface pg;
        public ConnectionsWindow(Plugin plugin) : base(
       "CONNECTIONS", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
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
            Misc.SetTitle(plugin, false, "Connections");
            using var defInfFontDen = ImRaii.DefaultFont();
            using var DefaultColor = ImRaii.DefaultColors();

            AddConnectionListingOptions();

           if(currentListing == 2) { 
            ImGui.TextUnformatted("Received Requests");
            if (ImGui.BeginChild("ReceivedRequests", new Vector2(290, 250), true))
            {

                for (int i = 0; i < receivedProfileRequests.Count; i++)
                {

                    string requesterName = receivedProfileRequests[i].Item1;
                    string requesterWorld = receivedProfileRequests[i].Item2;
                    ImGui.TextUnformatted(requesterName + " @ " + requesterWorld);
                    ImGui.SameLine();
                    using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                    {
                        if (ImGui.Button("Decline##Decline" + i))
                        {
                            DataSender.SendProfileAccessUpdate(requesterName, requesterWorld, (int)Constants.ConnectionStatus.refused);
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                    {
                        ImGui.SetTooltip("Ctrl Click to Enable");
                    }
                    ImGui.SameLine();
                    using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                    {
                        if (ImGui.Button("Accept##Accept" + i))
                        {
                            DataSender.SendProfileAccessUpdate(requesterName, requesterWorld, (int)Constants.ConnectionStatus.accepted);
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                    {
                        ImGui.SetTooltip("Ctrl Click to Enable");
                    }
                    ImGui.SameLine();
                    using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                    {
                        if (ImGui.Button("Block##Block" + i))
                        {
                            DataSender.SendProfileAccessUpdate(requesterName, requesterWorld, (int)Constants.ConnectionStatus.blocked);
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                    {
                        ImGui.SetTooltip("Ctrl Click to Enable");
                    }
                }
            }

                     ImGui.EndChild();
            }
            if (currentListing == 0)
            {

                    ImGui.TextUnformatted("Connected");
                if (ImGui.BeginChild("Connected", new Vector2(290, 250), true))
                {

                    for (int i = 0; i < connetedProfileList.Count; i++)
                    {
                        string connectionName = connetedProfileList[i].Item1;
                        string connectionWorld = connetedProfileList[i].Item2;
                        ImGui.TextUnformatted(connectionName + " @ " + connectionWorld);
                        ImGui.SameLine();
                        using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                        {
                            if (ImGui.Button("Remove##Remove" + i))
                            {
                                DataSender.SendProfileAccessUpdate(connectionName, connectionWorld, (int)Constants.ConnectionStatus.removed);
                            }
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                        {
                            ImGui.SetTooltip("Ctrl Click to Enable");
                        }
                        ImGui.SameLine();
                        using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                        {
                            if (ImGui.Button("Bock##Block" + i))
                            {
                                DataSender.SendProfileAccessUpdate(connectionName, connectionWorld, (int)Constants.ConnectionStatus.blocked);
                            }
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                        {
                            ImGui.SetTooltip("Ctrl Click to Enable");
                        }
                    }
                }
                ImGui.EndChild();

            }
            if(currentListing == 1) { 
                ImGui.TextUnformatted("Sent Requests");
                if (ImGui.BeginChild("SentRequests", new Vector2(290, 250), true))
                {

                    for (int i = 0; i < sentProfileRequests.Count; i++)
                    {

                        string receiverName = sentProfileRequests[i].Item1;
                        string receiverWorld = sentProfileRequests[i].Item2;
                        ImGui.TextUnformatted(receiverName + " @ " + receiverWorld);
                        ImGui.SameLine();
                        using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                        {
                            if (ImGui.Button("Cancel##Cancel" + i))
                            {
                                DataSender.SendProfileAccessUpdate(receiverName, receiverWorld, (int)Constants.ConnectionStatus.canceled);
                            }
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                        {
                            ImGui.SetTooltip("Ctrl Click to Enable");
                        }
                    }
                }
                ImGui.EndChild();
            }
            if(currentListing == 3)
            {
                ImGui.TextUnformatted("Blocked Requests");
                if (ImGui.BeginChild("BlockedRequests", new Vector2(290, 250), true))
                {

                    for (int i = 0; i < blockedProfileRequests.Count; i++)
                    {
                        string blockedName = sentProfileRequests[i].Item1;
                        string blockedWorld = sentProfileRequests[i].Item2;
                        ImGui.TextUnformatted(blockedName + " @ " + blockedWorld);
                        ImGui.SameLine();
                        using (ImRaii.Disabled(!Plugin.CtrlPressed()))
                        {
                            if (ImGui.Button("Unblock##Unblock" + i))
                            {
                                DataSender.SendProfileAccessUpdate(blockedName, blockedWorld, (int)Constants.ConnectionStatus.removed);
                            }
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                        {
                            ImGui.SetTooltip("Ctrl Click to Enable");
                        }
                    }
                }
                ImGui.EndChild();


            }

        }
        
            public void AddConnectionListingOptions()
            {
            var (text, desc) = Constants.ConnectionListingVals[currentListing];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Alignment", text);
            ImGuiUtil.HoverTooltip(desc);
            if (!combo)
                return;

            foreach (var ((newText, newDesc), idx) in Constants.ConnectionListingVals.WithIndex())
            {
                if (ImGui.Selectable(newText, idx == currentListing))
                    currentListing = idx;

                ImGuiUtil.SelectableHelpMarker(newDesc);
            }
        }
        public void Dispose()
        {

        }
    }
}
