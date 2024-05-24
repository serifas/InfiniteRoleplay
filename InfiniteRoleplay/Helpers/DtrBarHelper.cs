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
        public static bool BarAdded = false;
        public static void AddIconToDtrBar(Plugin plugin, IDtrBar dtrService)
        {
            BarAdded = true;
            dtrBar = dtrService;
            if (dtrBar.Get("Infinite Roleplay") is not { } entry) return;
            dtrBarEntry = entry;
            string text = "\uE03E";
            dtrBarEntry.Text = text;
            entry.OnClick = () => plugin.ToggleMainUI();
            string connectionStatus = ClientTCP.GetConnectionStatus(ClientTCP.clientSocket);
            dtrBarEntry.Tooltip = new SeStringBuilder().AddText($"Infinite Rolepaly: {connectionStatus}").Build();
        }


        public static void DisposeBar()
        {
            BarAdded = false;
        }
       
    }

}
