using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Interface.GameFonts;
using OtterGui.Raii;
using Networking;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Internal;
using InfiniteRoleplay.Helpers; 
using InfiniteRoleplay.Scripts.Misc;

namespace InfiniteRoleplay.Windows
{
    public class TargetWindow : Window, IDisposable
    {
        private Plugin plugin;
        public static string loading;
        private IChatGui chatGui;
        private DalamudPluginInterface pg;
        public GameFontHandle nameFont;
        public static float currentInd, max;
        public static string characterNameVal, characterWorldVal;
        public static string[] StoryContent = new string[30];
        public static string[] ChapterContent = new string[30];
        public static string[] ChapterTitle = new string[30];
        public static string[] HookContent = new string[30];
        public static string[] HookEditContent = new string[30];
        public static int chapterCount;
        public static bool viewBio, viewHooks, viewStory, viewOOC, viewGallery, addNotes = false;
        public static bool ExistingBio;
        public static bool ExistingHooks;
        public static int hookEditCount, existingGalleryImageCount;
        public static bool showAlignment, showPersonality;
        public static bool ExistingStory;
        public static bool ExistingOOC;
        public static bool ExistingGallery;
        public static bool ExistingProfile;
        public static string storyTitle = "";
        public byte[] existingAvatarBytes;
        //BIO VARS
        public static IDalamudTextureWrap alignmentImg, personalityImg1, personalityImg2, personalityImg3;
        public static IDalamudTextureWrap[] galleryImages, galleryThumbs = new IDalamudTextureWrap[30];
        public static List<IDalamudTextureWrap> galleryThumbsList = new List<IDalamudTextureWrap>();
        public static List<IDalamudTextureWrap> galleryImagesList = new List<IDalamudTextureWrap>();
       
        public static IDalamudTextureWrap currentAvatarImg, pictureTab;
        public static string    characterEditName,
                                characterEditRace,
                                characterEditGender,
                                characterEditAge,
                                characterEditAfg,
                                characterEditHeight,
                                characterEditWeight,
                                fileName, 
                                reportInfo, 
                                profileNotes,
                                alignmentTooltip, 
                                personality1Tooltip, 
                                personality2Tooltip, 
                                personality3Tooltip, 
                                oocInfo = string.Empty;
        private bool AllLoaded;

        public TargetWindow(Plugin plugin, DalamudPluginInterface Interface) : base(
       "TARGET", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(600, 400),
                MaximumSize = new Vector2(750, 950)
            };
            this.plugin = plugin;
            this.pg = plugin.PluginInterfacePub;

            this.nameFont = pg.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
            pictureTab = Constants.UICommonImage(Interface, Constants.CommonImageTypes.blankPictureTab);
            //alignment icons
             this.chatGui = chatGui;
            for (int i =0; i < 30; i++)
            {
                StoryContent[i] = string.Empty;
                ChapterContent[i] = string.Empty;
                ChapterTitle[i] = string.Empty;
                HookEditContent[i] = string.Empty;
                galleryImagesList.Add(pictureTab);
                galleryThumbsList.Add(pictureTab);
                
            }       
            galleryImages = galleryImagesList.ToArray();
            galleryThumbs = galleryThumbsList.ToArray();
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        
        public override void Draw()
        {
            if (AllLoaded == true)
            {
                if (ExistingProfile == true)
                {
                    if (ExistingBio == true)
                    {
                        if (ImGui.Button("Bio", new Vector2(100, 20))) { ClearUI(); viewBio = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View bio section of this profile."); }
                    }
                    if (ExistingHooks == true)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("Hooks", new Vector2(100, 20))) { ClearUI(); viewHooks = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View hooks section of this profile."); }
                    }
                    if (ExistingStory == true)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("Story", new Vector2(100, 20))) { ClearUI(); viewStory = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View story section to your profile."); }
                    }
                    if (ExistingGallery == true)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("Gallery", new Vector2(100, 20))) { ClearUI(); viewGallery = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View gallery section of this profile."); }
                    }
                    if (ExistingOOC == true)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("OOC Info", new Vector2(100, 20))) { ClearUI(); viewOOC = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View OOC section of this profile."); }
                    }

