using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Interface.ImGuiFileDialog;
using Networking;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Internal;
using Dalamud.Utility;
using Dalamud.Configuration;
using Microsoft.VisualBasic;
using InfiniteRoleplay;
using System.Diagnostics;
namespace InfiniteRoleplay.Windows;

public class MainPanel : Window, IDisposable
{
    //input field strings
    public string username = string.Empty;
    public string password = string.Empty;
    public string registerUser = string.Empty;
    public string registerPassword = string.Empty;
    public string registerVerPassword = string.Empty;
    public string email = string.Empty;
    public string restorationEmail = string.Empty;
    //window state toggles
    private bool viewProfile, viewSystems, viewEvents, viewConnections;
    public static bool login = true;
    public static bool forgot = false;
    public static bool register = false;
    public static bool viewMainWindow = false;
    //registration agreement
    public bool AgreeTOS = false;
    public bool Agree18 = false;
    public bool Remember = false;
    //duh
    //server status label stuff
    public static string serverStatus = "Connection Status...";
    public static Vector4 serverStatusColor = new Vector4(255, 255, 255, 255);
    public static string status = "";
    public static Vector4 statusColor = new Vector4(255, 255, 255, 255);
    //button images
    private IDalamudTextureWrap kofiBtnImg, discoBtn, profileSectionImage, eventsSectionImage, systemsSectionImage, connectionsSectionImage,
                                 //profiles
                                 profileImage, npcImage, profileBookmarkImage, npcBookmarkImage,
                                 //events and venues
                                 venueImage, eventImage, venueBookmarkImage, eventBookmarkImage,
                                 //systems
                                 combatImage, statSystemImage,
                                 reconnectImage;
    public Plugin plugin;
    public static bool LoggedIN = false;

