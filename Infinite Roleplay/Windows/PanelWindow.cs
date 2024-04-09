using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects;
using Networking;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using InfiniteRoleplay.Scripts.Misc;

namespace InfiniteRoleplay.Windows
{
    public class PanelWindow : Window, IDisposable
    {

        private Plugin plugin;
        private bool viewProfile, viewSystems, viewEvents, viewConnections;
        private IDalamudTextureWrap profileSectionImage, eventsSectionImage, systemsSectionImage, connectionsSectionImage,
                                    //profiles
                                    profileImage, npcImage, profileBookmarkImage, npcBookmarkImage,
                                    //events and venues
                                    venueImage, eventImage, venueBookmarkImage, eventBookmarkImage,
                                    //systems
                                    combatImage, statSystemImage; 
        private string profilesImagePath, eventsImagePath, systemsImagePath, connectionsImagePath,
                                    //profiles
                                    profileImagePath, npcImagePath, profileBookmarkImagePath, npcBookmarkImagePath,
                                    //events and venues
                                    venueImagePath, eventImagePath, venueBookmarkImagePath, eventBookmarkImagePath,
                                    //systems
                                    combatImagePath, statSystemImagePath;
        public static bool viewMainWindow = true;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        public static ITargetManager targetManager;
        public static PlayerCharacter playerCharacter;
        private IChatGui ChatGUI;
        public static PlayerCharacter lastTarget;
        public static string status = "";
        public static Vector4 statusCol = new Vector4(0,0,0,0);

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
            this.profileSectionImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.profileSection);
            this.eventsSectionImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.eventsSection);
            this.systemsSectionImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.systemsSection);
            this.connectionsSectionImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.connectionsSection);
            this.profileImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.profileCreateProfile);
            this.profileBookmarkImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.profileBookmarkProfile);
            this.npcImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.profileCreateNPC);
            this.npcBookmarkImage = Constants.UICommonImage(Interface, Constants.CommonImageTypes.profileBookmarkNPC);
            //profile subsections

        }

        public override void Draw()
        {
            if(viewMainWindow == true)
            {
                    #region PROFILES
                if (ImGui.ImageButton(this.profileSectionImage.ImGuiHandle, new Vector2(100, 50)))
                {
                    viewProfile = true;
                    viewMainWindow = false;
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Profiles");
                #endregion
                }
                ImGui.SameLine();
                using (OtterGui.Raii.ImRaii.Disabled(true))
                {
                    if (ImGui.ImageButton(this.connectionsSectionImage.ImGuiHandle, new Vector2(100, 50)))
                    {
                      //  viewConnections = true;
                       // viewMainWindow = false;

                    }

                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip("Connections - Coming soon");
                }
                using (OtterGui.Raii.ImRaii.Disabled(true))
                {
                    if (ImGui.ImageButton(this.eventsSectionImage.ImGuiHandle, new Vector2(100, 50)))
                    {
                        //  viewConnections = true;
                        // viewMainWindow = false;

                    }

                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip("Events - Coming soon");
                }


              
                ImGui.SameLine();

                using (OtterGui.Raii.ImRaii.Disabled(true))
                {
                    if (ImGui.ImageButton(this.systemsSectionImage.ImGuiHandle, new Vector2(100, 50)))
                    {
                        //  viewConnections = true;
                        // viewMainWindow = false;

                    }

                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip("Systems - Coming soon");
                }

               
                if (ImGui.Button("Options", new Vector2(225, 25)))
                {

                    plugin.optionsWindow.IsOpen = true;
                }
                if (ImGui.Button("Logout", new Vector2(225, 25)))
                {
                    plugin.loggedIn = false;
                    plugin.CloseAllWindows();
                    plugin.loginWindow.IsOpen = true;
                }
            }
            if(viewProfile == true)
            {
                if (ImGui.ImageButton(this.profileImage.ImGuiHandle, new Vector2(100, 50)))
                {
                    plugin.profileWindow.Reset(plugin);
                    plugin.ReloadProfile();
                    plugin.profileWindow.IsOpen = true;
                    if (playerCharacter != null)
                    {
                        //FETCH USER AND PASS ASEWLL
                        DataSender.FetchProfile(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString());
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Manage your profile");
                }
                ImGui.SameLine();
                if (ImGui.ImageButton(this.profileBookmarkImage.ImGuiHandle, new Vector2(100, 50)))
                {
                    DataSender.RequestBookmarks(configuration.username);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("View profile bookmarks");
                }
                using (OtterGui.Raii.ImRaii.Disabled(true))
                {
                    if (ImGui.ImageButton(this.npcImage.ImGuiHandle, new Vector2(100, 50)))
                    {
                        //  viewConnections = true;
                        // viewMainWindow = false;

                    }

                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip("Manage NPCs - Coming soon");
                }


             
                ImGui.SameLine();

                using (OtterGui.Raii.ImRaii.Disabled(true))
                {
                    if (ImGui.ImageButton(this.npcBookmarkImage.ImGuiHandle, new Vector2(100, 50)))
                    {
                        //  viewConnections = true;
                        // viewMainWindow = false;

                    }

                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip("View NPC bookmarks - Coming soon");
                }

            }
            
           
            if(viewProfile == true || viewSystems  == true || viewEvents == true || viewConnections == true)
            {
                if (ImGui.Button("Back"))
                {
                    switchUI();
                    viewMainWindow = true;
                }
            }
            ImGui.SameLine();
            ImGui.TextColored(statusCol, status);

        }
        public void switchWindow(bool viewWindow)
        {

        }
        public void switchUI()
        {
            viewProfile = false;
            viewSystems = false;
            viewEvents = false;
            viewConnections = false;
        }

        public void Dispose()
        {
            WindowOpen = false;
            profileSectionImage.Dispose();
        }
        public override void Update()
        {
        }


    }
}
