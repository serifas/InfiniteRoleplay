using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using InfiniteRoleplay.Windows;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Networking;
using InfiniteRoleplay.Helpers;
using Dalamud.Plugin.Services;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using System.Runtime.InteropServices;
using System.Timers;
using System;
using Dalamud.Game.Text.SeStringHandling;
using System.Threading.Tasks;

namespace InfiniteRoleplay
{
    public sealed partial class Plugin : IDalamudPlugin
    {

        public bool loggedIn = false;
        public bool toggleconnection;
        public bool targeted = false;
        public bool targetMenuClosed = true;
        public bool loadCallback = false;
        public bool uiLoaded = false;
        public string socketStatus;
        public int tick = 0;
        public DalamudPluginInterface PluginInterfacePub;
        public TargetWindow targetWindow;
        public ImagePreview imagePreview;
        public BookmarksWindow bookmarksWindow;
        public PanelWindow panelWindow;
        public LoginWindow loginWindow;
        public ProfileWindow profileWindow;
        public OptionsWindow optionsWindow;
        public ReportWindow reportWindow;
        public VerificationWindow verificationWindow;
        public RestorationWindow restorationWindow;
        public TOS termsWindow;
        public static Misc misc = new Misc();
        public static Timer timer = new Timer(3000);
        public string Name => "Infinite Roleplay";
        private const string CommandName = "/infinite";
        private DalamudPluginInterface pluginInterface { get; init; }
        public ITargetManager targetManager { get; init; }
        public IClientState clientState { get; init; }
        public static IClientState _clientState;
        private IFramework framework { get; init; }
        public IChatGui chatGUI { get; init; }
        private IDutyState dutyState { get; init; }
        private IContextMenu ct { get; init; }
        private ICommandManager CommandManager { get; init; }
        [LibraryImport("user32")]
        internal static partial short GetKeyState(int nVirtKey);
        public static bool CtrlPressed() => (GetKeyState(0xA2) & 0x8000) != 0 || (GetKeyState(0xA3) & 0x8000) != 0;
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("InfiniteRoleplay");
        public Plugin([RequiredVersion("1.0")] IClientState ClientState,
                      [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
                      [RequiredVersion("1.0")] IFramework framework,
                      [RequiredVersion("1.0")] ITargetManager targetManager,
                      [RequiredVersion("1.0")] IDutyState dutyState,
                      [RequiredVersion("1.0")] ICommandManager commandManager,
                      [RequiredVersion("1.0")] IContextMenu contextMenu,
                      [RequiredVersion("1.0")] IChatGui chatG
                        )
        {
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            this.pluginInterface = pluginInterface;
            CommandManager = commandManager;
            PluginInterfacePub = pluginInterface;
            clientState = ClientState;
            _clientState = ClientState;
            this.targetManager = targetManager;
            this.framework = framework;
            chatGUI = chatG;
            this.ct = contextMenu;
            this.dutyState = dutyState;
            Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(pluginInterface);
            DataSender.plugin = this;
            ClientTCP.plugin = this;
            Misc.pg = this;
            string name = "";

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "to open the plugin panel window"
            });
            LoadUI();
            this.pluginInterface.UiBuilder.Draw += DrawUI;
            this.pluginInterface.UiBuilder.OpenConfigUi += LoadOptions;
            this.pluginInterface.UiBuilder.OpenMainUi += DrawLoginUI;
            this.ct.OnMenuOpened += AddContextMenu;
            DataReceiver.plugin = this;
            timer.Elapsed += CheckStatus;
            timer.Start();
        }

        private void CheckStatus(object? sender, ElapsedEventArgs e)
        {
            ClientTCP.CheckStatus();
        }

