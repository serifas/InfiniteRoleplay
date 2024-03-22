using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects;
using Networking;
using Dalamud.Interface.Internal;
using InfiniteRoleplay.Scripts.Misc;

namespace InfiniteRoleplay.Windows
{
    public class TargetMenu : Window, IDisposable
    {

        private Plugin plugin;
        private IDalamudTextureWrap profileViewImage;
        private IDalamudTextureWrap groupInviteImage;
        private IDalamudTextureWrap bookmarkImage;
        private IDalamudTextureWrap assignConnectionImage;
        public static bool isAdmin;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        private ITargetManager targetManager;
        public static PlayerCharacter playerCharacter;
        public static PlayerCharacter lastTarget;
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
            this.profileViewImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.targetViewProfile);
            this.groupInviteImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.targetGroupInvite);
            this.bookmarkImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.targetBookmark);
            this.assignConnectionImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.targetConnections);
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

            if (ImGui.ImageButton(this.assignConnectionImage.ImGuiHandle, new Vector2(50, 50)))
            {
                //plugin.WindowSystem.GetWindow("SHINE RULEBOOK").IsOpen = true;

            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Specify Relationships (Coming Soon)");
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

            if (ImGui.ImageButton(this.bookmarkImage.ImGuiHandle, new Vector2(50, 50)))
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