                    ImGui.Text("Controls");
                    if (ImGui.Button("Notes", new Vector2(100, 20))) { ClearUI(); addNotes = true; }
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Add personal notes about this profile or the user."); }
                    ImGui.SameLine();
                    if (ImGui.Button("Report!", new Vector2(100, 20))) {
                        ReportWindow.reportCharacterName = characterNameVal;
                        ReportWindow.reportCharacterWorld = characterWorldVal;
                        plugin.reportWindow.IsOpen = true;
                    }
                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Report this profile for inappropriate use.\n(Repeat false reports may result in your report ability being revoked.)"); }
                    
                }




                if (ImGui.BeginChild("PROFILE"))
                {


                    if (ExistingBio == false && ExistingHooks == false && ExistingStory == false && ExistingOOC == false && ExistingOOC == false && ExistingGallery == false)
                    {
                        ImGui.Text("No Profile Data Available");
                    }
                    else
                    {
                        if (viewBio == true)
                        {
                            Misc.SetTitle(plugin, true, characterEditName);
                            ImGui.Image(currentAvatarImg.ImGuiHandle, new Vector2(100, 100));


                            ImGui.Spacing();
                            ImGui.Text("NAME:   " + characterEditName);
                            ImGui.Spacing();
                            ImGui.Text("RACE:   " + characterEditRace);
                            ImGui.Spacing();
                            ImGui.Text("GENDER:   " + characterEditGender);
                            ImGui.Spacing();
                            ImGui.Text("AGE:   " + characterEditAge);
                            ImGui.Spacing();
                            ImGui.Text("HEIGHT:   " + characterEditHeight);
                            ImGui.Spacing();
                            ImGui.Text("WEIGHT:   " + characterEditWeight);
                            ImGui.Spacing();
                            ImGui.Text("AT FIRST GLANCE: \n" + characterEditAfg);
                            ImGui.Spacing();
                            if(showAlignment == true)
                            {

                                ImGui.TextColored(new Vector4(1, 1, 1, 1), "ALIGNMENT:");

                                ImGui.Image(alignmentImg.ImGuiHandle, new Vector2(32, 32));
                            
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.SetTooltip(alignmentTooltip);
                                }
                            }
                            if(showPersonality == true)
                            {
                                ImGui.Spacing();

                                ImGui.TextColored(new Vector4(1, 1, 1, 1), "PERSONALITY TRAITS:");
                            
                                ImGui.Image(personalityImg1.ImGuiHandle, new Vector2(32, 42));

                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.SetTooltip(personality1Tooltip);
                                }
                                ImGui.SameLine();
                                ImGui.Image(personalityImg2.ImGuiHandle, new Vector2(32, 42));

                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.SetTooltip(personality2Tooltip);
                                }
                                ImGui.SameLine();
                                ImGui.Image(personalityImg3.ImGuiHandle, new Vector2(32, 42));

                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.SetTooltip(personality3Tooltip);
                                }

                            }


                        }   


                    }

                    if (viewHooks == true)
                    {
                        Misc.SetTitle(plugin, true, "Hooks");
                        for (int h = 0; h < hookEditCount; h++)
                        {
                            ImGui.Text(HookEditContent[h].Replace("---===---", "\n").Replace("''", "'"));
                        }

                    }

                    if (viewStory == true)
                    {
                        Misc.SetTitle(plugin, true, storyTitle);
                        string chapterMsg = "";


                        for (int h = 0; h < chapterCount; h++)
                        {
                            string Chapter = ChapterContent[h].Replace("---===---", "\n").Replace("''", "'");

                            ImGui.Text(ChapterTitle[h]);
                            using var defInfFontDen = ImRaii.DefaultFont();
                            ImGui.Text(Chapter);
                        }


                    }
                    if(viewOOC == true)
                    {
                        Misc.SetTitle(plugin, true, "OOC Information");
                        ImGui.Text(oocInfo);
                    }
                    if (viewGallery == true)
                    {

                        Misc.SetTitle(plugin, true, "Gallery");
                        if (ImGui.BeginTable("##GalleryTargetTable", 4))
                        {
                            for (int i = 0; i < existingGalleryImageCount; i++)
                            {
                                if (i % 4 == 0)
                                {
                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();

                                    // you might normally want to embed resources and load them from the manifest stream
                                    //this.imageTextures.Add(goatImage);
                                   

                                    ImGui.Image(galleryThumbs[i].ImGuiHandle, new Vector2(galleryThumbs[i].Width, galleryThumbs[i].Height));
                                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Click to enlarge"); }
                                    if (ImGui.IsItemClicked())
                                    {
                                        ImagePreview.width = galleryImages[i].Width;
                                        ImagePreview.height = galleryImages[i].Height;
                                        ImagePreview.PreviewImage = galleryImages[i];
                                        plugin.loadPreview = true;
                                    }
                                }
                                else
                                {
                                    ImGui.TableNextColumn();
                                    // simulate some work

                                    // you might normally want to embed resources and load them from the manifest stream
                                    //this.imageTextures.Add(goatImage);
                                   

                                    ImGui.Image(galleryThumbs[i].ImGuiHandle, new Vector2(galleryThumbs[i].Width, galleryThumbs[i].Height));
                                    if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Click to enlarge"); }
                                    if (ImGui.IsItemClicked())
                                    {
                                        ImagePreview.width = galleryImages[i].Width;
                                        ImagePreview.height = galleryImages[i].Height;
                                        ImagePreview.PreviewImage = galleryImages[i];
                                        plugin.loadPreview = true;
                                    }
                                }

                            }
                            ImGui.EndTable();





                        }
                    }
                   
                    if(addNotes == true)
                    {

                        Misc.SetTitle(plugin, true, "Personal Notes");

                        ImGui.Text("Here you can add personal notes about this player or profile");
                        ImGui.InputTextMultiline("##info", ref profileNotes, 500, new Vector2(400, 100));
                        if(ImGui.Button("Add Notes"))
                        {
                            DataSender.AddProfileNotes(plugin.Configuration.username, characterNameVal, characterWorldVal, profileNotes);
                        }
                        
                    }
                }

                else
                {
                    ImGui.Text("No Profile Available");
                }





            }
            else
            {
                Misc.StartLoader(currentInd, max, loading);
            }

        }

       
        public static void ClearUI()
        {
            viewBio = false;
            viewHooks = false; 
            viewStory = false;
            viewOOC= false;
            viewGallery = false;
            addNotes = false;
        }
        public void Dispose()
        {
            currentAvatarImg.Dispose();
            pictureTab.Dispose();
            alignmentImg.Dispose();
            personalityImg1.Dispose();
            personalityImg2.Dispose();
            personalityImg3.Dispose();
            for (int gt = 0; gt < galleryThumbsList.Count; gt++)
            {
                galleryThumbsList[gt].Dispose();
            }
            for (int gi = 0; gi < galleryThumbsList.Count; gi++)
            {
                galleryImagesList[gi].Dispose();
            }
            for(int i = 0; i < galleryImages.Length; i++)
            {
                galleryImages[i].Dispose();
                Array.Clear(galleryImages);
            }
            for(int t = 0; t < galleryThumbs.Length; t++)
            {
                galleryThumbs[t].Dispose();
                Array.Clear(galleryThumbs);
            }
        }
        public override void Update()
        {         
            if (DataReceiver.TargetStoryLoadStatus != -1 &&
               DataReceiver.TargetHooksLoadStatus != -1 &&
               DataReceiver.TargetBioLoadStatus != -1 &&
               DataReceiver.TargetGalleryLoadStatus != -1 &&
               DataReceiver.TargetNotesLoadStatus != -1)
            {
                AllLoaded = true;
            }
            else
            {
                AllLoaded = false;
            }

        }
       
       
    }
}
