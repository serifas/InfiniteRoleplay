using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using InfiniteRoleplay.Windows;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Numerics;
using InfiniteRoleplay;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Dalamud.Game;
using System.Runtime;
using Dalamud.Game.DutyState;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets;
using Networking;
using InfiniteRoleplay.Helpers;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Internal.Windows;
using Dalamud.Interface.Internal;
using System.Threading;
using System;
using OtterGui.Log;

namespace InfiniteRoleplay
{
    public sealed class Plugin : IDalamudPlugin
    {

        public bool loggedIn = false;
        public bool toggleconnection;
        public bool targeted = false;
        public bool targetMenuClosed = true;
        public bool loadCallback = false;
        public bool loadPreview = false;
        public bool uiLoaded = false;
        public string socketStatus;
        public DalamudPluginInterface PluginInterfacePub;
        public TargetWindow targetWindow;
        public TargetMenu targetMenu;
        public ImagePreview imagePreview;
        public BookmarksWindow bookmarksWindow;
        public PanelWindow panelWindow;
        public LoginWindow loginWindow;
        public ProfileWindow profileWindow;
        public OptionsWindow optionsWindow;
        public AdminWindow adminWindow;
        public ReportWindow reportWindow;
        public VerificationWindow verificationWindow;
        public RestorationWindow restorationWindow;
        public IDalamudTextureWrap AvatarHolder;
        //Icons
        public IDalamudTextureWrap lawfulGood;
        public IDalamudTextureWrap neutralGood;
        public IDalamudTextureWrap chaoticGood;
        public IDalamudTextureWrap lawfulNeutral;
        public IDalamudTextureWrap trueNeutral;
        public IDalamudTextureWrap chaoticNeutral;
        public IDalamudTextureWrap lawfulEvil;
        public IDalamudTextureWrap neutralEvil;
        public IDalamudTextureWrap chaoticEvil;
        public TOS termsWindow;
        public static Misc misc = new Misc();
        public string Name => "Infinite Roleplay";
        private const string CommandName = "/infinite";
        private const string TargetWindowCommandName = "/inftarget";
        private DalamudPluginInterface pluginInterface { get; init; }
        public Dalamud dalamud { get; init; }
        public ITargetManager targetManager { get; init; }
        public IClientState clientState { get; init; }
        public static IClientState _clientState;
        private IFramework framework { get; init; }
        public IChatGui chatGUI { get; init; }
        private IDutyState dutyState { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("InfiniteRoleplay");
        public Plugin([RequiredVersion("1.0")] IClientState ClientState,
                      [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
                      [RequiredVersion("1.0")] IFramework framework,
                      [RequiredVersion("1.0")] ITargetManager targetManager,
                      [RequiredVersion("1.0")] IDutyState dutyState,
                      [RequiredVersion("1.0")] ICommandManager commandManager,
                      [RequiredVersion("1.0")] IChatGui chatG)
        {
            this.pluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterfacePub = pluginInterface;
            this.clientState = ClientState;
            _clientState = ClientState;
            this.targetManager = targetManager;
            this.framework = framework;
            this.chatGUI = chatG;
            this.dalamud = dalamud;
            this.dutyState = dutyState;
            this.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(pluginInterface);
            DataSender.plugin = this;
            ClientTCP.plugin = this;
            Misc.pg = this;
            string name = "";




            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "to open the plugin panel window"
            });
            this.CommandManager.AddHandler(TargetWindowCommandName, new CommandInfo(OnViewTarget)
            {
                HelpMessage = "to open the target menu"
            });
            this.pluginInterface.UiBuilder.Draw += DrawUI;
            this.pluginInterface.UiBuilder.OpenConfigUi += LoadOptions;
            this.pluginInterface.UiBuilder.OpenMainUi += DrawLoginUI;
            
            DataReceiver.plugin = this;
            this.framework.Update += Update;

        }
        public async void ReloadClient()
        {
            ProfileWindow.playerCharacter = this.clientState.LocalPlayer;
            PanelWindow.playerCharacter = this.clientState.LocalPlayer;
            PanelWindow.targetManager = this.targetManager;
            await ClientTCP.CheckStatus();
        }
        public void ReloadTarget()
        {
            DataReceiver.TargetBioLoadStatus = -1;
            DataReceiver.TargetGalleryLoadStatus = -1;
            DataReceiver.TargetHooksLoadStatus = -1;
            DataReceiver.TargetStoryLoadStatus = -1;
            DataReceiver.TargetNotesLoadStatus = -1;
        }
        public void LoadOptions()
        {           
            LoadUI();
            optionsWindow.IsOpen= true;
        }
        public void ReloadProfile()
        {
            DataReceiver.BioLoadStatus = -1;
            DataReceiver.GalleryLoadStatus = -1;
            DataReceiver.HooksLoadStatus = -1;
            DataReceiver.StoryLoadStatus = -1;
        }
        public void UnloadUI()
        {
            if(uiLoaded == true)
            {
                this.WindowSystem.RemoveAllWindows();
                uiLoaded = false;
            }           
        }
        public void LoadUI()
        {
            if (uiLoaded == false)
            {
                AvatarHolder = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/common/avatar_holder.png"));
                
                lawfulGood = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/lawful_good.png"));
                neutralGood = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/neutral_good.png"));
                chaoticGood = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/chaotic_good.png"));
                lawfulNeutral = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/lawful_neutral.png"));
                trueNeutral = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/true_neutral.png"));
                chaoticNeutral = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/chaotic_neutral.png"));
                lawfulEvil = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/lawful_evil.png"));
                neutralEvil = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/neutral_evil.png"));
                chaoticEvil = pluginInterface.UiBuilder.LoadImage(Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "UI/alignments/chaotic_evil.png"));







                targetWindow = new TargetWindow(this, this.pluginInterface, AvatarHolder,  lawfulGood, neutralGood, chaoticGood, lawfulNeutral, trueNeutral, chaoticNeutral, lawfulEvil, neutralEvil, chaoticEvil);
                targetMenu = new TargetMenu(this, this.pluginInterface, targetManager);

                imagePreview = new ImagePreview(this, this.pluginInterface, targetManager);

                bookmarksWindow = new BookmarksWindow(this, this.pluginInterface, targetWindow);

                optionsWindow = new OptionsWindow(this, this.pluginInterface);

                reportWindow = new ReportWindow(this, this.pluginInterface);

                adminWindow = new AdminWindow(this, this.pluginInterface);

                panelWindow = new PanelWindow(this, this.pluginInterface, targetManager);

                loginWindow = new LoginWindow(this, this.clientState.LocalPlayer);
                profileWindow = new ProfileWindow(this, this.pluginInterface, chatGUI, this.Configuration);
                restorationWindow = new RestorationWindow(this, this.pluginInterface);
                verificationWindow = new VerificationWindow(this, this.pluginInterface);
                termsWindow = new TOS(this, this.pluginInterface);
                // this.WindowSystem.AddWindow(new Loader(this.pluginInterface, this));
                // this.WindowSystem.AddWindow(new SystemsWindow(this));
                this.WindowSystem.AddWindow(profileWindow);

                //  this.WindowSystem.AddWindow(new Rulebook(this));
                this.WindowSystem.AddWindow(loginWindow);
                this.WindowSystem.AddWindow(optionsWindow);
                //this.WindowSystem.AddWindow(new SystemsWindow(this));
                this.WindowSystem.AddWindow(panelWindow);
                //   this.WindowSystem.AddWindow(new MessageBox(this));
                //  this.WindowSystem.AddWindow(new AdminWindow(this, this.pluginInterface));
                this.WindowSystem.AddWindow(targetWindow);
                this.WindowSystem.AddWindow(targetMenu);
                this.WindowSystem.AddWindow(bookmarksWindow);
                this.WindowSystem.AddWindow(imagePreview);
                this.WindowSystem.AddWindow(reportWindow);
                this.WindowSystem.AddWindow(verificationWindow);
                this.WindowSystem.AddWindow(restorationWindow);
                this.WindowSystem.AddWindow(termsWindow);
                uiLoaded = true;

            }
        }
        public void Dispose()
        {
            this.pluginInterface.UiBuilder.Draw -= DrawUI;
            this.pluginInterface.UiBuilder.OpenConfigUi -= LoadOptions;
            this.pluginInterface.UiBuilder.OpenMainUi -= DrawLoginUI;
            this.framework.Update -= Update;

            this.CommandManager.RemoveHandler(CommandName);
            this.CommandManager.RemoveHandler(TargetWindowCommandName);
            this.WindowSystem.RemoveAllWindows();
            if(ClientHandleData.packets.Count > 0)
            {
                ClientHandleData.InitializePackets(false);
            }
            if (ClientTCP.IsConnectedToServer(ClientTCP.clientSocket) == true)
            {
                DisconnectFromServer();
            }
            if(AvatarHolder!= null) { AvatarHolder.Dispose(); }


            if (lawfulGood != null)
            {
                lawfulGood.Dispose();
            }
            if (neutralGood != null)
            {
                neutralGood.Dispose();
            }
            if (chaoticGood != null)
            {
                chaoticGood.Dispose();
            }
            if (lawfulNeutral != null)
            {
                lawfulNeutral.Dispose();
            }
            if (trueNeutral != null)
            {
                trueNeutral.Dispose();
            }
            if (chaoticNeutral != null)
            {
                chaoticNeutral.Dispose();
            }
            if (lawfulEvil != null)
            {
                lawfulEvil.Dispose();
            }
            if (neutralEvil != null)
            {
                neutralEvil.Dispose();
            }
            if (chaoticEvil != null)
            {
                chaoticEvil.Dispose();
            }
            Imaging.RemoveAllImages(this);

        }
        public void CloseAllWindows(bool closeLogin)
        { 
            if(closeLogin == true)
            {
                loginWindow.IsOpen = false;
            }
            profileWindow.IsOpen = false;
            bookmarksWindow.IsOpen = false;
            imagePreview.IsOpen = false;
            targetMenu.IsOpen = false;
            targetWindow.IsOpen = false;
            panelWindow.IsOpen = false;
            restorationWindow.IsOpen = false;
            verificationWindow.IsOpen = false;
        }