    public MainPanel(Plugin plugin) : base(
        "INFINITE ROLEPLAY",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(250, 310);
        this.SizeCondition = ImGuiCond.Always;
        this.plugin = plugin;
        this.username = plugin.Configuration.username;
        this.password = plugin.Configuration.password;
       

        Remember = plugin.Configuration.rememberInformation;
    }
    public override void OnOpen()
    {
        var kofi = Constants.UICommonImage(plugin, Constants.CommonImageTypes.kofiBtn);
        var discod = Constants.UICommonImage(plugin, Constants.CommonImageTypes.discordBtn);
        var profileSectionImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.profileSection);
        var eventsImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.eventsSection);
        var systemsImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.systemsSection);
        var connectionsImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.connectionsSection);
        var profileImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.profileCreateProfile);
        var profileBookmarkImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.profileBookmarkProfile);
        var npcImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.profileCreateNPC);
        var npcBookmarkImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.profileBookmarkNPC);
        var reconnectImg = Constants.UICommonImage(plugin, Constants.CommonImageTypes.reconnect);

        if (kofi != null) { kofiBtnImg = kofi; }
        if (discod != null) { discoBtn = discod; }
        if (profileSectionImg != null) { profileSectionImage = profileSectionImg; }
        if (eventsImg != null) { eventsSectionImage = eventsImg; }
        if (systemsImg != null) { systemsSectionImage = systemsImg; }
        if (connectionsImg != null) { connectionsSectionImage = connectionsImg; }
        if (profileImg != null) { profileImage = profileImg; }
        if (profileBookmarkImg != null) { profileBookmarkImage = profileBookmarkImg; }
        if (npcImg != null) { npcImage = npcImg; }
        if (npcBookmarkImg != null) { npcBookmarkImage = npcBookmarkImg; }
        if (reconnectImg != null) { reconnectImage = reconnectImg; }
    }

    public void Dispose()
    {
        kofiBtnImg?.Dispose();
        discoBtn?.Dispose();
        profileSectionImage?.Dispose();
        eventsSectionImage?.Dispose(); 
        systemsSectionImage?.Dispose(); 
        connectionsSectionImage?.Dispose();

        //profiles
        profileImage?.Dispose();
        npcImage?.Dispose(); 
        profileBookmarkImage?.Dispose(); 
        npcBookmarkImage?.Dispose();
        //events and venues
        venueImage?.Dispose(); 
        eventImage?.Dispose();
        venueBookmarkImage?.Dispose(); 
        eventBookmarkImage?.Dispose();
        //systems
        combatImage?.Dispose();
        statSystemImage?.Dispose();
        //connection
        reconnectImage?.Dispose();
    }
    public override void Draw()
    {
        // can't ref a property, so use a local copy
        if (login == true)
        {
            ImGui.InputTextWithHint("##username", $"Username", ref this.username, 100);
            ImGui.InputTextWithHint("##password", $"Password", ref this.password, 100, ImGuiInputTextFlags.Password);

            if (ImGui.Button("Login"))
            {
                if (plugin.IsLoggedIn())
                {
                    SaveLoginPreferences();
                    DataSender.Login(this.username, this.password, plugin.ClientState.LocalPlayer.Name.ToString(), plugin.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString());
                }
            }
            ImGui.SameLine();
            if(ImGui.Checkbox("Remember Me", ref Remember)){
                SaveLoginPreferences();
            }
            if (ImGui.Button("Forgot"))
            {
                login = false;
                register = false;
                forgot = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("Register"))
            {
                login = false;
                register = true;
            }
            if (plugin.Configuration.showKofi == true)
            {
                if (ImGui.ImageButton(kofiBtnImg.ImGuiHandle, new Vector2(172, 27)))
                {
                    Util.OpenLink("https://ko-fi.com/infiniteroleplay");
                }
            }
            if (plugin.Configuration.showDisc == true)
            {
                if (ImGui.ImageButton(discoBtn.ImGuiHandle, new Vector2(172, 27)))
                {
                    Util.OpenLink("https://discord.gg/JN5BcHDnHp");
                }
            }


        }
        if (forgot == true)
        {
            ImGui.InputTextWithHint("##RegisteredEmail", $"Email", ref this.restorationEmail, 100);
            if (ImGui.Button("Submit Request"))
            {
                if (plugin.IsLoggedIn())
                {
                    DataSender.SendRestorationRequest(this.restorationEmail);
                }
            }

            if (ImGui.Button("Back"))
            {
                login = true;
                register = false;
                forgot = false;
            }

        }
        if (register == true)
        {

            ImGui.InputTextWithHint("##username", $"Username", ref this.registerUser, 100);
            ImGui.InputTextWithHint("##passver", $"Password", ref this.registerPassword, 100, ImGuiInputTextFlags.Password);
            ImGui.InputTextWithHint("##regpassver", $"Verify Password", ref this.registerVerPassword, 100, ImGuiInputTextFlags.Password);
            ImGui.InputTextWithHint("##email", $"Email", ref this.email, 100);
            ImGui.Checkbox("I am atleast 18 years of age", ref Agree18);
            ImGui.Checkbox("I agree to the TOS.", ref AgreeTOS);
            if (ImGui.Button("View ToS & Rules"))
            {
                plugin.OpenTermsWindow();
            }
            if (Agree18 == true && AgreeTOS == true)
            {
                if (ImGui.Button("Register Account"))
                {
                    if (registerPassword == registerVerPassword)
                    {
                        plugin.Configuration.username = registerUser;
                        if (plugin.IsLoggedIn())
                        {
                            DataSender.Register(registerUser, registerPassword, email);
                        }
                       
                    }

                }
            }
            if (ImGui.Button("Back"))
            {
                login = true;
                register = false;
            }

        }
        if (viewMainWindow == true)
        {
            login = false;
            forgot = false;
            register = false;
            
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
            if (ImGui.ImageButton(this.connectionsSectionImage.ImGuiHandle, new Vector2(100, 50)))
            {
                DataSender.RequestConnections(plugin.Configuration.username.ToString());

            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Connections");
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
                plugin.OpenOptionsWindow();
            }
            if (ImGui.Button("Logout", new Vector2(225, 25)))
            {
                plugin.newConnection = false;
                plugin.UnloadConnectionsBar();
                LoggedIN = false;
                switchUI();
                login = true;
                viewMainWindow = false;
                status = "Logged Out";
                statusColor = new Vector4(255, 0, 0, 255);
            }
        }
        if (viewProfile == true)
        {
            if (ImGui.ImageButton(this.profileImage.ImGuiHandle, new Vector2(100, 50)))
            {
                if (plugin.IsLoggedIn())
                {
                    //FETCH USER AND PASS ASEWLL
                    DataSender.FetchProfile(plugin.ClientState.LocalPlayer.Name.ToString(), plugin.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString());
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Manage your profile");
            }
            ImGui.SameLine();
            if (ImGui.ImageButton(this.profileBookmarkImage.ImGuiHandle, new Vector2(100, 50)))
            {
                if (plugin.IsLoggedIn())
                {
                    DataSender.RequestBookmarks(plugin.Configuration.username);
                }
               
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


        if (viewProfile == true || viewSystems == true || viewEvents == true || viewConnections == true)
        {
            if (ImGui.Button("Back"))
            {
                switchUI();
                viewMainWindow = true;
            }
        }
        ImGui.TextColored(serverStatusColor, serverStatus);
        ImGui.SameLine();
        if (ImGui.ImageButton(reconnectImage.ImGuiHandle, new Vector2(18, 18)))
        {
            plugin.Connect();
            plugin.UpdateStatus();
        }
        ImGui.TextColored(statusColor, status);

        
    }
    public void SaveLoginPreferences()
    {
        plugin.Configuration.rememberInformation = Remember;
        if (plugin.Configuration.rememberInformation == true)
        {
            plugin.Configuration.username = username;
            plugin.Configuration.password = password;
        }
        else
        {
            plugin.Configuration.username = string.Empty;
            plugin.Configuration.password = string.Empty;
        }
        plugin.Configuration.Save();
    }
    public void AttemptLogin()
    {
        LoggedIN = true;
        DataSender.Login(plugin.Configuration.username, plugin.Configuration.password, plugin.ClientState.LocalPlayer.Name.ToString(), plugin.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString());
    }
    public void switchUI()
    {
        viewProfile = false;
        viewSystems = false;
        viewEvents = false;
        viewConnections = false;
    }

}
