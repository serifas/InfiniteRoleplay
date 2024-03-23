using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Interface.ImGuiFileDialog;
using Networking;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Internal;
using Dalamud.Utility;
using InfiniteRoleplay.Scripts.Misc;

namespace InfiniteRoleplay.Windows;

public class LoginWindow : Window, IDisposable
{
    private Configuration Configuration;
    public string username = string.Empty;
    public string password = string.Empty;
    public string registerUser = string.Empty;
    public string registerPassword = string.Empty;
    public string registerVerPassword = string.Empty;
    public string email = string.Empty;
    public string restorationEmail = string.Empty;
    public bool attemptedLogin = false;
    public bool login = true;
    public bool forgot = false;
    public static bool loginRequest = false;
    public bool register = false;
    public bool AgreeTOS = false;
    public bool Agree18 = false;
    public static IDalamudTextureWrap kofiBtnImg, discoBtn;
    private PlayerCharacter playerCharacter;
    public static string status = "status...";
    public static Vector4 statusColor = new Vector4(255, 255, 255, 255);
    public Plugin plugin;
    public LoginWindow(Plugin plugin, PlayerCharacter playerCharacter) : base(
        "LOGIN",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(250, 310);
        this.SizeCondition = ImGuiCond.Always;
        this.plugin = plugin;
        this.Configuration = plugin.Configuration;
        this.username = this.Configuration.username;
        this.password = this.Configuration.password;

        kofiBtnImg = Constants.UICommonImage(plugin.PluginInterfacePub, Constants.CommonImageTypes.kofiBtn);
        discoBtn = Constants.UICommonImage(plugin.PluginInterfacePub, Constants.CommonImageTypes.discordBtn);

        this.playerCharacter = playerCharacter;
    }

    public void Dispose()
    {
        kofiBtnImg.Dispose();
        discoBtn.Dispose();
    }

    public override void Draw()
    {

        // can't ref a property, so use a local copy
        if (login == true)
        {
            if(ClientTCP.clientSocket.Connected == true)
            {
                ImGui.InputTextWithHint("##username", $"Username", ref this.username, 100);
                ImGui.InputTextWithHint("##password", $"Password", ref this.password, 100, ImGuiInputTextFlags.Password);

                if (ImGui.Button("Login"))
                {
                    this.Configuration.username = this.username;
                    this.Configuration.password = this.password;
                    this.Configuration.Save();
                    DataSender.Login(username, password, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString());
                }
                ImGui.SameLine();
                if (ImGui.Button("Forgot"))
                {
                    login = false;
                    register = false;
                    forgot = true;
                }
                if (ImGui.Button("Register"))
                {
                    login = false;
                    register = true;
                }
                if (Configuration.showKofi == true)
                {
                    if (ImGui.ImageButton(kofiBtnImg.ImGuiHandle, new Vector2(172, 27)))
                    {
                        Util.OpenLink("https://ko-fi.com/infiniteroleplay");
                    }
                }
                if (Configuration.showDisc == true)
                {
                    if (ImGui.ImageButton(discoBtn.ImGuiHandle, new Vector2(172, 27)))
                    {
                        Util.OpenLink("https://discord.gg/infinite-roleplay");
                    }
                }
            }
            else
            {
                ImGui.Text("Loading...");
            }
        }
        if(forgot == true)
        {
            ImGui.InputTextWithHint("##RegisteredEmail", $"Email", ref this.restorationEmail, 100);
            if(ImGui.Button("Submit Request"))
            {
                DataSender.SendRestorationRequest(this.restorationEmail);
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
                plugin.termsWindow.IsOpen = true;
            }
            if (Agree18 == true && AgreeTOS == true)
            {
                if (ImGui.Button("Register Account"))
                {
                    if (registerPassword == registerVerPassword)
                    {
                        DataSender.Register(registerUser, registerPassword, email);
                        login = true;
                        register = false;
                    }

                }
            }
            if (ImGui.Button("Back"))
            {
                login = true;
                register = false;
            }

        }
        ImGui.TextColored(statusColor, status);
    }
    public override void Update()
    {
    }


}
