using Dalamud.Interface.Colors;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using OtterGui.Raii;
using OtterGui;
using System;
using Dalamud.Interface.GameFonts;
using Networking;
using Dalamud.Interface.Utility;
using Dalamud.IoC;

namespace InfiniteRoleplay.Windows
{
    public class RestorationWindow : Window, IDisposable
    {
        private GameFontHandle _nameFont;
        public static Plugin pg;
        public static string restorationKey = string.Empty;
        public static string restorationPass = string.Empty;
        public static string restorationPassConfirm = string.Empty;
        public static string restorationEmail = string.Empty;
        public static string restorationStatus = string.Empty;
        public static Vector4 restorationCol = new Vector4(1, 1, 1, 1);
        public RestorationWindow(Plugin plugin) : base(
       "RESTORATION", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(420, 350),
                MaximumSize = new Vector2(420, 350)
            };
            pg = plugin;
            this._nameFont = plugin.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
          
        }
        public override void Draw()
        {
            using var col = ImRaii.PushColor(ImGuiCol.Border, ImGuiColors.DalamudViolet);
            using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameBorderSize, 2 * ImGuiHelpers.GlobalScale);
            using var font = ImRaii.PushFont(_nameFont.ImFont, _nameFont.Available);
            ImGuiUtil.DrawTextButton("Account Restoration", Vector2.Zero, 0);
            //set everything back
            using var defCol = ImRaii.DefaultColors();
            using var defStyle = ImRaii.DefaultStyle();
            using var defFont = ImRaii.DefaultFont();
            //okay that's done.
            ImGui.Text("We sent a restoration key to the email address provided. \nPlease enter the key with a new username and password below.");
            ImGui.Spacing();
            //now for some simple toggles
            ImGui.InputText("Restoration Key", ref restorationKey, 10);
            ImGui.InputText("New Password", ref restorationPass, 30, ImGuiInputTextFlags.Password);
            ImGui.InputText("New Password Confirmation", ref restorationPassConfirm, 30, ImGuiInputTextFlags.Password);
            if (ImGui.Button("Submit"))
            {
                if(restorationKey != string.Empty && restorationPass != string.Empty && restorationPassConfirm != string.Empty)
                {
                    if (restorationPass == restorationPassConfirm)
                    {
                        DataSender.SendRestoration(restorationEmail, restorationPass, restorationKey);
                    }
                   

                }
               
            }
            ImGui.TextColored(restorationCol, restorationStatus);
        }
        public void Dispose()
        {

        }
    }

}
