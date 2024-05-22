using Dalamud.Configuration;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using InfiniteRoleplay;
using Networking;
using Dalamud.Game.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Timers;
using System;
using System.Data;
namespace InfiniteRoleplay
{

    public class DtrBarHelper
    {
        private static DtrBarEntry? dtrBarEntry;
        private static IDtrBar dtrBar;
        private static Timer timer = new Timer(3000);
        public static void AddIconToDtrBar(Plugin plugin, IDtrBar dtrService)
        {
            dtrBar = dtrService;
            if (dtrBar.Get("Who Am I Again?") is not { } entry) return;
            timer.Elapsed += CheckStatus;
            dtrBarEntry = entry;
            string text = "\uE03E";
            dtrBarEntry.Text = text;
            timer.Start();
            entry.OnClick = () => plugin.MainPanel.Toggle();
        }

        private static void CheckStatus(object? sender, ElapsedEventArgs e)
        {
            string connectionStatus = ClientTCP.GetConnectionStatus(ClientTCP.clientSocket);
            dtrBarEntry.Tooltip = new SeStringBuilder().AddText($"{connectionStatus}").Build();
        }

        public static void DisposeBar()
        {
            dtrBarEntry?.Remove();
            dtrBarEntry = null;
        }
       
    }

}
