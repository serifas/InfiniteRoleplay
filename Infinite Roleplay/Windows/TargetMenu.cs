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
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;

namespace InfiniteRoleplay.Windows
{
    public class TargetMenu : Window, IDisposable
    {

        private Plugin plugin;
        private string profileViewImagePath;
        private IDalamudTextureWrap profileViewImage;
        private string requestFriendImagePath;
        private IDalamudTextureWrap requestFriendImage;
        private string addBookmarkImagePath;
        private IDalamudTextureWrap addBookmarkImage;
        private string removeBookmarkImagePath;
        private IDalamudTextureWrap removeBookmarkImage;
        private string groupInviteImagePath;
        private IDalamudTextureWrap groupInviteImage;
        public static bool isAdmin;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        private ITargetManager targetManager;
        public static PlayerCharacter playerCharacter;
        private IChatGui ChatGUI;
        public static PlayerCharacter lastTarget;
        private bool _showFileDialogError = false;
        public bool openedProfile = false;
        public bool openedTargetProfile = false;
        public static bool DisableInput = false;
        public TargetMenu(Plugin plugin, DalamudPluginInterface Interface, ITargetManager targetManager) : base(
       "TARGET OPTIONS", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(280, 100),
                MaximumSize = new Vector2(280, 100)
            };
            this.plugin = plugin;
            this.configuration = plugin.Configuration;
            this.profileViewImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/profile_view.png");
            this.profileViewImage = Interface.UiBuilder.LoadImage(profileViewImagePath);
            this.requestFriendImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/friend_request.png");
            this.requestFriendImage = Interface.UiBuilder.LoadImage(requestFriendImagePath);
            this.addBookmarkImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/bookmark.png");
            this.addBookmarkImage = Interface.UiBuilder.LoadImage(addBookmarkImagePath);
            this.removeBookmarkImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/remove_bookmark.png");
            this.removeBookmarkImage = Interface.UiBuilder.LoadImage(removeBookmarkImagePath);
            this.groupInviteImagePath = Path.Combine(Interface.AssemblyLocation.Directory?.FullName!, "UI/common/group_invite.png");
            this.groupInviteImage = Interface.UiBuilder.LoadImage(groupInviteImagePath);
            this.targetManager = targetManager;
        }
        public override void OnClose()
        {
            plugin.targeted = false;
        }
       
        public override void Draw()
        {
            if(DisableInput == true)
            {
                ImGui.BeginDisabled();
            }
            if (ImGui.ImageButton(this.profileViewImage.ImGuiHandle, new Vector2(50, 50)))
            {
                DisableInput = true;
                LoginWindow.loginRequest = true;
                plugin.ReloadTarget();
                plugin.targetWindow.IsOpen = true;
                var targetPlayer = targetManager.Target as PlayerCharacter;
                if (targetPlayer != null)
                {
                    TargetWindow.characterNameVal = targetPlayer.Name.ToString();
                    TargetWindow.characterWorldVal = targetPlayer.HomeWorld.GameData.Name.ToString();
                    DataSender.RequestTargetProfile(targetPlayer.Name.ToString(), targetPlayer.HomeWorld.GameData.Name.ToString(), configuration.username);
                }


            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("View Profile");
            }


            ImGui.SameLine();

            if (ImGui.ImageButton(this.requestFriendImage.ImGuiHandle, new Vector2(50, 50)))
            {
                //plugin.WindowSystem.GetWindow("SHINE RULEBOOK").IsOpen = true;

            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Specify Relationships (Coming soon)");
            }


            ImGui.SameLine();

            if (ImGui.ImageButton(this.groupInviteImage.ImGuiHandle, new Vector2(50, 50)))
            {
                
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Invite to Event (Coming Soon)");
            }


            ImGui.SameLine();

            if (ImGui.ImageButton(this.addBookmarkImage.ImGuiHandle, new Vector2(50, 50)))
            {
                var targetPlayer = targetManager.Target as PlayerCharacter;
                if (targetPlayer != null)
                {
                    DataSender.BookmarkPlayer(plugin.Configuration.username, targetPlayer.Name.ToString(), targetPlayer.HomeWorld.GameData.Name.ToString());
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Bookmark Profile");
            }
            if(DisableInput == true)
            {
                ImGui.EndDisabled();
            }


        }


        public void Dispose()
        {
            WindowOpen = false;
        }


    }
}
