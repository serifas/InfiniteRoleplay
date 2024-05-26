using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Utility;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Networking;
using Dalamud.Interface.Internal;
using OtterGui;
using System.Linq;

namespace InfiniteRoleplay.Windows
{
    public enum TabValue
    {
        Bio = 1,
        Hooks = 2,
        Story = 3,
        OOC = 4,
        Gallery = 5,
    }
    //changed
    public class ProfileWindow : Window, IDisposable
    {
        public static bool firstChapterLoad = true;
        public static string loading;
        public static float percentage = 0f;
        private Plugin plugin;
        private DalamudPluginInterface pg;
        private FileDialogManager _fileDialogManager;
        public Configuration configuration;
        public static int imageIndex = 0;
        public static IDalamudTextureWrap pictureTab;
        public static string[] HookNames = new string[31];
        public static string[] HookContents = new string[31];
        public static string[] ChapterContents = new string[31];
        public static string[] ChapterNames = new string[31];
        public static string[] imageURLs = new string[31];
        public static bool[] NSFW = new bool[31];
        public static bool[] TRIGGER = new bool[31];
        public static bool[] ImageExists = new bool[31];
        public static bool[] viewChapter = new bool[31];
        public static bool[] hookExists = new bool[31];
        public static bool[] storyChapterExists = new bool[31];
        public static SortedList<TabValue, bool> TabOpen = new SortedList<TabValue, bool>();
        public static bool editAvatar, addProfile, editProfile, Reorder, addGalleryImageGUI, alignmentHidden, personalityHidden, loadPreview = false;
        public static string oocInfo, storyTitle = string.Empty;
        public static bool ExistingProfile, ExistingStory, ExistingOOC, ExistingHooks, ExistingGallery, ExistingBio, ReorderHooks, ReorderChapters, AddHooks, AddStoryChapter;
        public static int chapterCount, currentAlignment, currentPersonality_1, currentPersonality_2, currentPersonality_3, hookCount = 0;
        public static byte[] avatarBytes;
        public static float _modVersionWidth, loaderInd;
        public static IDalamudTextureWrap avatarHolder, currentAvatarImg;
        public static List<IDalamudTextureWrap> galleryThumbsList = new List<IDalamudTextureWrap>();
        public static List<IDalamudTextureWrap> galleryImagesList = new List<IDalamudTextureWrap>();
        public static IDalamudTextureWrap[] galleryImages, galleryThumbs;
        public static string[] bioFieldsArr = new string[7];
        private IDalamudTextureWrap persistAvatarHolder;
        public static bool drawChapter;
        public static int storyChapterCount = -1;
        public static int currentChapter;
        public bool RedrawChapters { get; private set; }

        public ProfileWindow(Plugin plugin) : base(
       "PROFILE", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {

                MinimumSize = new Vector2(600, 400),
                MaximumSize = new Vector2(750, 950)
            };

            this.plugin = plugin;
            pg = plugin.PluginInterface;
            this.configuration = plugin.Configuration;
            this._fileDialogManager = new FileDialogManager();
            avatarHolder = Constants.UICommonImage(plugin.PluginInterface, Constants.CommonImageTypes.avatarHolder);
            pictureTab = Constants.UICommonImage(plugin.PluginInterface, Constants.CommonImageTypes.blankPictureTab);
            this.persistAvatarHolder = avatarHolder;
            for (int bf = 0; bf < bioFieldsArr.Length; bf++)
            {
                bioFieldsArr[bf] = string.Empty;
            }
            foreach (TabValue tab in Enum.GetValues(typeof(TabValue)))
            {
                TabOpen.Add(tab, false);
            }
            for (int i = 0; i < 31; i++)
            {
                ChapterNames[i] = string.Empty;
                ChapterContents[i] = string.Empty;
                HookNames[i] = string.Empty;
                HookContents[i] = string.Empty;
                hookExists[i] = false;
                NSFW[i] = false;
                TRIGGER[i] = false;
                storyChapterExists[i] = false;
                viewChapter[i] = false;
                ImageExists[i] = false;
                galleryImagesList.Add(pictureTab);
                galleryThumbsList.Add(pictureTab);
                imageURLs[i] = string.Empty;
            }
            galleryImages = galleryImagesList.ToArray();
            galleryThumbs = galleryThumbsList.ToArray();
            for (int b = 0; b < bioFieldsArr.Length; b++)
            {
                bioFieldsArr[b] = string.Empty;
            }
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {
                avatarBytes = File.ReadAllBytes(Path.Combine(path, "UI/common/profiles/avatar_holder.png"));
            }
        }