        private static void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Mark the exception as observed to prevent it from being thrown by the finalizer thread
            e.SetObserved();
        }
        public void AttemptLogin()
        {
            string username = Configuration.username.ToString();
            string password = Configuration.password.ToString();
            PlayerCharacter playerCharacter = this.clientState.LocalPlayer;
            if(Configuration.username.Length > 0 &&  Configuration.password.Length > 0)
            {
                DataSender.Login(username, password, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString());
            }
        }

        public void AddContextMenu(MenuOpenedArgs args)
        {
            var targetPlayer = targetManager.Target as PlayerCharacter;
            if (args.AddonPtr == (nint)0 && targetPlayer != null && loggedIn == true)
            {
                MenuItem view = new MenuItem();
                MenuItem bookmark = new MenuItem();
                view.Name = "View Infinite Profile";
                view.PrefixColor = 56;
                view.Prefix = SeIconChar.BoxedQuestionMark;
                bookmark.Name = "Bookmark Infinite Profile";
                bookmark.PrefixColor = 56;
                bookmark.Prefix = SeIconChar.BoxedPlus;
                view.OnClicked += ViewProfile;
                bookmark.OnClicked += BookmarkProfile;
                args.AddMenuItem(view);
                args.AddMenuItem(bookmark);

            }
        }

        private void ViewProfile(MenuItemClickedArgs args)
        {
            try { 
                var targetPlayer = targetManager.Target as PlayerCharacter;
                string characterName = targetPlayer.Name.ToString();
                string characterWorld = targetPlayer.HomeWorld.GameData.Name.ToString();
                ReportWindow.reportCharacterName = characterName;
                ReportWindow.reportCharacterWorld = characterWorld;
                TargetWindow.characterNameVal = characterName;
                TargetWindow.characterWorldVal = characterWorld;
                ReloadTarget();
                targetWindow.IsOpen = true;
                DataSender.RequestTargetProfile(characterName, characterWorld, Configuration.username);
            }
            catch(Exception ex)
            {
                DataSender.PrintMessage("Error when viewing profile from context " + ex.ToString(), LogLevels.LogError);
            }


        }
        private void BookmarkProfile(MenuItemClickedArgs args)
        {
            var targetPlayer = targetManager.Target as PlayerCharacter;
            DataSender.BookmarkPlayer(Configuration.username.ToString(), targetPlayer.Name.ToString(), targetPlayer.HomeWorld.GameData.Name.ToString());
        }

        public async void ReloadClient()
        {
            ProfileWindow.playerCharacter = this.clientState.LocalPlayer;
            PanelWindow.playerCharacter = this.clientState.LocalPlayer;
            PanelWindow.targetManager = this.targetManager;
            ClientTCP.CheckStatus();
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
            optionsWindow.IsOpen= true;
        }
        public void ReloadProfile()
        {
            DataReceiver.BioLoadStatus = -1;
            DataReceiver.GalleryLoadStatus = -1;
            DataReceiver.HooksLoadStatus = -1;
            DataReceiver.StoryLoadStatus = -1;
        }
       
        public void LoadUI()
        {
            if (uiLoaded == false)
            {
                try
                {
                    targetWindow = new TargetWindow(this, this.pluginInterface);

                    imagePreview = new ImagePreview(this, this.pluginInterface, targetManager);

                    bookmarksWindow = new BookmarksWindow(this, this.pluginInterface, targetWindow);

                    optionsWindow = new OptionsWindow(this, this.pluginInterface);

                    reportWindow = new ReportWindow(this, this.pluginInterface);


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
                    this.WindowSystem.AddWindow(bookmarksWindow);
                    this.WindowSystem.AddWindow(imagePreview);
                    this.WindowSystem.AddWindow(reportWindow);
                    this.WindowSystem.AddWindow(verificationWindow);
                    this.WindowSystem.AddWindow(restorationWindow);
                    this.WindowSystem.AddWindow(termsWindow);
                    uiLoaded = true;
                }
                catch(Exception ex)
                {
                    DataSender.PrintMessage("Unable to Load Plugin UI LoadUI Failed " + ex.ToString(), LogLevels.LogError);
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if(timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                // Unsubscribe from Dalamud events
                if (this.pluginInterface != null)
                {
                    this.pluginInterface.UiBuilder.Draw -= DrawUI;
                    this.pluginInterface.UiBuilder.OpenConfigUi -= LoadOptions;
                    this.pluginInterface.UiBuilder.OpenMainUi -= DrawLoginUI;

                }

                // Unsubscribe from custom events
                if (this.ct != null)
                {
                    this.ct.OnMenuOpened -= AddContextMenu;
                }
                if(this.clientState != null)
                {
                    this.clientState.Login -= ClientTCP.CheckStatus;
                }
                // Remove command handlers
                if (this.CommandManager != null)
                {
                    this.CommandManager.RemoveHandler(CommandName);
                }

                // Remove all windows managed by the window system
                this.WindowSystem?.RemoveAllWindows();

                // Disconnect the network client
                ClientTCP.Disconnect();

                // Clear packets if any are present
                if (ClientHandleData.packets.Count > 0)
                {
                    ClientHandleData.InitializePackets(false);
                }

                // Remove all loaded images associated with this plugin
                Imaging.RemoveAllImages(this);
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage($"Error disposing resources: {ex}", LogLevels.LogError);
            }
        }
        public void CloseAllWindows()
        { 
            if(uiLoaded == true)
            {
                try 
                { 
                    loginWindow.IsOpen = false;            
                    profileWindow.IsOpen = false;
                    bookmarksWindow.IsOpen = false;
                    imagePreview.IsOpen = false;
                    targetWindow.IsOpen = false;
                    panelWindow.IsOpen = false;
                    restorationWindow.IsOpen = false;
                    verificationWindow.IsOpen = false;
                }
                catch(Exception ex)
                {
                    DataSender.PrintMessage("Unable to close windows CloseAllWindows failed " + ex.ToString(), LogLevels.LogError);
                }
            }
        }


       
        private void OnCommand(string command, string args)
        {
            DrawLoginUI();
            // in response to the slash command, just display our main ui          
        }
       
        private void DrawUI()
        {
            this.WindowSystem.Draw();
            
        }

       
        public void DrawLoginUI()
        {

            if(clientState.IsLoggedIn && clientState.LocalPlayer != null)
            {
                ReloadClient();
                if (loggedIn == true)
                {
                    panelWindow.IsOpen = true;
                    loginWindow.IsOpen = false;
                }
                else
                {
                    CloseAllWindows();
                    loginWindow.IsOpen = true;
                }
                if (loginWindow.IsOpen == false && loggedIn == false)
                {
                    loginWindow.IsOpen = true;
                }
            }
         
        }
        




    }
}
