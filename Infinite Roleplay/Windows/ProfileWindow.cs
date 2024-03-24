using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Utility;
using OtterGui.Raii;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Networking;
using InfiniteRoleplay.Helpers;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using InfiniteRoleplay.Scripts.Misc;
using OtterGui;

namespace InfiniteRoleplay.Windows
{
    //changed
    public class ProfileWindow : Window, IDisposable
    {
        public static string loading;
        public static bool AllLoaded;
        public static float percentage = 0f;
        public static bool Reorder = false;
        private Plugin plugin;
        public static PlayerCharacter playerCharacter;
        private DalamudPluginInterface pg;
        public static bool addGalleryImageGUI = false;
        private FileDialogManager _fileDialogManager;
        public Configuration configuration;
        public static bool editBio = false;
        public static bool editHooks = false;
        public static bool alignmentHidden = false;
        public static bool personalityHidden = false;
        public static bool galleryTableAdded = false;
        public static bool resetHooks;
        public static int imageIndex = 0;
        public static bool resetStory;
        public static IDalamudTextureWrap pictureTab;
        public static string[] HookContent = new string[30];
        public static string[] HookEditContent = new string[30];
        public static string[] ChapterContent = new string[30];
        public static string[] ChapterEditContent = new string[30];
        public static string[] ChapterTitle = new string[30];
        public static string[] ChapterEditTitle = new string[30];
        public static string[] imageURLs = new string[30];
        public static bool[] NSFW = new bool[30];
        public static bool[] TRIGGER = new bool[30];
        public static bool[] ImageExists = new bool[30];
        public static bool editStory, addOOC, editOOC, addGallery, editGallery, addAvatar, editAvatar, addProfile, editProfile, LoadPreview = false;
        public static int hookCount = 0;
        public static int hookEditCount;
        public static int chapterCount = 0;
        public static int chapterEditCount;
        public static string oocInfo = string.Empty;
        public static int imageIndexVal = 0;
        public bool ExistingProfile;
        public bool ExistingStory;
        public bool ExistingOOC;
        public bool ExistingGallery;
        public bool ExistingBio;
        public bool ExistingHooks;
        public static string storyEditTitle = string.Empty;
        public static int currentAlignment, currentPersonality_1, currentPersonality_2, currentPersonality_3 = 0;
        public byte[] avatarBytes;
        public static float _modVersionWidth, loaderInd;
        public static IDalamudTextureWrap avatarHolder, currentAvatarImg;
        public static List<IDalamudTextureWrap> galleryThumbsList = new List<IDalamudTextureWrap>();
        public static List<IDalamudTextureWrap> galleryImagesList = new List<IDalamudTextureWrap>();
        public static IDalamudTextureWrap[] galleryImages, galleryThumbs;
        public static string[] bioFieldsArr = new string[7];

        public bool reduceChapters = false;
        public bool reduceHooks = false;
        public IDalamudTextureWrap blank;
        public static System.Drawing.Image bl;
        private IDalamudTextureWrap persistAvatarHolder;
        private IDalamudTextureWrap[] otherImages;

        public ProfileWindow(Plugin plugin,
                             DalamudPluginInterface Interface,
                             IChatGui chatGUI,
                             Configuration configuration) : base(
       "PROFILE", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {

                MinimumSize = new Vector2(600, 400),
                MaximumSize = new Vector2(750, 950)
            };

            this.plugin = plugin;
            pg = plugin.PluginInterfacePub;
            string path = pg.AssemblyLocation.Directory?.FullName!;
            this.configuration = plugin.Configuration;
            this._fileDialogManager = new FileDialogManager();
            avatarHolder = Constants.UICommonImage(Interface, Constants.CommonImageTypes.avatarHolder); 
            pictureTab = Constants.UICommonImage(Interface, Constants.CommonImageTypes.blankPictureTab);
            this.persistAvatarHolder = avatarHolder;
            this.configuration = configuration;
            for(int bf = 0; bf < bioFieldsArr.Length; bf++)
            {
                bioFieldsArr[bf] = string.Empty;
            }
            for(int i = 0; i < 30; i++)
            {
                ChapterTitle[i] = string.Empty;
                ChapterEditTitle[i] = string.Empty;
                ChapterContent[i] = string.Empty;
                ChapterEditContent[i] = string.Empty;
                HookContent[i] = string.Empty;
                HookEditContent[i] = string.Empty;
                NSFW[i] = false;
                TRIGGER[i] = false;
                galleryImagesList.Add(pictureTab);
                galleryThumbsList.Add(pictureTab);
                imageURLs[i] = string.Empty;
            }
            galleryImages = galleryImagesList.ToArray();
            galleryThumbs = galleryThumbsList.ToArray();
            for(int b = 0; b <bioFieldsArr.Length; b++)
            {
                bioFieldsArr[b] = string.Empty;
            }
            
            this.avatarBytes = File.ReadAllBytes(Path.Combine(path, "UI/common/profiles/avatar_holder.png"));
         }
      

