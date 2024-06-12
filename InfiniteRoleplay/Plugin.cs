using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using InfiniteRoleplay.Windows;
using Dalamud.Game.ClientState.Objects;
using System.Runtime.InteropServices;
using Networking;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using System;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using System.Threading.Tasks;
using InfiniteRoleplay.Helpers;
using FFXIVClientStructs.Havok;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Drawing;
using Lumina.Extensions;
using System.Threading;
using System.Timers;
namespace InfiniteRoleplay
{ 
    public partial class Plugin : IDalamudPlugin
    {
        public static Plugin plugin;
        public string username;
        private const string CommandName = "/infinite";
        public bool loggedIn;
        public bool PluginLoaded = false;
        private readonly IDtrBar dtrBar;
        private DtrBarEntry? statusBarEntry;
        private DtrBarEntry? connectionsBarEntry;
        public static bool BarAdded = false;
        private float timer = 0f;
        public DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public IFramework Framework { get; init; }
        private IContextMenu ContextMenu { get; init; }
        public ICondition Condition { get; init; }
        public ITextureProvider TextureProvider { get; init; }
        [LibraryImport("user32")]
        internal static partial short GetKeyState(int nVirtKey);
        public static bool CtrlPressed() => (GetKeyState(0xA2) & 0x8000) != 0 || (GetKeyState(0xA3) & 0x8000) != 0;
        private ITargetManager TargetManager { get; init; }
        public Configuration Configuration { get; init; }

        private readonly WindowSystem WindowSystem = new("Infinite Roleplay");
        private OptionsWindow OptionsWindow { get; init; }
        private VerificationWindow VerificationWindow { get; init; }
        private RestorationWindow RestorationWindow { get; init; }
        private ReportWindow ReportWindow { get; init; }
        private MainPanel MainPanel { get; init; }
        private ProfileWindow ProfileWindow { get; init; }
        private BookmarksWindow BookmarksWindow { get; init; }
        private TargetWindow TargetWindow { get; init; }
        private ImagePreview ImagePreview { get; init; }
        private TOS TermsWindow { get; init; }
        private ConnectionsWindow ConnectionsWindow { get; init; }

