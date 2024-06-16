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
using Microsoft.VisualBasic;
using InfiniteRoleplay;

namespace InfiniteRoleplay.Windows
{
    public class TargetWindow : Window, IDisposable
    {
        private Plugin plugin;
        public static string loading;
        private DalamudPluginInterface pg;
        public GameFontHandle nameFont;
        public static float currentInd, max;
        public static string characterNameVal, characterWorldVal;
        public static string[] ChapterContent = new string[30];
        public static string[] ChapterTitle = new string[30];
        public static string[] HookNames = new string[30];
        public static string[] HookContents = new string[30];
        public static string[] HookEditContent = new string[30];
        public static int chapterCount;
        public static bool viewBio, viewHooks, viewStory, viewOOC, viewGallery, addNotes, loadPreview = false; //used to specify what view to show
        public static bool ExistingBio;
        public static bool ExistingHooks;
        public static int hookEditCount, existingGalleryImageCount;
        public static bool showAlignment, showPersonality;
        public static bool ExistingStory;
        public static bool ExistingOOC;
        public static bool ExistingGallery;
        public static bool ExistingProfile;
        public static string storyTitle = "";
        public static byte[] existingAvatarBytes;
        //BIO VARS
        public static IDalamudTextureWrap alignmentImg, personalityImg1, personalityImg2, personalityImg3;
        public static IDalamudTextureWrap[] galleryImages, galleryThumbs = new IDalamudTextureWrap[30];
        public static List<IDalamudTextureWrap> galleryThumbsList = new List<IDalamudTextureWrap>();
        public static List<IDalamudTextureWrap> galleryImagesList = new List<IDalamudTextureWrap>();

        public static IDalamudTextureWrap currentAvatarImg, pictureTab;
        //profile vars
        public static string characterEditName,
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
        public static bool[] ChapterExists = new bool[30];
        internal static string characterName;
        internal static string characterWorld;

