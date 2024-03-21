using System;
using Dalamud.Interface;
using System.Security.Cryptography;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Interface.ImGuiFileDialog;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System.IO;
using ImGuiScene;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using System.Drawing;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using System.Xml.Linq;
using Networking;
using Dalamud.Interface.Internal;

namespace InfiniteRoleplay.Windows;

public class SystemsWindow : Window, IDisposable
{
    private bool system_created = false;
    private Configuration Configuration;
    private IDalamudTextureWrap systemImage;
    private Vector3 statColor1, statColor2, statColor3, statColor4, statColor5, statColor6, statColor7, statColor8, statColor9, statColor10, statColor11, statColor12;
    private IDalamudTextureWrap statImg1, statImg2, statImg3, statImg4, statImg5, statImg6, statImg7, statImg8, statImg9, statImg10, statImg11, statImg12;
    private IDalamudTextureWrap[] statImgs;
    public Dictionary<int, string> AccountSystems = new Dictionary<int, string>();
    public bool createSystem, statAllocationStatus, reductionStatus, selectSystem, reductionStatEffect;
    public string SystemName = string.Empty;
    public string SystemDescription = string.Empty;
    public string SystemImage = string.Empty;
    public string maxReductionPerStat = string.Empty;
    public string SystemImagePath = string.Empty;
    public string maxPointsPerStat = string.Empty;
    public string maxStats = string.Empty;
    public string maxReductionCount = string.Empty;
    public int statCount = 0;
    public Plugin plugin;
    public bool createSys;
    public byte[] SystemImageBytes;
    public List<byte[]> iconBytes;
    public int allocation_allowed, reduction_allowed = 0;
    public string[] statNames = new string[12];
    public string[] statDescriptions = new string[12];
    public string[] statIconPaths = new string[12];
    public string[] statIcons = new string[12];
    public Vector3[] statColors;

    public byte[][] statIconImages = new byte[][] { };
    public SystemsWindow(Plugin plugin) : base(
        "SYSTEMS",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(750, 950);
        this.SizeCondition = ImGuiCond.Always;
        this.plugin = plugin;
        this.Configuration = plugin.Configuration;
        for (int i = 0; i < 12; i++)
        {
            int statInd = i + 1;
            statNames[i] = "Stat " + statInd + "Name";
            statDescriptions[i] = "Stat " + statInd + "Description";
            statIconPaths[i] = Path.Combine(this.plugin.PluginInterfacePub.AssemblyLocation.Directory?.FullName!, "stats/statImgHolder.png");
            statIcons[i] = Path.Combine(this.plugin.PluginInterfacePub.AssemblyLocation.Directory?.FullName!, "stats/statImgHolder.png");
        }
        statImgs = new IDalamudTextureWrap[12] { statImg1, statImg2, statImg3, statImg4, statImg5, statImg6, statImg7, statImg8, statImg9, statImg10, statImg11, statImg12 };

        statColors = new Vector3[12] { statColor1, statColor2, statColor3, statColor4, statColor5, statColor6, statColor7, statColor8, statColor9, statColor10, statColor11, statColor12 };



        Vector3 StatBaseColor = new Vector3(255, 255, 255);
        for (int i = 0; i < 11; i++)
        {
            statColors[i] = StatBaseColor;
        }

        var statPlaceholderImg = Path.Combine(this.plugin.PluginInterfacePub.AssemblyLocation.Directory?.FullName!, "stats/statImgHolder.png");

    }

    public void Dispose() { }

