using Dalamud.Interface.Colors;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using ImGuiScene;
using InfiniteRoleplay;
using OtterGui.Raii;
using OtterGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dalamud.Interface.GameFonts;
using Dalamud.Game.Gui.Dtr;
using Microsoft.VisualBasic;
using Networking;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Internal;
using Lumina.Excel.GeneratedSheets;

namespace InfiniteRoleplay.Windows
{
    public class ReportWindow : Window, IDisposable
    {
        public static string reportCharacterName;
        public static string reportCharacterWorld;
        public static string reportInfo = string.Empty;
        public static string reportStatus = string.Empty;
        public static Plugin pg;
        public ReportWindow(Plugin plugin, DalamudPluginInterface Interface) : base(
       "REPORT USER PROFILE", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(400, 400),
                MaximumSize = new Vector2(1200, 950)
            };
            pg = plugin;
          
        }
        public override void Draw()
        {
            ImGui.TextColored(new Vector4(100, 0, 0, 100), reportStatus);
            ImGui.Text("Reason for report");
            ImGui.InputTextMultiline("##info", ref reportInfo, 500, new Vector2(400, 100));
            if (ImGui.Button("Report!"))
            {
                if(reportInfo.Length > 15)
                {
                    DataSender.ReportProfile(pg.Configuration.username, pg.Configuration.password, reportInfo);
                }
                else
                {
                    reportStatus = "Please give a reason for the report.";
                }
            }
        }
        public void Dispose()
        {
            reportInfo = string.Empty;
            reportCharacterName = string.Empty;
            reportCharacterWorld = string.Empty;
        }
    }

}