        public static bool AllLoaded()
        {
            if (DataReceiver.StoryLoadStatus != -1 &&
                   DataReceiver.HooksLoadStatus != -1 &&
                   DataReceiver.BioLoadStatus != -1 &&
                   DataReceiver.GalleryLoadStatus != -1)
            {
                return true;
            }
            return false;
        }
        public override void Draw()
        {
            PlayerCharacter player = plugin.ClientState.LocalPlayer;
            if (player != null)
            {
                if (AllLoaded() == true)
                {
                    _fileDialogManager.Draw();

                    if (ExistingProfile == true)
                    {
                        if (ImGui.Button("Edit Profile", new Vector2(100, 20))) { editProfile = true; }
                    }
                    if (ExistingProfile == false)
                    {
                        if (ImGui.Button("Add Profile", new Vector2(100, 20))) { addProfile = true; DataSender.CreateProfile(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString()); }
                    }


                    if (editProfile == true)
                    {
                        addProfile = false;
                        ImGui.Spacing();
                        if (ImGui.Button("Edit Bio", new Vector2(100, 20))) { ClearUI(); TabOpen[TabValue.Bio] = true; }
                        ImGui.SameLine();
                        if (ImGui.Button("Edit Hooks", new Vector2(100, 20))) { ClearUI(); TabOpen[TabValue.Hooks] = true; }
                        ImGui.SameLine();
                        if (ImGui.Button("Edit Story", new Vector2(100, 20))) { ClearUI(); TabOpen[TabValue.Story] = true; }
                        ImGui.SameLine();
                        if (ImGui.Button("Edit OOC Info", new Vector2(100, 20))) { ClearUI(); TabOpen[TabValue.OOC] = true; }
                        ImGui.SameLine();
                        if (ImGui.Button("Edit Gallery", new Vector2(100, 20))) { ClearUI(); TabOpen[TabValue.Gallery] = true; }

                    }
                    if (ImGui.BeginChild("PROFILE"))
                    {
                        #region BIO
                        if (TabOpen[TabValue.Bio])
                        {

                            ImGui.Image(currentAvatarImg.ImGuiHandle, new Vector2(100, 100));

                            if (ImGui.Button("Edit Avatar"))
                            {
                                editAvatar = true;
                            }
                            ImGui.Spacing();
                            for (int i = 0; i < Constants.BioFieldVals.Length; i++)
                            {
                                var BioField = Constants.BioFieldVals[i];
                                if (BioField.Item4 == Constants.InputTypes.single)
                                {
                                    ImGui.Text(BioField.Item1);
                                    if (BioField.Item1 != "AT FIRST GLANCE:")
                                    {
                                        ImGui.SameLine();
                                    }
                                    ImGui.InputTextWithHint(BioField.Item2, BioField.Item3, ref bioFieldsArr[i], 100);
                                }
                                else
                                {
                                    ImGui.Text(BioField.Item1);
                                    ImGui.InputTextMultiline(BioField.Item2, ref bioFieldsArr[i], 3100, new Vector2(500, 150));
                                }
                            }
                            ImGui.Spacing();
                            ImGui.Spacing();
                            ImGui.TextColored(new Vector4(1, 1, 1, 1), "ALIGNMENT:");
                            ImGui.SameLine();
                            ImGui.Checkbox("Hidden", ref alignmentHidden);
                            if (alignmentHidden == true)
                            {
                                currentAlignment = 9;
                            }
                            else
                            {
                                AddAlignmentSelection();
                            }

                            ImGui.Spacing();

                            ImGui.TextColored(new Vector4(1, 1, 1, 1), "PERSONALITY TRAITS:");
                            ImGui.SameLine();
                            ImGui.Checkbox("Hidden", ref personalityHidden);
                            if (personalityHidden == true)
                            {
                                currentPersonality_1 = 26;
                                currentPersonality_2 = 26;
                                currentPersonality_3 = 26;
                            }
                            else
                            {
                                AddPersonalitySelection_1();
                                AddPersonalitySelection_2();
                                AddPersonalitySelection_3();
                            }
                            if (ImGui.Button("Save Bio"))
                            {
                                DataSender.SubmitProfileBio(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                                        avatarBytes,
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.name].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.race].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.gender].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.age].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.height].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.weight].Replace("'", "''"),
                                                        bioFieldsArr[(int)Constants.BioFieldTypes.afg].Replace("'", "''"),
                                                        currentAlignment, currentPersonality_1, currentPersonality_2, currentPersonality_3);

                            }
                        }
                        #endregion
                        #region HOOKS
                        if (TabOpen[TabValue.Hooks])
                        {
                            if (ImGui.Button("Add Hook"))
                            {
                                if (hookCount < 30)
                                {
                                    hookCount++;
                                }
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Submit Hooks"))
                            {
                                List<Tuple<int, string, string>> hooks = new List<Tuple<int, string, string>>();
                                for (int i = 0; i < hookCount; i++)
                                {
                                    Tuple<int, string, string> hook = Tuple.Create(i, HookNames[i], HookContents[i]);
                                    hooks.Add(hook);
                                }
                                DataSender.SendHooks(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), hooks);

                            }
                            ImGui.NewLine();
                            AddHooks = true;
                            hookExists[hookCount] = true;
                        }
                        #endregion
                        #region STORY
                        if (TabOpen[TabValue.Story])
                        {
                            ImGui.Text("Story Title");
                            ImGui.SameLine();
                            ImGui.InputText("##storyTitle", ref storyTitle, 35);

                            ImGui.Text("Chapter");
                            ImGui.SameLine();
                            AddChapterSelection();
                            ImGui.SameLine();
                            if (ImGui.Button("Add Chapter"))
                            {
                                CreateChapter();
                            }
                            using (OtterGui.Raii.ImRaii.Disabled(!storyChapterExists.Any(x => x)))
                            {
                                if (ImGui.Button("Submit Story"))
                                {
                                    List<Tuple<string, string>> storyChapters = new List<Tuple<string, string>>();
                                    for (int i = 0; i < storyChapterCount + 1; i++)
                                    {

                                        string chapterName = ChapterNames[i].ToString();
                                        string chapterContent = ChapterContents[i].ToString();
                                        Tuple<string, string> chapter = Tuple.Create(chapterName, chapterContent);
                                        storyChapters.Add(chapter);
                                    }
                                    DataSender.SendStory(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), storyTitle, storyChapters);
                                }
                            }
                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                            {
                                ImGui.SetTooltip("Add a chapter to submit your story");
                            }
                            ImGui.NewLine();
                        }
                        #endregion
                        #region GALLERY

                        if (TabOpen[TabValue.Gallery])
                        {
                            if (ImGui.Button("Add Image"))
                            {
                                if (imageIndex < 28)
                                {
                                    imageIndex++;
                                }
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Submit Gallery"))
                            {
                                for (int i = 0; i < imageIndex; i++)
                                {
                                    DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                                      NSFW[i], TRIGGER[i], imageURLs[i], i);

                                }

                            }
                            ImGui.NewLine();
                            addGalleryImageGUI = true;
                            ImageExists[imageIndex] = true;
                        }
                        #endregion
                        #region OOC

                        if (TabOpen[TabValue.OOC])
                        {
                            ImGui.InputTextMultiline("##OOC", ref oocInfo, 50000, new Vector2(500, 600));
                            if (ImGui.Button("Submit OOC"))
                            {
                                DataSender.SendOOCInfo(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), oocInfo);
                            }
                        }
                        #endregion
                        if (loadPreview == true)
                        {
                            plugin.OpenImagePreview();
                            loadPreview = false;
                        }
                        if (addGalleryImageGUI == true)
                        {
                            AddImageToGallery(plugin, imageIndex);
                        }
                        if (AddHooks == true)
                        {
                            DrawHooksUI(plugin, hookCount);
                        }
                        if (editAvatar == true)
                        {
                            editAvatar = false;
                            EditImage(true, 0);
                        }
                        if (drawChapter == true)
                        {
                            ImGui.NewLine();
                            DrawChapter(currentChapter, plugin);
                        }
                        if (Reorder == true)
                        {
                            Reorder = false;
                            bool nextExists = ImageExists[NextAvailableImageIndex() + 1];
                            int firstOpen = NextAvailableImageIndex();
                            ImageExists[firstOpen] = true;
                            if (nextExists)
                            {
                                for (int i = firstOpen; i < imageIndex; i++)
                                {
                                    galleryImages[i] = galleryImages[i + 1];
                                    galleryThumbs[i] = galleryThumbs[i + 1];
                                    imageURLs[i] = imageURLs[i + 1];

                                }
                            }

                            imageIndex--;
                            galleryImages[imageIndex] = pictureTab;
                            galleryThumbs[imageIndex] = pictureTab;
                            ImageExists[imageIndex] = false;

                        }
                        if (ReorderHooks == true)
                        {
                            ReorderHooks = false;
                            bool nextHookExists = hookExists[NextAvailableHookIndex() + 1];
                            int firstHookOpen = NextAvailableHookIndex();
                            hookExists[firstHookOpen] = true;
                            if (nextHookExists)
                            {
                                for (int i = firstHookOpen; i < hookCount; i++)
                                {
                                    HookNames[i] = HookNames[i + 1];
                                    HookContents[i] = HookContents[i + 1];

                                }
                            }

                            hookCount--;
                            HookNames[hookCount] = string.Empty;
                            HookContents[hookCount] = string.Empty;
                            hookExists[hookCount] = false;

                        }
                        if (ReorderChapters == true)
                        {
                            ReorderChapters = false;
                            bool nextChapterExists = storyChapterExists[NextAvailableChapterIndex() + 1];
                            int firstChapterOpen = NextAvailableChapterIndex();
                            storyChapterExists[firstChapterOpen] = true;
                            if (nextChapterExists)
                            {
                                for (int i = firstChapterOpen; i < storyChapterCount; i++)
                                {
                                    ChapterNames[i] = ChapterNames[i + 1];
                                    ChapterContents[i] = ChapterContents[i + 1];
                                    DrawChapter(i, plugin);
                                }
                            }


                        }



                    }
                }
                else
                {
                    Misc.StartLoader(loaderInd, percentage, loading);
                }
            }
        }
        public void CreateChapter()
        {
            if (storyChapterCount < 30)
            {
                storyChapterCount++;
                storyChapterExists[storyChapterCount] = true;
                ChapterNames[storyChapterCount] = "New Chapter";
                currentChapter = storyChapterCount;
                viewChapter[storyChapterCount] = true;
            }

        }
        public void RemoveChapter(int index)
        {
            storyChapterCount--;
            storyChapterExists[index] = false;
            ChapterNames[index] = string.Empty;
            ChapterContents[index] = string.Empty;
            if (storyChapterExists[index - 1] == true)
            {
                currentChapter = index - 1;
                viewChapter[index - 1] = true;
            }
            ReorderChapters = true;

        }
        public void ClearChaptersInView()
        {
            for (int i = 0; i < viewChapter.Length; i++)
            {
                viewChapter[i] = false;
            }
        }
        public void DrawChapter(int i, Plugin plugin)
        {

            if (TabOpen[TabValue.Story] == true && i >= 0)
            {

                if (storyChapterExists[i] == true && viewChapter[i] == true)
                {
                    if (ImGui.BeginChild("##Chapter" + i, new Vector2(550, 250)))
                    {
                        ImGui.InputTextWithHint("##ChapterName" + i, "Chapter Name", ref ChapterNames[i], 300);
                        ImGui.InputTextMultiline("##ChapterContent" + i, ref ChapterContents[i], 5000, new Vector2(500, 200));
                        try
                        {

                            if (ImGui.BeginChild("##ChapterControls" + i))
                            {
                                using (OtterGui.Raii.ImRaii.Disabled(!InfiniteRoleplay.Plugin.CtrlPressed()))
                                {
                                    if (ImGui.Button("Remove##" + "chapter" + i))
                                    {
                                        RemoveChapter(i);

                                    }

                                }
                                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                                {
                                    ImGui.SetTooltip("Ctrl Click to Enable");
                                }


                            }



                            ImGui.EndChild();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    ImGui.EndChild();


                }
            }
        }
        public void DrawHook(int i, Plugin plugin)
        {
            if (hookExists[i] == true)
            {
                if (ImGui.BeginChild("##Hook" + i, new Vector2(550, 250)))
                {
                    ImGui.InputTextWithHint("##HookName" + i, "Hook Name", ref HookNames[i], 300);
                    ImGui.InputTextMultiline("##HookContent" + i, ref HookContents[i], 5000, new Vector2(500, 200));

                    try
                    {

                        if (ImGui.BeginChild("##HookControls" + i))
                        {
                            using (OtterGui.Raii.ImRaii.Disabled(!InfiniteRoleplay.Plugin.CtrlPressed()))
                            {
                                if (ImGui.Button("Remove##" + "hook" + i))
                                {
                                    hookExists[i] = false;
                                    ReorderHooks = true;
                                }
                            }
                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                            {
                                ImGui.SetTooltip("Ctrl Click to Enable");
                            }
                        }
                        ImGui.EndChild();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                ImGui.EndChild();
            }
        }

        public void AddImageToGallery(Plugin plugin, int imageIndex)
        {
            if (TabOpen[TabValue.Gallery])
            {
                if (ImGui.BeginTable("##GalleryTable", 4))
                {
                    for (int i = 0; i < imageIndex; i++)
                    {
                        if (i % 4 == 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            DrawGalleryImage(i);
                        }
                        else
                        {
                            ImGui.TableNextColumn();
                            DrawGalleryImage(i);
                        }
                    }
                    ImGui.EndTable();
                }
            }
        }
        public void DrawHooksUI(Plugin plugin, int hookCount)
        {
            if (TabOpen[TabValue.Hooks])
            {
                for (int i = 0; i < hookCount; i++)
                {
                    DrawHook(i, plugin);
                }
            }
        }


        public static int NextAvailableImageIndex()
        {
            bool load = true;
            int index = 0;
            for (int i = 0; i < ImageExists.Length; i++)
            {
                if (ImageExists[i] == false && load == true)
                {
                    load = false;
                    index = i;
                    return index;
                }
            }
            return index;
        }
        public static int NextAvailableChapterIndex()
        {
            bool load = true;
            int index = 0;
            for (int i = 0; i < storyChapterExists.Length; i++)
            {
                if (storyChapterExists[i] == false && load == true)
                {
                    load = false;
                    index = i;
                    return index;
                }
            }
            return index;
        }
        public int NextAvailableHookIndex()
        {
            bool load = true;
            int index = 0;
            for (int i = 0; i < hookExists.Length; i++)
            {
                if (hookExists[i] == false && load == true)
                {
                    load = false;
                    index = i;
                    return index;
                }
            }
            return index;
        }


        public void DrawGalleryImage(int i)
        {
            PlayerCharacter player = plugin.ClientState.LocalPlayer;
            if (ImageExists[i] == true)
            {

                if (ImGui.BeginChild("##GalleryImage" + i, new Vector2(150, 280)))
                {
                    ImGui.Text("Will this image be 18+ ?");
                    if (ImGui.Checkbox("Yes 18+", ref NSFW[i]))
                    {
                        for (int g = 0; g < imageIndex; g++)
                        {
                            DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                              NSFW[g], TRIGGER[g], imageURLs[g], g);

                        }
                    }
                    ImGui.Text("Is this a possible trigger ?");
                    if (ImGui.Checkbox("Yes Triggering", ref TRIGGER[i]))
                    {
                        for (int g = 0; g < imageIndex; g++)
                        {
                            DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                              NSFW[g], TRIGGER[g], imageURLs[g], g);
                        }
                    }
                    ImGui.InputTextWithHint("##ImageURL" + i, "Image URL", ref imageURLs[i], 300);
                    try
                    {
                        ImGui.Image(galleryThumbs[i].ImGuiHandle, new Vector2(galleryThumbs[i].Width, galleryThumbs[i].Height));
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Click to enlarge"); }
                        if (ImGui.IsItemClicked())
                        {
                            ImagePreview.width = galleryImages[i].Width;
                            ImagePreview.height = galleryImages[i].Height;
                            ImagePreview.PreviewImage = galleryImages[i];
                            loadPreview = true;

                        }




                        if (ImGui.BeginChild("##GalleryImageControls" + i))
                        {
                            using (OtterGui.Raii.ImRaii.Disabled(!InfiniteRoleplay.Plugin.CtrlPressed()))
                            {
                                if (ImGui.Button("Remove##" + "gallery_remove" + i))
                                {
                                    ImageExists[i] = false;
                                    Reorder = true;
                                    DataSender.RemoveGalleryImage(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), i, imageIndex);
                                }
                            }
                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                            {
                                ImGui.SetTooltip("Ctrl Click to Enable");
                            }

                        }
                        ImGui.EndChild();
                    }
                    catch (Exception ex)
                    {
                    }
                }


                ImGui.EndChild();

            }






        }
        public async void ResetGallery()
        {
            try
            {
                for (int g = 0; g < galleryImages.Length; g++)
                {
                    imageIndex = 0;
                    Reorder = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    ImageExists[i] = false;
                }
                for (int i = 0; i < galleryImages.Length; i++)
                {
                    galleryImages[i] = pictureTab;
                    galleryThumbs[i] = pictureTab;
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not reset gallery:: Results may be incorrect.", LogLevels.LogWarning);
            }
        }
        public async void Reset()
        {
            ResetBio();
            ResetGallery();
            ResetHooks();
            ResetStory();
        }
        public void ResetBio()
        {
            currentAvatarImg = this.persistAvatarHolder;
        }
        public void ResetHooks()
        {
            for (int h = 0; h < hookCount; h++)
            {
                HookNames[h] = string.Empty;
                HookContents[h] = string.Empty;
                hookExists[h] = false;
            }
            hookCount = 0;
        }
        public static void ResetStory()
        {
            for (int s = 0; s < storyChapterCount; s++)
            {
                ChapterNames[s] = string.Empty;
                ChapterContents[s] = string.Empty;
                chapterCount = 0;
                storyChapterExists[s] = false;
            }


            currentChapter = 0;
            chapterCount = 0;
            storyChapterCount = -1;
            storyTitle = string.Empty;
        }

        public static void ClearUI()
        {
            TabOpen[TabValue.Bio] = false;
            TabOpen[TabValue.Hooks] = false;
            TabOpen[TabValue.Story] = false;
            TabOpen[TabValue.OOC] = false;
            TabOpen[TabValue.Gallery] = false;
        }

        public void Dispose()
        {
            avatarHolder?.Dispose();
            avatarHolder = null;
            pictureTab?.Dispose();
            pictureTab = null;
            currentAvatarImg?.Dispose();
            currentAvatarImg = null;
            persistAvatarHolder?.Dispose();
            persistAvatarHolder = null;

            for (int i = 0; i < galleryImages.Length; i++)
            {
                galleryImages[i]?.Dispose();
                galleryImages[i] = null;
            }
            for (int i = 0; i < galleryThumbs.Length; i++)
            {
                galleryThumbs[i]?.Dispose();
                galleryThumbs[i] = null;
            }
            for (int i = 0; i < galleryImagesList.Count; i++)
            {
                galleryImagesList[i]?.Dispose();
                galleryImagesList[i] = null;
            }
            for (int i = 0; i < galleryThumbsList.Count; i++)
            {
                galleryThumbsList[i]?.Dispose();
                galleryThumbsList[i] = null;
            }
        }
        public void AddChapterSelection()
        {
            string chapterName = ChapterNames[currentChapter];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Chapter", chapterName);
            if (!combo)
                return;
            foreach (var (newText, idx) in ChapterNames.WithIndex())
            {
                string label = newText;
                if (label == string.Empty)
                {
                    label = "New Chapter";
                }
                if (newText != string.Empty)
                {
                    if (ImGui.Selectable(label + "##" + idx, idx == currentChapter))
                    {
                        currentChapter = idx;
                        storyChapterExists[currentChapter] = true;
                        viewChapter[currentChapter] = true;
                        drawChapter = true;
                    }
                    ImGuiUtil.SelectableHelpMarker("Select to edit chapter");
                }
            }
        }
        public void AddAlignmentSelection()
        {
            var (text, desc) = Constants.AlignmentVals[currentAlignment];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Alignment", text);
            ImGuiUtil.HoverTooltip(desc);
            if (!combo)
                return;

            foreach (var ((newText, newDesc), idx) in Constants.AlignmentVals.WithIndex())
            {
                if (ImGui.Selectable(newText, idx == currentAlignment))
                    currentAlignment = idx;

                ImGuiUtil.SelectableHelpMarker(newDesc);
            }
        }
        public void AddPersonalitySelection_1()
        {
            var (text, desc) = Constants.PersonalityValues[currentPersonality_1];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Personality Feature #1", text);
            ImGuiUtil.HoverTooltip(desc);
            if (!combo)
                return;

            foreach (var ((newText, newDesc), idx) in Constants.PersonalityValues.WithIndex())
            {
                if (ImGui.Selectable(newText, idx == currentPersonality_1))
                    currentPersonality_1 = idx;

                ImGuiUtil.SelectableHelpMarker(newDesc);
            }
        }
        public void AddPersonalitySelection_2()
        {
            var (text, desc) = Constants.PersonalityValues[currentPersonality_2];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Personality Feature #2", text);
            ImGuiUtil.HoverTooltip(desc);
            if (!combo)
                return;

            foreach (var ((newText, newDesc), idx) in Constants.PersonalityValues.WithIndex())
            {
                if (ImGui.Selectable(newText, idx == currentPersonality_2))
                    currentPersonality_2 = idx;

                ImGuiUtil.SelectableHelpMarker(newDesc);
            }
        }
        public void AddPersonalitySelection_3()
        {
            var (text, desc) = Constants.PersonalityValues[currentPersonality_3];
            using var combo = OtterGui.Raii.ImRaii.Combo("##Personality Feature #3", text);
            ImGuiUtil.HoverTooltip(desc);
            if (!combo)
                return;

            foreach (var ((newText, newDesc), idx) in Constants.PersonalityValues.WithIndex())
            {
                if (ImGui.Selectable(newText, idx == currentPersonality_3))
                    currentPersonality_3 = idx;

                ImGuiUtil.SelectableHelpMarker(newDesc);
            }
        }

        public void EditImage(bool avatar, int imageIndex)
        {
            _fileDialogManager.OpenFileDialog("Select Image", "Image{.png,.jpg}", (s, f) =>
            {
                if (!s)
                    return;
                string imagePath = f[0].ToString();
                var image = Path.GetFullPath(imagePath);
                byte[] imageBytes = File.ReadAllBytes(image);
                if (avatar == true)
                {
                    avatarBytes = File.ReadAllBytes(imagePath);
                    currentAvatarImg = pg.UiBuilder.LoadImage(avatarBytes);
                }
            }, 0, null, this.configuration.AlwaysOpenDefaultImport);

        }
        public static void ReloadProfile()
        {
            DataReceiver.BioLoadStatus = -1;
            DataReceiver.GalleryLoadStatus = -1;
            DataReceiver.HooksLoadStatus = -1;
            DataReceiver.StoryLoadStatus = -1;
        }

    }
}


