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
using System.Reflection.Metadata;

namespace InfiniteRoleplay.Windows
{
    public class VerificationWindow : Window, IDisposable
    {
        private GameFontHandle _nameFont;
        private float _modVersionWidth;
        public static Plugin pg;
        public static string verificationKey = string.Empty;
        public static string verificationStatus = string.Empty;
        public static Vector4 verificationCol = new Vector4(1, 1, 1, 1);
        public VerificationWindow(Plugin plugin, DalamudPluginInterface Interface) : base(
       "VERIFICATION", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(350, 200),
                MaximumSize = new Vector2(350, 200)
            };
            pg = plugin;
            this._nameFont = plugin.PluginInterfacePub.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
          
        }
        public override void Draw()
        {
            using var col = ImRaii.PushColor(ImGuiCol.Border, ImGuiColors.DalamudViolet);
            using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameBorderSize, 2 * ImGuiHelpers.GlobalScale);
            using var font = ImRaii.PushFont(_nameFont.ImFont, _nameFont.Available);
            ImGuiUtil.DrawTextButton("Verification", Vector2.Zero, 0);
            //set everything back
            using var defCol = ImRaii.DefaultColors();
            using var defStyle = ImRaii.DefaultStyle();
            using var defFont = ImRaii.DefaultFont();
            //okay that's done.
            ImGui.Text("We sent a verification key to the email provided. \nPlease provide it below...");
            ImGui.Spacing();
            //now for some simple toggles
            ImGui.InputText("Key", ref verificationKey, 10);
            if (ImGui.Button("Submit"))
            {
                DataSender.SendVerification(pg.Configuration.username, verificationKey);
            }
            ImGui.TextColored(verificationCol, verificationStatus);
        }
        public void Dispose()
        {

        }
    }

}
