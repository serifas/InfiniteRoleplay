using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using InfiniteRoleplay.Windows;
using Dalamud.Game.ClientState.Objects;
using System.Runtime.InteropServices;
using System.Timers;
using Networking;
using FFXIVClientStructs.FFXIV.Client.UI;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using System;
using ImGuiNET;
using System.Numerics;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Game.ClientState.Conditions;
using InfiniteRoleplay.Helpers;
namespace InfiniteRoleplay;

public partial class Plugin : IDalamudPlugin
{
    private const string CommandName = "/infinite";
    public bool loggedIn;
    public bool PluginLoaded = false;
    private readonly IDtrBar dtrBar;

    private DtrBarEntry? dtrBarEntry;
    public static bool BarAdded = false;
    public DalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    public IFramework Framework { get; init; }
    private IContextMenu ContextMenu { get; init; }
    public ICondition Condition { get; init; }
    [LibraryImport("user32")]
    internal static partial short GetKeyState(int nVirtKey);
    public static bool CtrlPressed() => (GetKeyState(0xA2) & 0x8000) != 0 || (GetKeyState(0xA3) & 0x8000) != 0;
    private ITargetManager TargetManager { get; init; }
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Infinite Roleplay");
    public OptionsWindow OptionsWindow { get; init; }
    public VerificationWindow VerificationWindow { get; init; }
    public RestorationWindow RestorationWindow { get; init; }
    public ReportWindow ReportWindow { get; init; }
    public MainPanel MainPanel { get; init; }
    public ProfileWindow ProfileWindow { get; init; }
    public BookmarksWindow BookmarksWindow { get; init; }
    public TargetWindow TargetWindow { get; init; }
    public ImagePreview ImagePreview { get; init; }
    public TOS TermsWindow { get; init; }
    public IClientState ClientState { get; init; }
    public bool barLoaded = false;

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
        TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        ClientState = clientState;
        TargetManager = targetManager;
        ContextMenu = contextMenu;
        this.dtrBar = dtrBar;
        this.Condition = condition;

        // Subscribe to condition change events
        framework = framework;
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        DataReceiver.plugin = this;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Type /infinite to open the plugin window."
        });
        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        Configuration.Initialize(PluginInterface);
        PluginInterface.UiBuilder.Draw += DrawUI;
        OptionsWindow = new OptionsWindow(this);
        MainPanel = new MainPanel(this);
        TermsWindow = new TOS(this);
        ProfileWindow = new ProfileWindow(this, this.ClientState.LocalPlayer);
        ImagePreview = new ImagePreview(this);
        BookmarksWindow = new BookmarksWindow(this);
        TargetWindow = new TargetWindow(this);
        VerificationWindow = new VerificationWindow(this);
        RestorationWindow = new RestorationWindow(this);
        ReportWindow = new ReportWindow(this);
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        ContextMenu.OnMenuOpened += AddContextMenu;
        Condition.ConditionChange += OnConditionChange;
        ClientState.Logout += UnloadPlugin;
        ClientState.Login += CheckPlugin;
        ClientState.Login += ClientTCP.CheckStatus;
        if (ClientState.IsLoggedIn && ClientState.LocalPlayer != null)
        {
            CheckPlugin();
            ClientTCP.CheckStatus();
        }

    }

    private void OnConditionChange(ConditionFlag flag, bool value)
    {
        if (value && ClientState.IsLoggedIn && ClientState.LocalPlayer != null)
        {
            CheckPlugin();
            ClientTCP.CheckStatus();
        }
    }

    public void CheckPlugin()
    {       
        if (!PluginLoaded)
        {
            LoadPluginUI();
            ClientTCP.CheckStatus();
        }
    }
    private void UnloadPlugin()
    {
        if (PluginLoaded)
        {
            WindowSystem.RemoveAllWindows();
            ContextMenu.OnMenuOpened -= AddContextMenu;
            dtrBarEntry?.Remove();
            dtrBarEntry = null;
            PluginLoaded = false;
        }
    }
    private void LoadPluginUI()
    {
        WindowSystem.AddWindow(OptionsWindow);
        WindowSystem.AddWindow(MainPanel);
        WindowSystem.AddWindow(TermsWindow);
        WindowSystem.AddWindow(ImagePreview);
        WindowSystem.AddWindow(ProfileWindow);
        WindowSystem.AddWindow(TargetWindow);
        WindowSystem.AddWindow(BookmarksWindow);
        WindowSystem.AddWindow(VerificationWindow);
        WindowSystem.AddWindow(RestorationWindow);
        WindowSystem.AddWindow(ReportWindow);
        LoadDtrBar();
        PluginLoaded = true;
    }

    private static void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
    {
        // Mark the exception as observed to prevent it from being thrown by the finalizer thread
        e.SetObserved();
    }
   
    public void AddContextMenu(MenuOpenedArgs args)
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

    private void ViewProfile(MenuItemClickedArgs args)
    {
        try
        {
            var targetPlayer = TargetManager.Target as PlayerCharacter;
            string characterName = targetPlayer.Name.ToString();
            string characterWorld = targetPlayer.HomeWorld.GameData.Name.ToString();
            ReportWindow.reportCharacterName = characterName;
            ReportWindow.reportCharacterWorld = characterWorld;
            TargetWindow.characterNameVal = characterName;
            TargetWindow.characterWorldVal = characterWorld;
            TargetWindow.ReloadTarget();
            TargetWindow.IsOpen = true;
            DataSender.RequestTargetProfile(characterName, characterWorld, Configuration.username);
        }
        catch (Exception ex)
        {
            DataSender.PrintMessage("Error when viewing profile from context " + ex.ToString(), LogLevels.LogError);
        }
    }
    private void BookmarkProfile(MenuItemClickedArgs args)
    {
        var targetPlayer = TargetManager.Target as PlayerCharacter;
        DataSender.BookmarkPlayer(Configuration.username.ToString(), targetPlayer.Name.ToString(), targetPlayer.HomeWorld.GameData.Name.ToString());
    }
    public void LoadDtrBar()
    {
        if (dtrBar.Get("Infinite Roleplay") is not { } entry) return;
        dtrBarEntry = entry;
        string text = "\uE03E";
        dtrBarEntry.Text = text;
        dtrBarEntry.Tooltip = "Infinite Roleplay";
        entry.OnClick = () => this.MainPanel.Toggle();
        barLoaded = true;
    }
    public void Dispose()
    {
        // timer.Stop();
        //  timer.Dispose();
        UnloadPlugin();
        CommandManager.RemoveHandler(CommandName);
        Condition.ConditionChange -= OnConditionChange;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;
        ClientState.Logout -= UnloadPlugin;
        ClientState.Login -= CheckPlugin;
        Imaging.RemoveAllImages(this);
        PluginLoaded = false;
    }
    public void UpdateStatus()
    {        
        string connectionStatus = ClientTCP.GetConnectionStatus(ClientTCP.clientSocket);
        dtrBarEntry.Tooltip = new SeStringBuilder().AddText($"Infinite Rolepaly: {connectionStatus}").Build();
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
            if(window.IsOpen)
            {
                window.Toggle();
            }
        }
    }
   
    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => OptionsWindow.Toggle();
    public void ToggleMainUI() => MainPanel.Toggle();
}
