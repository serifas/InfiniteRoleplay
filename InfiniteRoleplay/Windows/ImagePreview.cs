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
using InfiniteRoleplay;

namespace InfiniteRoleplay.Windows
{
    public class ImagePreview : Window, IDisposable
    {
        public static IDalamudTextureWrap PreviewImage;
        public static bool isAdmin;
        public Configuration configuration;
        public static bool WindowOpen;
        public string msg;
        public bool openedProfile = false;
        public static int width = 0, height = 0;
        public bool openedTargetProfile = false;
        public ImagePreview(Plugin plugin) : base(
       "PREVIEW", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.configuration = plugin.Configuration;
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(100, 100),
                MaximumSize = new Vector2(2000, 2000)
            };
        }

        public override void Draw()
        {
            (int scaledWidth, int scaledHeight) = CalculateScaledDimensions();

            // Here you would render the texture at the calculated width and height
            ImGui.Image(PreviewImage.ImGuiHandle, new Vector2(scaledWidth, scaledHeight));
        }
        private void GetImGuiWindowDimensions(out int width, out int height)
        {
            var windowSize = ImGui.GetWindowSize();
            width = (int)windowSize.X;
            height = (int)windowSize.Y;
        }

        private (int, int) CalculateScaledDimensions()
        {
            GetImGuiWindowDimensions(out int windowWidth, out int windowHeight);

            // Calculate the aspect ratios
            float windowAspect = (float)windowWidth / windowHeight;
            float textureAspect = (float)width / height;

            // Determine the scale factor
            int newWidth, newHeight;
            if (windowAspect > textureAspect)
            {
                // Window is wider relative to the texture's aspect ratio
                newHeight = windowHeight;
                newWidth = (int)(newHeight * textureAspect);
            }
            else
            {
                // Window is taller relative to the texture's aspect ratio
                newWidth = windowWidth;
                newHeight = (int)(newWidth / textureAspect);
            }

            return (newWidth, newHeight);
        }



        public void Dispose()
        {
            WindowOpen = false;
            PreviewImage?.Dispose();
        }


    }
}
