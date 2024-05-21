using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using InfiniteRoleplay.Helpers;
using System;
using System.Net.Http;
using System.Numerics;
using InfiniteRoleplay;
namespace InfiniteRoleplay.Windows
{
    public class TOS : Window, IDisposable
    {
        private GameFontHandle _nameFont;
        private float _modVersionWidth;
        public static Plugin pg;
        public static string verificationKey = string.Empty;
        public static string verificationStatus = string.Empty;
        public static Vector4 verificationCol = new Vector4(1, 1, 1, 1);
        public static string ToS1, ToS2, Rules1, Rules2;
        public static bool load;
        public TOS(Plugin plugin) : base(
        "TERMS OF SERVICE")
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1200, 200),
                MaximumSize = new Vector2(1200, 1000)
            };
            pg = plugin;

            load = true;
            ToS1 = ReadTOS("https://raw.githubusercontent.com/serifas/infinite-roleplay-plugin/main/TOS1.txt");
            ToS2 = ReadTOS("https://raw.githubusercontent.com/serifas/infinite-roleplay-plugin/main/TOS2.txt");
            Rules1 = ReadTOS("https://raw.githubusercontent.com/serifas/infinite-roleplay-plugin/main/Rules1.txt");
            Rules2 = ReadTOS("https://raw.githubusercontent.com/serifas/infinite-roleplay-plugin/main/Rules2.txt");
        }
        public override async void Draw()
        {

            Misc.SetTitle(pg, true, "Terms of Service");
        //okay that's done.
            ImGui.Text(ToS1);
            ImGui.Text(ToS2);
            Misc.SetTitle(pg, true, "Rules");
            //now for some simple toggles
            ImGui.Text(Rules1);
            ImGui.Text(Rules2);
        }

        public void Dispose()
        {

        }
       
        static string ReadTOS(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }
    }
    
}
