using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Lumina.Excel.GeneratedSheets2;
using Newtonsoft.Json.Linq;
using OtterGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;

namespace InfiniteRoleplay.Helpers
{
    public class Misc
    {
        public static float _modVersionWidth;
        public static GameFontHandle _nameFont;
        public static int loaderIndex = 0;
        public static IDalamudTextureWrap loaderAnimInd;
        public static Plugin pg; 
        public static float ConvertToPercentage(float value)
        {
            // Clamp the value between 0 and 100
            value = Math.Max(0f, Math.Min(100f, value));

            // Return the percentage
            return value / 100f * 100f;
        }
        public static void SetCenter(Plugin plugin, string name)
        {
         
                int NameWidth = name.Length * 6;
                var decidingWidth = Math.Max(500, ImGui.GetWindowWidth());
                var offsetWidth = (decidingWidth - NameWidth) / 2;
                var offsetVersion = name.Length > 0
                    ? _modVersionWidth + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().WindowPadding.X
                    : 0;
                var offset = Math.Max(offsetWidth, offsetVersion);
                if (offset > 0)
                {
                    ImGui.SetCursorPosX(offset);
                }
        }
        public static void SetTitle(Plugin plugin, bool center, string title)
        {
            _nameFont = plugin.PluginInterfacePub.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
            if(center == true){
                int NameWidth = title.Length * 10;
                var decidingWidth = Math.Max(500, ImGui.GetWindowWidth());
                var offsetWidth = (decidingWidth - NameWidth) / 2;
                var offsetVersion = title.Length > 0
                    ? _modVersionWidth + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().WindowPadding.X
                    : 0;
                var offset = Math.Max(offsetWidth, offsetVersion);
                if (offset > 0)
                {
                    ImGui.SetCursorPosX(offset);
                }
            }


            using var col = ImRaii.PushColor(ImGuiCol.Border, ImGuiColors.DalamudViolet);
            using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameBorderSize, 2 * ImGuiHelpers.GlobalScale);
            using var font = ImRaii.PushFont(_nameFont.ImFont, _nameFont.Available);
            ImGuiUtil.DrawTextButton(title, Vector2.Zero, 0);

            using var defInfFontDen = ImRaii.DefaultFont();
            using var defCol = ImRaii.DefaultColors();
            using var defStyle = ImRaii.DefaultStyle();
        }
        public static void StartLoader(float value, float max, string loading)
        {
            value = Math.Max(0f, Math.Min(100f, value));
            ImGui.ProgressBar(value / max, new Vector2(500, 20), "Loading " + loading);
        }
        public static byte[] RemoveBytes(byte[] input, byte[] pattern)
        {
            if (pattern.Length == 0) return input;
            var result = new List<byte>();
            for (int i = 0; i < input.Length; i++)
            {
                var patternLeft = i <= input.Length - pattern.Length;
                if (patternLeft && (!pattern.Where((t, j) => input[i + j] != t).Any()))
                {
                    i += pattern.Length - 1;
                }
                else
                {
                    result.Add(input[i]);
                }
            }
            return result.ToArray();
        }
        public static string GetBetween(string content, string startString, string endString)
        {
            int Start = 0, End = 0;
            if (content.Contains(startString) && content.Contains(endString))
            {
                Start = content.IndexOf(startString, 0) + startString.Length;
                End = content.IndexOf(endString, Start);
                return content.Substring(Start, End - Start);
            }
            else
                return string.Empty;
        }
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }
            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }
        

    }
}