        public IClientState ClientState { get; init; }
        public float BlinkInterval = 0.5f;
        public bool newConnection;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] ITextureProvider textureProvider,
            [RequiredVersion("1.0")] IClientState clientState,
            [RequiredVersion("1.0")] IContextMenu contextMenu,
            [RequiredVersion("1.0")] ITargetManager targetManager,
            [RequiredVersion("1.0")] IFramework framework,
            [RequiredVersion("1.0")] ICondition condition,
            [RequiredVersion("1.0")] IDtrBar dtrBar
            )
        {
            // Original Dalamud-related object creation
            plugin = this;
            // Wrap the original service with the proxy
            this.dtrBar = dtrBar;
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            ClientState = clientState;
            TargetManager = targetManager;
            ContextMenu = contextMenu;
            TextureProvider = textureProvider;
            this.Condition = condition;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            this.Framework = framework;
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            DataReceiver.plugin = this;
            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Type /infinite to open the plugin window."
            });
            OptionsWindow = new OptionsWindow(this);
            MainPanel = new MainPanel(this);
            TermsWindow = new TOS(this);
            ProfileWindow = new ProfileWindow(this);
            ImagePreview = new ImagePreview(this);
            BookmarksWindow = new BookmarksWindow(this);
            TargetWindow = new TargetWindow(this);
            VerificationWindow = new VerificationWindow(this);
            RestorationWindow = new RestorationWindow(this);
            ReportWindow = new ReportWindow(this);
            ConnectionsWindow = new ConnectionsWindow(this);
            Configuration.Initialize(PluginInterface);
            WindowSystem.AddWindow(OptionsWindow);
            WindowSystem.AddWindow(MainPanel);
            WindowSystem.AddWindow(TermsWindow);
            WindowSystem.AddWindow(ProfileWindow);
            WindowSystem.AddWindow(ImagePreview);
            WindowSystem.AddWindow(BookmarksWindow);
            WindowSystem.AddWindow(TargetWindow);
            WindowSystem.AddWindow(VerificationWindow);
            WindowSystem.AddWindow(RestorationWindow);
            WindowSystem.AddWindow(ReportWindow);
            WindowSystem.AddWindow(ConnectionsWindow);
            ConnectionsWindow.plugin = this;
            // Subscribe to condition change events
            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
            PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
            ClientState.Logout += Logout;
            ContextMenu.OnMenuOpened += AddContextMenu;
            ClientState.Login += LoadConnection;

            this.Framework.Update += OnUpdate;

            if (ClientState.IsLoggedIn && clientState.LocalPlayer != null)
            {
                LoadConnection();
            }
        }

        public void LoadConnection()
        {
            Connect();
            UpdateStatus();
            ToggleMainUI();
        }
        public void Connect()
        {
            if (IsLoggedIn())
            {
                LoadStatusBar();
                if (!ClientTCP.Connected)
                {
                    ClientTCP.AttemptConnect();
                }

            }
        }

        private void Logout()
        {
            connectionsBarEntry?.Dispose();
            connectionsBarEntry = null;
            statusBarEntry?.Dispose();
            statusBarEntry = null;
            MainPanel.status = "Logged Out";
            MainPanel.statusColor = new Vector4(255, 0, 0, 255);
            MainPanel.switchUI();
            MainPanel.login = true;
            if (ClientTCP.Connected)
            {
                ClientTCP.Disconnect();
            }

        }
        private void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Mark the exception as observed to prevent it from being thrown by the finalizer thread
            e.SetObserved();
            Framework.RunOnFrameworkThread(() =>
            {
                DataSender.PrintMessage("Exception handled" + e.Exception.Message, LogLevels.LogError);
            });
        }
        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle the unhandled exception here
            var exception = e.ExceptionObject as Exception;
            Framework.RunOnFrameworkThread(() =>
            {
                DataSender.PrintMessage("Exception handled" + exception.Message, LogLevels.LogError);
            });
        }
        public void AddContextMenu(MenuOpenedArgs args)
        {
            if (IsLoggedIn())
            {
                var targetPlayer = TargetManager.Target as PlayerCharacter;
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

        }

        private void ViewProfile(MenuItemClickedArgs args)
        {
            try
            {
                if (IsLoggedIn())
                {
                    var targetPlayer = TargetManager.Target as PlayerCharacter;
                    string characterName = targetPlayer.Name.ToString();
                    string characterWorld = targetPlayer.HomeWorld.GameData.Name.ToString();
                    ReportWindow.reportCharacterName = characterName;
                    ReportWindow.reportCharacterWorld = characterWorld;
                    TargetWindow.characterNameVal = characterName;
                    TargetWindow.characterWorldVal = characterWorld;
                    TargetWindow.ReloadTarget();
                    OpenTargetWindow();
                    DataSender.RequestTargetProfile(characterName, characterWorld, Configuration.username);
                }

            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error when viewing profile from context " + ex.ToString(), LogLevels.LogError);
            }
        }
        private void BookmarkProfile(MenuItemClickedArgs args)
        {
            if (IsLoggedIn())
            {
                var targetPlayer = TargetManager.Target as PlayerCharacter;
                DataSender.BookmarkPlayer(Configuration.username.ToString(), targetPlayer.Name.ToString(), targetPlayer.HomeWorld.GameData.Name.ToString());
            }
        }
        public void LoadStatusBar()
        {
            if (statusBarEntry == null)
            {
                string randomTitle = Misc.GenerateRandomString();
                if (dtrBar.Get(randomTitle) is not { } entry) return;
                statusBarEntry = entry;
                string icon = "\uE03E";
                statusBarEntry.Text = icon;
                statusBarEntry.Tooltip = "Infinite Roleplay";
                entry.OnClick = () => ToggleMainUI();
            }
        }
        public void LoadConnectionsBar(float deltaTime)
        {
            timer += deltaTime;
            float pulse = ((int)(timer / BlinkInterval) % 2 == 0) ? 14 : 0; // Alternate between 0 and 14 every BlinkInterval

            if (connectionsBarEntry == null)
            {
                string randomTitle = Misc.GenerateRandomString();
                if (dtrBar.Get(randomTitle) is not { } entry) return;
                connectionsBarEntry = entry;
                connectionsBarEntry.Tooltip = "New Connections Request";
                entry.OnClick = () => DataSender.RequestConnections(Configuration.username.ToString());                 
            }

            SeStringBuilder statusString = new SeStringBuilder();
            statusString.AddUiGlow((ushort)pulse); // Apply pulsing glow
            statusString.AddText("\uE070");
            statusString.AddUiGlow(0);
            SeString str = statusString.BuiltString;
            connectionsBarEntry.Text = str;
        }
        public void UnloadConnectionsBar()
        {
            if(connectionsBarEntry != null)
            {
                connectionsBarEntry?.Dispose();
                connectionsBarEntry = null;
            }
        }
        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            WindowSystem?.RemoveAllWindows();
            statusBarEntry?.Remove();
            statusBarEntry = null;
            connectionsBarEntry?.Dispose();
            connectionsBarEntry = null;
            CommandManager.RemoveHandler(CommandName);


            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
            PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;
            ClientState.Login -= Connect;
            ClientState.Login -= LoadConnection;
            ClientState.Logout -= Logout;
            ContextMenu.OnMenuOpened -= AddContextMenu; 
            this.Framework.Update -= OnUpdate;
            // Dispose all windows
            OptionsWindow?.Dispose();
            MainPanel?.Dispose();
            TermsWindow?.Dispose();
            ImagePreview?.Dispose();
            ProfileWindow?.Dispose();
            TargetWindow?.Dispose();
            BookmarksWindow?.Dispose();
            VerificationWindow?.Dispose();
            RestorationWindow?.Dispose();
            ReportWindow?.Dispose();
            ConnectionsWindow?.Dispose();
            Misc._nameFont?.Dispose();
            Imaging.RemoveAllImages(this);
        }
        private void OnUpdate(IFramework framework)
        {
            TimeSpan deltaTimeSpan = framework.UpdateDelta;
            float deltaTime = (float)deltaTimeSpan.TotalSeconds; // Convert deltaTime to seconds
            if(newConnection == true)
            {
                //if th
                LoadConnectionsBar(deltaTime);
            }
            
        }


        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            ToggleMainUI();
        }
        public void CloseAllWindows()
        {
            foreach (Window window in WindowSystem.Windows)
            {
                if (window.IsOpen)
                {
                    window.Toggle();
                }
            }
        }
        public bool IsLoggedIn()
        {
            bool loggedIn = false;
            if (ClientState.IsLoggedIn && ClientState.LocalPlayer != null)
            {
                loggedIn = true;
            }
            return loggedIn;
        }

        private void DrawUI() => WindowSystem.Draw();
        public void ToggleConfigUI() => OptionsWindow.Toggle();
        public void ToggleMainUI()
        {
            MainPanel.IsOpen = true;
            if (IsLoggedIn() && loggedIn == false)
            {
                MainPanel.AttemptLogin();
            }
        }
       
        
        public void OpenMainPanel() => MainPanel.IsOpen = true;
        public void OpenTermsWindow() => TermsWindow.IsOpen = true;
        public void OpenImagePreview() => ImagePreview.IsOpen = true;
        public void OpenProfileWindow() => ProfileWindow.IsOpen = true;
        public void OpenTargetWindow() => TargetWindow.IsOpen = true;
        public void OpenBookmarksWindow() => BookmarksWindow.IsOpen = true;
        public void OpenVerificationWindow() => VerificationWindow.IsOpen = true;
        public void OpenRestorationWindow() => RestorationWindow.IsOpen = true;
        public void OpenReportWindow() => ReportWindow.IsOpen = true;
        public void OpenOptionsWindow() => OptionsWindow.IsOpen = true;
        public void OpenConnectionsWindow() => ConnectionsWindow.IsOpen = true;

        internal async void UpdateStatus()
        {
            try
            {

                string connectionStatus = await ClientTCP.GetConnectionStatusAsync(ClientTCP.clientSocket);
                MainPanel.serverStatus = connectionStatus;
                if (ClientState.IsLoggedIn && ClientState.LocalPlayer != null)
                {
                    statusBarEntry.Tooltip = new SeStringBuilder().AddText($"Infinite Roleplay: {connectionStatus}").Build();
                }

            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error updating status: " + ex.ToString(), LogLevels.LogError);
            }
        }
    }
   
}
