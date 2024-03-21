using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using static Dalamud.Interface.Windowing.Window;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ImGuiFileDialog;
using ImGuiNET;
using ImGuiScene;
using static Lumina.Data.Files.ScdFile;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState;
using Networking;
using Dalamud.Game.Config;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;

namespace InfiniteRoleplay.Windows
{
    public class PanelWindow : Window, IDisposable
    {

        private Plugin plugin;
        private string profilesImagePath;
        private string profilesNoWIPImagePath;
        private IDalamudTextureWrap profilesImage;
        private IDalamudTextureWrap profilesNoWIPImage;
        private string documentImagePath;
        private IDalamudTextureWrap documentImage;
        private string systemsImagePath;
        private IDalamudTextureWrap systemsImage;
        private string systemsNoWIPImagePath;
        private IDalamudTextureWrap systemsNoWIPImage;
        private string groupsImagePath;
        private IDalamudTextureWrap groupsImage;
        public static bool isAdmin;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        public static ITargetManager targetManager;
        public static PlayerCharacter playerCharacter;
        private IChatGui ChatGUI;
        public static PlayerCharacter lastTarget;
        private bool _showFileDialogError = false;
        public bool openedProfile = false;
        public bool openedTargetProfile = false;
        public static string status = "";
        public static Vector4 statusCol = new Vector4(0,0,0,0);    
        public static bool DisableInput = false;
        public PanelWindow(Plugin plugin, DalamudPluginInterface Interface, ITargetManager targetManager) : base(
       "INFINITE PANEL", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(250, 250),
                MaximumSize = new Vector2(250, 250)
            };
            
            this.plugin = plugin;
            this.configuration = plugin.Configuration;
            this.profilesImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/profiles.png");
            this.profilesImage = Interface.UiBuilder.LoadImage(profilesImagePath);
            this.profilesNoWIPImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/profiles_NoWIP.png");
            this.profilesNoWIPImage = Interface.UiBuilder.LoadImage(profilesNoWIPImagePath);
            this.documentImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/friends.png");
            this.documentImage = Interface.UiBuilder.LoadImage(documentImagePath);
            this.groupsImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/groups.png");
            this.groupsImage = Interface.UiBuilder.LoadImage(groupsImagePath);
            this.systemsImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/bookmarks.png");
            this.systemsImage = Interface.UiBuilder.LoadImage(systemsImagePath);
            this.systemsNoWIPImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/bookmarks_NoWIP.png");
            this.systemsNoWIPImage = Interface.UiBuilder.LoadImage(systemsNoWIPImagePath);


        }

        public override void Draw()
        {
            if (isAdmin == true)
            {
                if (ImGui.Button("Administration", new Vector2(225, 25)))
                {
                    plugin.adminWindow.IsOpen = true;
                }
            }
            if (configuration.showWIP == true)
            {
                if (ImGui.ImageButton(this.profilesImage.ImGuiHandle, new Vector2(100, 50)))
                {

                    LoginWindow.loginRequest = true;
                    plugin.profileWindow.Reset(plugin);
                    plugin.ReloadProfile();
                    plugin.profileWindow.IsOpen = true;
                    if (playerCharacter != null)
                    {
                        DataSender.FetchProfile(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString());
                    }

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Profile");
                }
                ImGui.SameLine();
                if (ImGui.ImageButton(this.documentImage.ImGuiHandle, new Vector2(100, 50)))
                {
                    // plugin.WindowSystem.GetWindow("SHINE RULEBOOK").IsOpen = true;

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Connections - Coming Soon");
                }
                if (ImGui.ImageButton(this.systemsImage.ImGuiHandle, new Vector2(100, 50)))
                {
                    DataSender.RequestBookmarks(configuration.username);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Bookmarks");
                }

                ImGui.SameLine();
                if (ImGui.ImageButton(this.groupsImage.ImGuiHandle, new Vector2(100, 50)))
                {

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Events - Coming Soon");
                }
            }
            else
            {
                if (ImGui.ImageButton(this.profilesNoWIPImage.ImGuiHandle, new Vector2(216, 50)))
                {

                    LoginWindow.loginRequest = true;
                    plugin.profileWindow.Reset(plugin);
                    plugin.ReloadProfile();
                    plugin.profileWindow.IsOpen = true;
                    if (playerCharacter != null)
                    {
                        DataSender.FetchProfile(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString());
                    }

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Profile");
                }
                
                if (ImGui.ImageButton(this.systemsNoWIPImage.ImGuiHandle, new Vector2(216, 50)))
                {
                    DataSender.RequestBookmarks(configuration.username);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Bookmarks");
                }

            }
            if (ImGui.Button("Options", new Vector2(225, 25)))
            {
                //plugin.window("ADMINISTRATION").IsOpen = true;
                plugin.optionsWindow.IsOpen = true;
            }
            if (ImGui.Button("Logout", new Vector2(225, 25)))
            {
                plugin.loggedIn = false;
                plugin.CloseAllWindows(false);
                plugin.loginWindow.IsOpen = true;
                           
                
            }
            ImGui.TextColored(statusCol, status);

        }
       

        public void Dispose()
        {
            WindowOpen = false;
            profilesImage.Dispose();
            profilesNoWIPImage.Dispose();
            systemsNoWIPImage.Dispose();
            documentImage.Dispose();
            groupsImage.Dispose();
            systemsImage.Dispose();
        }
        public override void Update()
        {
        }


    }
}
