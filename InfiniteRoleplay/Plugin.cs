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
namespace InfiniteRoleplay;

public partial class Plugin : IDalamudPlugin
{
    private const string CommandName = "/infinite";
    public bool loggedIn;
    private readonly IDtrBar dtrBar;

    private DtrBarEntry? dtrBarEntry;
    public static bool BarAdded = false;
    public DalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    private IContextMenu ContextMenu { get; init; }

    [LibraryImport("user32")]
    internal static partial short GetKeyState(int nVirtKey);
    public static bool CtrlPressed() => (GetKeyState(0xA2) & 0x8000) != 0 || (GetKeyState(0xA3) & 0x8000) != 0;
    public ITargetManager TargetManager { get; init; }
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
    public Timer timer = new Timer(3000);
    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ICommandManager commandManager,
        [RequiredVersion("1.0")] ITextureProvider textureProvider,
        [RequiredVersion("1.0")] IClientState clientState,
        [RequiredVersion("1.0")] IContextMenu contextMenu,
        [RequiredVersion("1.0")] ITargetManager targetManager,
        [RequiredVersion("1.0")] IDtrBar dtrBar
        )
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        ClientState = clientState;
        TargetManager = targetManager;
        ContextMenu = contextMenu;
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        DataReceiver.plugin = this;
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
        this.dtrBar = dtrBar;
        this.ContextMenu.OnMenuOpened += AddContextMenu;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Type /infinite to open the plugin window."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        this.ClientState.Logout += OnLogout;
        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        timer.Elapsed += CheckConnectionStatus;
        timer.Start();

    }


    private void CheckConnectionStatus(object? sender, ElapsedEventArgs e)
    {   
        ClientTCP.CheckStatus();
        UpdateStatus();
        if(ClientState.IsLoggedIn && ClientState.LocalPlayer != null)
        {
            LoadDtrBar();
        }
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
        entry.OnClick = () => this.MainPanel.Toggle();
    }
    private void OnLogout()
    {
        timer.Stop();
        dtrBarEntry?.Remove();
        dtrBarEntry = null;
    }

    public void Dispose()
    {
        timer.Stop();
        timer.Dispose();
        WindowSystem.RemoveAllWindows();
        CommandManager.RemoveHandler(CommandName);
        ClientState.Logout -= OnLogout;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
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