    public override void Draw()
    {
        if (ImGui.BeginChild("ACCOUNT SYSTEMS LIST", new Vector2(200, 425)))
        {
            if (ImGui.Button("Add System", new Vector2(100, 20)))
            {
                createSystem = true;
            }

            if (ImGui.Selectable("System Name"))
            {
                selectSystem = true;
            }
        }
        ImGui.EndChild();

        ImGui.SameLine();
        if (ImGui.BeginChild("CREATE SYSTEM", new Vector2(550, 935)))
        {
            if (createSystem == true)
            {
                ImGui.InputTextWithHint("##name", $"Name", ref SystemName, 50);
                ImGui.InputTextWithHint("##description", $"A short but sweet description.", ref SystemDescription, 100);
                ImGui.InputTextWithHint("##systemImage", $"System Image Path (On your local machine, not a url).", ref SystemImage, 100);
                ImGui.SameLine();
                // add system image button
                if (ImGui.Button("Add System Image"))
                {
                    SystemImagePath = Path.GetFullPath(SystemImage);
                    systemImage = this.plugin.PluginInterfacePub.UiBuilder.LoadImage(SystemImagePath);
                    ImGui.SameLine();
                    SystemImageBytes = File.ReadAllBytes(SystemImagePath);
                }
                //if the system image path length is greater than 0 add the image
                if (SystemImagePath.Length > 0)
                {
                    ImGui.Image(systemImage.ImGuiHandle, new Vector2(50, 50));
                }
                ImGui.Separator();

                ImGui.Text("Stat Definitions");

                ImGui.Checkbox("Stat Allocation", ref statAllocationStatus);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Allow users to allocate their stats");
                }
                if (statAllocationStatus == true)
                {
                    allocation_allowed = 1;
                    ImGui.InputTextWithHint("##maxallocation", $"Max Stat Allocation (How many stat points are available)", ref maxStats, 4, ImGuiInputTextFlags.CharsHexadecimal);
                    ImGui.InputTextWithHint("##maxstatperstat", $"Max Stat Allocation Per Stat (Not including stat reduction effects)", ref maxPointsPerStat, 4, ImGuiInputTextFlags.CharsHexadecimal);
                    ImGui.Checkbox("Stat Reduction", ref reductionStatus);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Check if you want users to be able to reduce their stats below 0");
                    }
                }
                else
                {
                    allocation_allowed = 0;
                }
                if (reductionStatus == true)
                {
                    reduction_allowed = 1;
                    ImGui.InputTextWithHint("##maxreductionperstat", $"How far a can a user reduce a single stat below 0?", ref maxReductionPerStat, 4, ImGuiInputTextFlags.CharsHexadecimal);
                    ImGui.InputTextWithHint("##maxreduction", $"How far a can a user reduce their stats below 0?", ref maxReductionCount, 4, ImGuiInputTextFlags.CharsHexadecimal);

                    ImGui.Checkbox("Stat Reduction Effect", ref reductionStatEffect);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Enabling this feature will give extra stat allocation points to the user for every time they reduce a stat below 0");
                    }
                }
                else
                {
                    reduction_allowed = 0;
                }

                ImGui.Text("Stats: (12 max)");
                if (ImGui.Button("+", new Vector2(20, 20)))
                {
                    statCount++;
                    if (statCount > 12)
                    {
                        statCount = 12;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("-", new Vector2(20, 20)))
                {
                    statCount--;
                    if (statCount < 0)
                    {
                        statCount = 0;
                    }
                }
                for (int i = 0; i < statCount; i++)
                {
                    int labelNumber = i + 1;
                    ImGui.InputTextWithHint("##name" + i, $"Stat Name " + labelNumber, ref statNames[i], 50);

                    ImGui.InputTextMultiline("##description" + labelNumber, ref statDescriptions[i], 100, new Vector2(300, 50));


                    if (ImGui.BeginChild("COLORS" + i, new Vector2(300, 310)))
                    {
                        var picker = ImGui.ColorPicker3("Stat " + labelNumber + " Color", ref statColors[i], ImGuiColorEditFlags.Uint8);
                    }

                    ImGui.InputTextWithHint("##icon" + i, $"Stat " + labelNumber + " Icon path.", ref statIcons[i], 50);
                    if (ImGui.Button("Add Stat " + labelNumber + " Image"))
                    {

                        statIconPaths[i] = Path.GetFullPath(statIcons[i]);
                        statImgs[i] = this.plugin.PluginInterfacePub.UiBuilder.LoadImage(statIconPaths[i]);

                        statIconImages[i] = File.ReadAllBytes(statIconPaths[i]);
                    }
                    ImGui.SameLine();
                    if (statImgs[i] != null)
                    {
                        ImGui.SameLine();
                        ImGui.Image(statImgs[i].ImGuiHandle, new Vector2(25, 25));
                    }
                    ImGui.EndChild();





                }
                if (ImGui.Button("Create System"))
                {
                    byte[] bytes = new byte[12];
                    if (maxStats == string.Empty) { maxStats = "0"; }
                    if (maxPointsPerStat == string.Empty) { maxPointsPerStat = "0"; }
                    if (maxReductionCount == string.Empty) { maxReductionCount = "0"; }
                    if (maxReductionPerStat == string.Empty) { maxReductionPerStat = "0"; }

                    DataSender.CreateSystem(plugin.Configuration.username, SystemName, SystemDescription, SystemImageBytes, int.Parse(maxStats), int.Parse(maxPointsPerStat), int.Parse(maxReductionCount), int.Parse(maxReductionPerStat), allocation_allowed, reduction_allowed, statCount);
                    string statsMsg = "";
                    for (int i = 0; i < statCount; i++)
                    {
                        statsMsg += "<StatName>" + statNames[i] + "</StatName>" +
                                    "<StatDescription>" + statDescriptions[i] + "</StatDescription>" +
                                    "<StatColorR>" + statColors[i].X + "</StatColorR>" +
                                    "<StatColorG>" + statColors[i].Y + "</StatColorG>" +
                                    "<StatColorB>" + statColors[i].Z + "</StatColorB>|||";
                    }

                    DataSender.SendSystemStats(plugin.Configuration.username, SystemName, statsMsg);



                }
                ImGui.Separator();


            }
        }
        ImGui.EndChild();

    }
    public static FileDialogManager SetupFileManager()
    {
        var fileManager = new FileDialogManager
        {
            AddedWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking,
        };

        // Add Penumbra Root. This is not updated if the root changes right now.


        return fileManager;
    }

}