        public override void Draw()
        {
            if (playerCharacter != null)
            {
                
                if (AllLoaded == true)
                {
                    _fileDialogManager.Draw();

               
                    if (this.ExistingProfile == true)
                    {
                        if (ImGui.Button("Edit Profile", new Vector2(100, 20))) { editProfile = true; }
                    }
                    if (this.ExistingProfile == false)
                    {
                        if (ImGui.Button("Add Profile", new Vector2(100, 20))) { addProfile = true; DataSender.CreateProfile(configuration.username, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString()); }
                    }

                
                    if (editProfile == true)
                    {
                        addProfile = false;
                        ImGui.Spacing();
                        if (this.ExistingBio == true) { if (ImGui.Button("Edit Bio", new Vector2(100, 20))) { ClearUI(); editBio = true; } if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Edit your bio."); } } else { if (ImGui.Button("Add Bio", new Vector2(100, 20))) { ClearUI(); editBio = true; } }
                        ImGui.SameLine();
                        if (this.ExistingHooks == true) { if (ImGui.Button("Edit Hooks", new Vector2(100, 20))) { ClearUI(); editHooks = true; } if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Edit your Hooks."); } } else { if (ImGui.Button("Add Hooks", new Vector2(100, 20))) { ClearUI(); editHooks = true; } }
                        ImGui.SameLine();
                        if (this.ExistingStory == true) { if (ImGui.Button("Edit Story", new Vector2(100, 20))) { ClearUI(); editStory = true; } if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Edit your Story."); } } else { if (ImGui.Button("Add Story", new Vector2(100, 20))) { ClearUI(); editStory = true; } }
                        ImGui.SameLine();
                        if (this.ExistingOOC == true) { if (ImGui.Button("Edit OOC Info", new Vector2(100, 20))) { ClearUI(); addOOC = true; } if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Edit your OOC Info."); } } else { if (ImGui.Button("Add OOC Info", new Vector2(100, 20))) { ClearUI(); addOOC = true; } }
                        ImGui.SameLine();
                        if (this.ExistingGallery == true) { if (ImGui.Button("Edit Gallery", new Vector2(100, 20))) { ClearUI(); addGallery = true; } if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Edit your Gallery."); } } else { if (ImGui.Button("Add Gallery", new Vector2(100, 20))) { ClearUI(); addGallery = true; } }

                    }
                    bool warning = false;
                    bool success = false;
                    if (ImGui.BeginChild("PROFILE"))
                    {
                        #region BIO
                        if (editBio == true)
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
                                    if(BioField.Item1 != "AT FIRST GLANCE:")
                                    {
                                        ImGui.SameLine();
                                    }                                
                                    ImGui.InputTextWithHint(BioField.Item2, BioField.Item3, ref bioFieldsArr[i], 100);
                                }
                                else
                                {
                                    ImGui.Text(BioField.Item1);
                                    ImGui.InputTextMultiline(BioField.Item2, ref bioFieldsArr[i], 3000, new Vector2(500, 150));
                                }
                            }
                            ImGui.Spacing();
                            ImGui.Spacing();
                            ImGui.TextColored(new Vector4(1, 1, 1, 1), "ALIGNMENT:");
                            ImGui.SameLine();
                            ImGui.Checkbox("Hidden", ref alignmentHidden);
                            if(alignmentHidden == true)
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
                            if(personalityHidden == true)
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
                                DataSender.SubmitProfileBio(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(),
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
                        if (editHooks == true)
                        {
                            if (resetHooks == true)
                            {
                                hookCount = 0;
                                resetHooks = false;
                            }
                            string hookMsg = "";
                            if (ImGui.Button("+##addhook", new Vector2(30, 30)))
                            {
                                hookCount++;
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("-##removehook", new Vector2(30, 30)))
                            {
                                reduceHooks = true;
                                if (hookCount > 0)
                                {
                                    hookCount--;
                                    reduceHooks = false;
                                }
                                if (hookCount == 0 && reduceHooks == true)
                                {
                                    if (hookEditCount > 0)
                                    {
                                        hookEditCount--;
                                    }
                                }
                            }
                            for (int i = 0; i < hookCount; i++)
                            {
                                int index = hookCount + hookEditCount;
                                hookMsg += "<hook>" + HookContent[i].Replace("\n", "---===---") + "</hook>|||";
                                ImGui.InputTextMultiline("##hook" + i, ref HookContent[i], 3000, new Vector2(450, 100));
                            }
                            for (int h = 0; h < hookEditCount; h++)
                            {
                                int index = hookCount + hookEditCount;
                                ImGui.InputTextMultiline("##hookedit" + h, ref HookEditContent[h], 3000, new Vector2(450, 100));
                                hookMsg += "<hook>" + HookEditContent[h].Replace("\n", "---===---") + "</hook>|||";
                            }
                            if (ImGui.Button("Submit Hooks"))
                            {
                                DataSender.SendHooks(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(), hookMsg);
                            }
                        }
                        #endregion
                        #region STORY
                        if (editStory == true)
                        {
                            if (resetStory == true)
                            {
                                chapterCount = 0;
                                resetStory = false;
                            }
                            ImGui.Text("Story Title:");
                            ImGui.SameLine();
                            ImGui.InputText("##story_edit_title", ref storyEditTitle, 100);
                            string chapterMsg = "";
                            string ChapterEdit = "";
                            if (ImGui.Button("Add Chapter"))
                            {
                                chapterCount++;
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Remove Chapter"))
                            {
                                reduceChapters = true;
                                if (chapterCount > 0)
                                {
                                    chapterCount--;
                                    reduceChapters = false;
                                }
                                if (chapterCount == 0 && reduceChapters == true)
                                {
                                    if (chapterEditCount > 0)
                                    {
                                        chapterEditCount--;
                                    }
                                }
                            }
                            for (int h = 0; h < chapterEditCount; h++)
                            {
                                ImGui.Text("Chapter Title:");
                                ImGui.SameLine();
                                ImGui.InputText("##editchaptertitle" + h, ref ChapterEditTitle[h], 100);

                                ImGui.Text("Chapter:");
                                ImGui.SameLine();
                                ImGui.InputTextMultiline("##chapteredit" + h, ref ChapterEditContent[h], 3000, new Vector2(450, 100));
                                ChapterEdit += "<chapter><chapterTitle>" + ChapterEditTitle[h] + "</chapterTitle><chapterContent>" + ChapterEditContent[h].Replace("\n", "---===---") + "</chapterContent></chapter>|||";
                            }
                            for (int i = 0; i < chapterCount; i++)
                            {
                                ImGui.Text("Chapter Title:");
                                ImGui.SameLine();
                                ImGui.InputText("##chaptertitle" + i, ref ChapterTitle[i], 100);
                                chapterMsg += "<chapter><chapterTitle>" + ChapterTitle[i] + "</chapterTitle><chapterContent>" + ChapterContent[i].Replace("\n", "---===---") + "</chapterContent></chapter>|||";
                                ImGui.Text("Chapter:");
                                ImGui.SameLine();
                                ImGui.InputTextMultiline("##chapter" + i, ref ChapterContent[i], 3000, new Vector2(450, 100));
                            }
                            string chapterMessage = ChapterEdit + chapterMsg;
                            if (ImGui.Button("Update Story"))
                            {
                                DataSender.SendStory(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(), storyEditTitle, chapterMessage);
                            }
                        }
                        #endregion
                        #region GALLERY

                        if (addGallery == true)
                        {
                            if (ImGui.Button("Add Image"))
                            {
                                if (imageIndex < 29)
                                {
                                    imageIndex++;
                                }
                            }
                            ImGui.SameLine();
                            if(ImGui.Button("Submit Gallery"))
                            {
                                for(int i = 0; i < imageIndex; i++)
                                {
                                    DataSender.SendGalleryImage(configuration.username, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(),
                                                      NSFW[i], TRIGGER[i], imageURLs[i], i);
                                }   
                            
                            }
                            ImGui.NewLine();
                            addGalleryImageGUI = true;
                            ImageExists[imageIndex] = true;
                        }
                        #endregion
                        #region OOC

                        if (addOOC)
                        {
                            ImGui.InputTextMultiline("##OOC", ref oocInfo, 50000, new Vector2(500, 600));  
                            if(ImGui.Button("Submit OOC"))
                            {
                                DataSender.SendOOCInfo(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(), oocInfo);
                            }
                        }
                        #endregion
                                     
                        if (addGalleryImageGUI == true)
                        {
                            AddImageToGallery(plugin, imageIndex);
                        }


                        if (editAvatar == true)
                        {
                            editAvatar = false;
                            EditImage(true, 0);
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
                   

                    }
                }
                else
                {
                    Misc.StartLoader(loaderInd, percentage, loading);
                }
            }
        }


        public void AddImageToGallery(Plugin plugin, int imageIndex)
        {
            if(addGallery == true)
            {          
                if (ImGui.BeginTable("##GalleryTable", 4))
                {                    
                    for (int i = 0; i < imageIndex; i++)
                    {                       
                        if (i % 4 == 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            DrawGalleryImage(i, plugin);
                        }
                        else
                        {
                            ImGui.TableNextColumn();
                            DrawGalleryImage(i, plugin);
                        }
                    }
                    ImGui.EndTable();
                }
            }

        }
        
        public static int NextAvailableImageIndex()
        {
            bool load = true;
            int index = 0;
            for(int i = 0; i < ImageExists.Length; i++)
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


        public void DrawGalleryImage(int i, Plugin plugin)
        {
            if (ImageExists[i] == true)
            {
               
                if (ImGui.BeginChild("##GalleryImage" + i, new Vector2(150, 280)))
                {
                    ImGui.Text("Will this image be 18+ ?");
                    if (ImGui.Checkbox("Yes 18+", ref NSFW[i]))
                    {
                        DataSender.SendGalleryImage(configuration.username, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(),
                                                    NSFW[i], TRIGGER[i], imageURLs[i], i);
                    }
                    ImGui.Text("Is this a possible trigger ?");
                    if (ImGui.Checkbox("Yes Triggering", ref TRIGGER[i]))
                    {
                        DataSender.SendGalleryImage(configuration.username, playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(),
                                                    NSFW[i],TRIGGER[i], imageURLs[i], i);
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
                            plugin.loadPreview = true;
                        }
                        if (ImGui.BeginChild("##GalleryImageControls" + i))
                        {
                            if (ImGui.Button("Remove##" + "gallery_remove" + i))
                            {
                                imageIndexVal = i;
                                ImageExists[i] = false;
                                Reorder = true;
                                DataSender.RemoveGalleryImage(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.GameData.Name.ToString(), i, imageIndex);
                            }
                        }
                        ImGui.EndChild();
                    }
                    catch(Exception ex)
                    {
                    }
            }


        ImGui.EndChild();

        }






    }
        public async void ResetGallery(Plugin plugin)
        {
            try
            {           
                for (int g = 0; g < galleryImages.Length; g++)
                {
                    imageIndexVal = 0;
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
            }catch(Exception ex)
            {
                 plugin.chatGUI.PrintError("Could not reset gallery. Results may not be correct " +  ex.Message);
            }
        }
        public async void Reset(Plugin plugin)
        {
            ResetBio(plugin);
            ResetGallery(plugin);
            ResetHooks();
            ResetStory();
        }
        public void ResetBio(Plugin plugin)
        {
            currentAvatarImg = this.persistAvatarHolder;
        }
        public void ResetHooks()
        {
            for (int h = 0; h < hookCount; h++)
            {
                HookContent[h] = string.Empty;
                HookEditContent[h] = string.Empty;
            }
            hookCount = 0;
        }
        public void ResetStory()
        {
            for (int s = 0; s < chapterEditCount; s++)
            {
                ChapterTitle[s] = string.Empty;
                ChapterEditTitle[s] = string.Empty;
                ChapterContent[s] = string.Empty;
                ChapterEditContent[s] = string.Empty;
                chapterCount = 0;
            }



            chapterCount = 0;
            chapterEditCount = 0;
            storyEditTitle = string.Empty;
        }
       
        public void SET_VAL(string mytype, string myvalue)
        {
            this.GetType().GetField(mytype).SetValue(this, myvalue);
            
        }
        public object FindValByName(string PropName)
        {
            PropName = PropName.ToLower();
            var props = this.GetType().GetProperties();

            foreach (var item in props)
            {
                if (item.Name.ToLower() == PropName)
                {
                    return item.GetValue(this);
                }
            }
            return null;
        }
        public static void ClearUI()
        {
            editBio = false;
            editHooks = false;
            editStory = false;
            addOOC = false;
            editOOC = false;
            addGallery = false;
            editGallery = false;
        }
        public void Dispose()
        {
            this.persistAvatarHolder.Dispose();
            avatarHolder.Dispose();
            pictureTab.Dispose();
            currentAvatarImg.Dispose();
            for (int gil = 0; gil < galleryImagesList.Count; gil++)
            {
                galleryImagesList[gil].Dispose();
                plugin.chatGUI.Print("GalleryList Item Removed" + gil.ToString());
            }
            for (int gtl = 0; gtl < galleryThumbsList.Count; gtl++)
            {
                galleryThumbsList[gtl].Dispose();
                plugin.chatGUI.Print("GalleryThumbList Item Removed" + gtl.ToString());
            }
            foreach (IDalamudTextureWrap ti in galleryImages)
            {
                ti.Dispose();
                Array.Clear(galleryImages);
                plugin.chatGUI.Print("GalleryArrImage Image Removed" + ti.ToString());
            }
            foreach (IDalamudTextureWrap gt in galleryThumbs)
            {
                gt.Dispose();
                Array.Clear(galleryThumbs);
                plugin.chatGUI.Print("GalleryArrThumb Image Removed" + gt.ToString());
            }
            for(int o = 0; o < otherImages.Length; o++)
            {
                otherImages[o].Dispose();
                Array.Clear(otherImages);
                plugin.chatGUI.Print("Other Image Removed" + o.ToString());
            }
        }
        public void AddAlignmentSelection()
        {
            var (text, desc) = Constants.AlignmentVals[currentAlignment];
            using var combo = ImRaii.Combo("##Alignment", text);
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
            using var combo = ImRaii.Combo("##Personality Feature #1", text);
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
            using var combo = ImRaii.Combo("##Personality Feature #2", text);
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
            using var combo = ImRaii.Combo("##Personality Feature #3", text);
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
        public override void Update()
        {
            if (DataReceiver.StoryLoadStatus != -1 &&
               DataReceiver.HooksLoadStatus != -1 &&
               DataReceiver.BioLoadStatus != -1 &&
               DataReceiver.GalleryLoadStatus != -1
               )
            {

                AllLoaded = true;
            }
            else
            {
                AllLoaded = false;
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
                    this.avatarBytes = File.ReadAllBytes(imagePath);
                    DataReceiver.currentAvatar = this.avatarBytes;
                    currentAvatarImg = pg.UiBuilder.LoadImage(avatarBytes);
                }
               


            }, 0, null, this.configuration.AlwaysOpenDefaultImport);
        }
       
    }
}
