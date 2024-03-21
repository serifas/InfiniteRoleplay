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

namespace InfiniteRoleplay.Windows
{
    public class AdminWindow : Window, IDisposable
    {

        public AdminWindow(Plugin plugin, DalamudPluginInterface Interface) : base(
       "ADMINISTRATION", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1200, 950),
                MaximumSize = new Vector2(1200, 950)
            };
          
        }
        public override void Draw()
        {
            

        }
        public void Dispose()
        {

        }
        public override void Update()
        {
            
        }
    }

}