        public void Update(IFramework framework)
        {
            var targetPlayer = targetManager.Target as PlayerCharacter;
            if (loggedIn == true)
            {
                if (targetPlayer != null && dutyState.IsDutyStarted == false && Configuration.showTargetOptions == true)
                {
                    targetMenu.IsOpen = true;
                }
                else
                {
                    if (targeted == false)
                    {
                        targetMenu.IsOpen = false;
                    }
                }
            }
            if (loadPreview == true)
            {
                imagePreview.IsOpen = true;
                loadPreview = false;
            }
            
        }

        private void OnCommand(string command, string args)
        {
            DrawLoginUI();
            // in response to the slash command, just display our main ui          
        }
        private void OnViewTarget(string command, string args)
        {
            var targetPlayer = targetManager.Target as PlayerCharacter;
            if (loggedIn == true)
            {
                if (targetPlayer != null && dutyState.IsDutyStarted == false)
                {

                    targeted = true;
                    this.targetMenu.IsOpen = true;
                }
            }
        }
        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

      

        public bool IsLoggedIn()
        {
            if (clientState.IsLoggedIn == true && clientState.LocalPlayer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void DisconnectFromServer()
        {
            await ClientTCP.InitializingNetworking(false);
        }
        public static void ClearPackets()
        {
            ClientHandleData.InitializePackets(false);
        }
        public void DrawLoginUI()
        {
            LoadUI();            
            ReloadClient();
            if (loggedIn == true)
            {
                panelWindow.IsOpen = true;
                loginWindow.IsOpen = false;
            }
            else
            {
                CloseAllWindows(false);
                loginWindow.IsOpen = true;
            }
            if(loginWindow.IsOpen == false && loggedIn == false)
            {
                loginWindow.IsOpen = true;
            }
        }
        




    }
}
