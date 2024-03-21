using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using static Dalamud.Interface.Windowing.Window;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ImGuiFileDialog;
using ImGuiNET;
using ImGuiScene;
using static Lumina.Data.Files.ScdFile;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState;
using Networking;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Internal;

namespace InfiniteRoleplay.Windows
{
    public class ImagePreview : Window, IDisposable
    {
        public static IDalamudTextureWrap PreviewImage;
        public static bool isAdmin;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        public static ITargetManager targetManager;
        public static PlayerCharacter playerCharacter;
        public static PlayerCharacter lastTarget;
        public bool openedProfile = false;
        public static int width = 0, height = 0;
        public bool openedTargetProfile = false;
        public ImagePreview(Plugin plugin, DalamudPluginInterface Interface, ITargetManager targetManager) : base(
       "PREVIEW", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.configuration = plugin.Configuration;
        }

        public override void Draw()
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(width, height),
                MaximumSize = new Vector2(width, height)
            };
            ImGui.Image(PreviewImage.ImGuiHandle, new Vector2(width, height));
        }


        public void Dispose()
        {
            WindowOpen = false;
            PreviewImage.Dispose();
        }


    }
}