        public TargetWindow(Plugin plugin) : base(
       "TARGET", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(600, 400),
                MaximumSize = new Vector2(750, 950)
            };
            this.plugin = plugin;
            this.pg = plugin.PluginInterface;
        }
        public override void OnOpen()
        {
            this.nameFont = pg.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Jupiter23));
            var blankPictureTab = Constants.UICommonImage(plugin, Constants.CommonImageTypes.blankPictureTab);
            if (blankPictureTab != null)
            {
                pictureTab = blankPictureTab;
            }

            //alignment icons
            for (int i = 0; i < 30; i++)
            {
                ChapterContent[i] = string.Empty;
                ChapterTitle[i] = string.Empty;
                ChapterExists[i] = false;
                HookContents[i] = string.Empty;
                HookNames[i] = string.Empty;
                galleryImagesList.Add(pictureTab);
                galleryThumbsList.Add(pictureTab);

            }
            galleryImages = galleryImagesList.ToArray();
            galleryThumbs = galleryThumbsList.ToArray();
        }

        public override void Draw()
        {
            if (plugin.IsOnline())
            {

                if (AllLoaded() == true)
                {
                    //if we receive that there is an existing profile that we can view show the available view buttons
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
                        if (ExistingOOC == true)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("OOC Info", new Vector2(100, 20))) { ClearUI(); viewOOC = true; }
                            if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View OOC section of this profile."); }
                        }
                        if (ExistingGallery == true)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("Gallery", new Vector2(100, 20))) { ClearUI(); viewGallery = true; }
                            if (ImGui.IsItemHovered()) { ImGui.SetTooltip("View gallery section of this profile."); }
                        }


                        //personal controls for viewing user
                        ImGui.Text("Controls");
                        if (ImGui.Button("Notes", new Vector2(100, 20))) { ClearUI(); addNotes = true; }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Add personal notes about this profile or the user."); }

                        ImGui.SameLine();

                        if (ImGui.Button("Report!", new Vector2(100, 20)))
                        {
                            ReportWindow.reportCharacterName = characterNameVal;
                            ReportWindow.reportCharacterWorld = characterWorldVal;
                            plugin.OpenReportWindow();
                        }
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Report this profile for inappropriate use.\n(Repeat false reports may result in your account being banned.)"); }

                    }

                    using var profileTable = ImRaii.Child("PROFILE");
                    if (profileTable)
                    {
                        //if there is absolutely no items to view
                        if (ExistingBio == false && ExistingHooks == false && ExistingStory == false && ExistingOOC == false && ExistingOOC == false && ExistingGallery == false)
                        {
                            //inform the viewer that there is no profile to view
                            ImGui.TextUnformatted("No Profile Data Available:\nIf this character has a profile, you can request to view it below.");

                            //but incase the profile is set to private, give the user a request button to ask for access
                            if (ImGui.Button("Request access"))
                            {
                                //send a new request to the server and then the profile owner if pressed
                                DataSender.SendProfileAccessUpdate(plugin.username, plugin.ClientState.LocalPlayer.Name.ToString(), plugin.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString(), characterName, characterWorld, (int)Constants.ConnectionStatus.pending);
                            }
                        }
                        else
                        {
                            if (viewBio == true)
                            {
                                //set bordered title at top of window and set fonts back to normal
                                Misc.SetTitle(plugin, true, characterEditName);
                                ImGui.Image(currentAvatarImg.ImGuiHandle, new Vector2(100, 100)); //display avatar image
                                ImGui.Spacing();
                                ImGui.TextUnformatted("NAME:   " + characterEditName); // display character name
                                ImGui.Spacing();
                                ImGui.TextUnformatted("RACE:   " + characterEditRace); // race
                                ImGui.Spacing();
                                ImGui.TextUnformatted("GENDER:   " + characterEditGender); //and so on
                                ImGui.Spacing();
                                ImGui.TextUnformatted("AGE:   " + characterEditAge);
                                ImGui.Spacing();
                                ImGui.TextUnformatted("HEIGHT:   " + characterEditHeight);
                                ImGui.Spacing();
                                ImGui.TextUnformatted("WEIGHT:   " + characterEditWeight);
                                ImGui.Spacing();
                                ImGui.TextUnformatted("AT FIRST GLANCE: \n" + characterEditAfg);
                                ImGui.Spacing();
                                if (showAlignment == true)
                                {
                                    ImGui.TextColored(new Vector4(1, 1, 1, 1), "ALIGNMENT:");

                                    ImGui.Image(alignmentImg.ImGuiHandle, new Vector2(32, 32));

                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip(alignmentTooltip);
                                    }
                                }
                                if (showPersonality == true)
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



                            if (viewHooks == true)
                            {
                                Misc.SetTitle(plugin, true, "Hooks"); //set title again
                                for (int h = 0; h < hookEditCount; h++)
                                {
                                    Misc.SetCenter(plugin, HookNames[h].ToString()); // set the position to the center of the window
                                    ImGui.TextUnformatted(HookNames[h].ToUpper()); //display the title in the center
                                    ImGui.TextUnformatted(HookContents[h]); //display the content
                                }

                            }

                            if (viewStory == true)
                            {
                                Misc.SetTitle(plugin, true, storyTitle);
                                string chapterMsg = "";


                                for (int h = 0; h < chapterCount; h++)
                                {
                                    Misc.SetCenter(plugin, ChapterTitle[h]);
                                    ImGui.TextUnformatted(ChapterTitle[h].ToUpper());
                                    ImGui.Spacing();
                                    using var defInfFontDen = ImRaii.DefaultFont();
                                    ImGui.TextUnformatted(ChapterContent[h]);
                                }


                            }
                            if (viewOOC == true)
                            {
                                Misc.SetTitle(plugin, true, "OOC Information");
                                ImGui.TextUnformatted(oocInfo);
                            }
                            if (viewGallery == true)
                            {
                                Misc.SetTitle(plugin, true, "Gallery");
                                using var table = ImRaii.Table("GalleryTargetTable", 4);
                                if (table)
                                {
                                    for (int i = 0; i < existingGalleryImageCount; i++)
                                    {
                                        ImGui.TableNextColumn();
                                        ImGui.Image(galleryThumbs[i].ImGuiHandle, new Vector2(galleryThumbs[i].Width, galleryThumbs[i].Height));
                                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Click to enlarge"); }
                                        if (ImGui.IsItemClicked())
                                        {
                                            ImagePreview.width = galleryImages[i].Width;
                                            ImagePreview.height = galleryImages[i].Height;
                                            ImagePreview.PreviewImage = galleryImages[i];
                                            loadPreview = true;
                                        }
                                    }
                                }

                            }

                            if (addNotes == true)
                            {

                                Misc.SetTitle(plugin, true, "Personal Notes");

                                ImGui.Text("Here you can add personal notes about this player or profile");
                                ImGui.InputTextMultiline("##info", ref profileNotes, 500, new Vector2(400, 100));
                                if (ImGui.Button("Add Notes"))
                                {
                                    if (plugin.IsOnline())
                                    {
                                        DataSender.AddProfileNotes(plugin.Configuration.username, characterNameVal, characterWorldVal, profileNotes);
                                    }

                                }

                            }
                            if (loadPreview == true)
                            {
                                plugin.OpenImagePreview();
                                loadPreview = false;
                            }
                        }

                    }
                }
                else
                {
                    Misc.StartLoader(currentInd, max, loading);
                }
            }
        }


        public static void ClearUI()
        {
            viewBio = false;
            viewHooks = false;
            viewStory = false;
            viewOOC = false;
            viewGallery = false;
            addNotes = false;
        }
        public static void ReloadTarget()
        {
            DataReceiver.TargetBioLoadStatus = -1;
            DataReceiver.TargetGalleryLoadStatus = -1;
            DataReceiver.TargetHooksLoadStatus = -1;
            DataReceiver.TargetStoryLoadStatus = -1;
            DataReceiver.TargetNotesLoadStatus = -1;
        }
        public void Dispose()
        {
            // Properly dispose of IDisposable resources
            currentAvatarImg?.Dispose();
            currentAvatarImg = null;
            nameFont?.Dispose();
            nameFont = null;
            pictureTab?.Dispose();
            pictureTab = null;
            alignmentImg?.Dispose();
            alignmentImg = null;
            personalityImg1?.Dispose();
            personalityImg1 = null;
            personalityImg2?.Dispose();
            personalityImg2 = null;
            personalityImg3?.Dispose();
            personalityImg3 = null;

            // Dispose gallery images and thumbs
            DisposeListResources(galleryImagesList);
            DisposeListResources(galleryThumbsList);
        }
        //method to check if all our data for the window is loaded
        public bool AllLoaded()
        {
            bool loaded = false;
            if (DataReceiver.TargetStoryLoadStatus != -1 &&
              DataReceiver.TargetHooksLoadStatus != -1 &&
              DataReceiver.TargetBioLoadStatus != -1 &&
              DataReceiver.TargetGalleryLoadStatus != -1 &&
              DataReceiver.TargetNotesLoadStatus != -1)
            {
                loaded = true;
            }
            return loaded;
        }

        // Helper method to dispose resources in a list
        private void DisposeListResources(List<IDalamudTextureWrap> resources)
        {
            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    if (resource != null)
                    {
                        resource?.Dispose();
                    }
                }
                resources.Clear();
            }
        }

    }
}
