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
using Dalamud.Interface.Utility.Raii;

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
        public static string loading; //loading status string for loading the profile gallery mainly
        public static float percentage = 0f; //loading base value
        private Plugin plugin;
        private DalamudPluginInterface pg;
        private FileDialogManager _fileDialogManager; //for avatars only at the moment
        public Configuration configuration;
        public static int galleryImageCount = 0;
        public static IDalamudTextureWrap pictureTab; //picturetab.png for base picture in gallery
        public static string[] HookNames = new string[31];
        public static string[] HookContents = new string[31];
        public static string[] ChapterContents = new string[31];
        public static string[] ChapterNames = new string[31];
        public static string[] imageURLs = new string[31];
        public static bool[] NSFW = new bool[31]; //gallery images NSFW status
        public static bool[] TRIGGER = new bool[31]; //gallery images TRIGGER status
        public static bool[] ImageExists = new bool[31]; //used to check if an image exists in the gallery
        public static bool[] viewChapter = new bool[31]; //to check which chapter we are currently viewing
        public static bool[] hookExists = new bool[31]; //same as ImageExists but for hooks
        public static bool[] storyChapterExists = new bool[31]; //same again but for story chapters
        public static SortedList<TabValue, bool> TabOpen = new SortedList<TabValue, bool>(); //what part of the profile we have open
        public static bool editAvatar, addProfile, editProfile, ReorderGallery, addGalleryImageGUI, alignmentHidden, personalityHidden, loadPreview = false;
        public static string oocInfo, storyTitle = string.Empty;
        public static bool ExistingProfile, ExistingStory, ExistingOOC, ExistingHooks, ExistingGallery, ExistingBio, ReorderHooks, ReorderChapters, AddHooks, AddStoryChapter; //to check if we have data from the DataReceiver for the respective fields or to reorder the gallery or hooks after deletion
        public static int chapterCount, currentAlignment, currentPersonality_1, currentPersonality_2, currentPersonality_3, hookCount = 0; //values changed by DataReceiver as well
        public static byte[] avatarBytes; //avatar image in a byte array
        public static float loaderInd; //used for the gallery loading bar
        public static IDalamudTextureWrap avatarHolder, currentAvatarImg;
        public static List<IDalamudTextureWrap> galleryThumbsList = new List<IDalamudTextureWrap>();
        public static List<IDalamudTextureWrap> galleryImagesList = new List<IDalamudTextureWrap>();
        public static IDalamudTextureWrap[] galleryImages, galleryThumbs;
        public static string[] bioFieldsArr = new string[7]; //fields such as name, race, gender and so on
        private IDalamudTextureWrap persistAvatarHolder;
        public static bool drawChapter;
        public static int storyChapterCount = -1;
        public static int currentChapter;
        public static bool privateProfile; //sets whether the profile is allowed to be publicly viewed

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
           

        }
        public override void OnOpen()
        {
            TabOpen.Clear(); //clear our TabOpen array before populating again
            var avatarHolderImage = Constants.UICommonImage(plugin, Constants.CommonImageTypes.avatarHolder); //Load the avatarHolder TextureWrap from Constants.UICommonImage
            if (avatarHolderImage != null)
            {
                avatarHolder = avatarHolderImage; //set our avatarHolder ot the same TextureWrap
            }
            //same for pictureTab
            var pictureTabImage = Constants.UICommonImage(plugin, Constants.CommonImageTypes.blankPictureTab);
            if (pictureTabImage != null)
            {
                pictureTab = pictureTabImage;
            }
            this.persistAvatarHolder = avatarHolder; //unneeded at the moment, but I seem to keep needing and not needing it so I am leaving it for now.
            for (int bf = 0; bf < bioFieldsArr.Length; bf++)
            {
                //set all the bioFields to an empty string
                bioFieldsArr[bf] = string.Empty;
            }
            foreach (TabValue tab in Enum.GetValues(typeof(TabValue)))
            {
                TabOpen.Add(tab, false); //set all tabs to be closed by default
            }
            //set the base value for our arrays and lists
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

            //set all our text entry fields for the bio to empty strings
            for (int b = 0; b < bioFieldsArr.Length; b++)
            {
                bioFieldsArr[b] = string.Empty;
            }

            //set the avatar to the avatar_holder.png by default
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {
                avatarBytes = File.ReadAllBytes(Path.Combine(path, "UI/common/profiles/avatar_holder.png"));
            }
        }
        //method to check if we have loaded our data received from the server
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
            //if we have loaded all the data received from the server and we are logged in game
            if (AllLoaded() == true && plugin.IsOnline())
            {
                _fileDialogManager.Draw(); //file dialog mainly for avatar atm. galleries later possibly.


                if (ExistingProfile == true)//if we have a profile add the edit profile button
                {                    
                    if (ImGui.Button("Edit Profile", new Vector2(100, 20))) { editProfile = true; }

                    ImGui.SameLine();
                    if (ImGui.Checkbox("Set Private", ref privateProfile))
                    {
                        //send our privacy settings to the server
                        DataSender.SetProfileStatus(plugin.Configuration.username.ToString(), player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), privateProfile);
                    }
                }
                if (ExistingProfile == false) //else create our add profile button to create a new profile
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
                using var ProfileTable = ImRaii.Child("PROFILE");
                if(ProfileTable) 
                {
                    #region BIO
                    if (TabOpen[TabValue.Bio])
                    {
                        //display for avatar
                        ImGui.Image(currentAvatarImg.ImGuiHandle, new Vector2(100, 100));

                        if (ImGui.Button("Edit Avatar"))
                        {
                            editAvatar = true; //used to open the file dialog
                        }
                        ImGui.Spacing();
                        //simple for loop to get through our bio text fields
                        for (int i = 0; i < Constants.BioFieldVals.Length; i++)
                        {
                            var BioField = Constants.BioFieldVals[i];
                            //if our input type is single line 
                            if (BioField.Item4 == Constants.InputTypes.single)
                            {
                                ImGui.Text(BioField.Item1);
                                //if our label is not AFG use sameline
                                if (BioField.Item1 != "AT FIRST GLANCE:")
                                {
                                    ImGui.SameLine();
                                }
                                //add the input text for the field
                                ImGui.InputTextWithHint(BioField.Item2, BioField.Item3, ref bioFieldsArr[i], 100);
                            }
                            else
                            {
                                //text must be multiline so add the multiline field/fields
                                ImGui.Text(BioField.Item1);
                                ImGui.InputTextMultiline(BioField.Item2, ref bioFieldsArr[i], 3100, new Vector2(500, 150));
                            }
                        }
                        ImGui.Spacing();
                        ImGui.Spacing();

                        ImGui.TextColored(new Vector4(1, 1, 1, 1), "ALIGNMENT:");
                        AddAlignmentSelection(); //add alignment combo selection

                        ImGui.Spacing();

                        ImGui.TextColored(new Vector4(1, 1, 1, 1), "PERSONALITY TRAITS:");
                        //add personality combos
                        AddPersonalitySelection_1(); 
                        AddPersonalitySelection_2();
                        AddPersonalitySelection_3();
                        if (ImGui.Button("Save Bio"))
                        {
                            //submit our bio to the server
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
                            //create a new List to hold our hook values
                            List<Tuple<int, string, string>> hooks = new List<Tuple<int, string, string>>();
                            for (int i = 0; i < hookCount; i++)
                            {
                                //create a new hook tuple to add to the list
                                Tuple<int, string, string> hook = Tuple.Create(i, HookNames[i], HookContents[i]);
                                hooks.Add(hook);
                            }
                            //send the data to the server
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
                        //add our chapter combo select input
                        AddChapterSelection();
                        ImGui.SameLine();
                        if (ImGui.Button("Add Chapter"))
                        {
                            CreateChapter();
                        }

                        using (OtterGui.Raii.ImRaii.Disabled(!storyChapterExists.Any(x => x))) //disable if no stories chapters exist
                        {
                            if (ImGui.Button("Submit Story"))
                            {
                                //create a new list for our stories to be held in
                                List<Tuple<string, string>> storyChapters = new List<Tuple<string, string>>();
                                for (int i = 0; i < storyChapterCount + 1; i++)
                                {
                                    //get the data from our chapterNames and Content and store them in a tuple ot be added in the storyChapters list
                                    string chapterName = ChapterNames[i].ToString();
                                    string chapterContent = ChapterContents[i].ToString();
                                    Tuple<string, string> chapter = Tuple.Create(chapterName, chapterContent);
                                    storyChapters.Add(chapter);
                                }
                                //finally send the story data to the server
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
                            if (galleryImageCount < 28)
                            {
                                galleryImageCount++;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Submit Gallery"))
                        {
                            for (int i = 0; i < galleryImageCount; i++)
                            {
                                //pretty simple stuff, just send the gallery related array values to the server
                                DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                                    NSFW[i], TRIGGER[i], imageURLs[i], i);

                            }

                        }
                        ImGui.NewLine();
                        addGalleryImageGUI = true;
                        ImageExists[galleryImageCount] = true;
                    }
                    #endregion
                    #region OOC

                    if (TabOpen[TabValue.OOC])
                    {
                        ImGui.InputTextMultiline("##OOC", ref oocInfo, 50000, new Vector2(500, 600));
                        if (ImGui.Button("Submit OOC"))
                        {
                            //send the OOC info to the server, just a string really
                            DataSender.SendOOCInfo(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), oocInfo);
                        }
                    }
                    #endregion
                    if (loadPreview == true)
                    {
                        //load gallery image preview if requested
                        plugin.OpenImagePreview();
                        loadPreview = false;
                    }
                    if (addGalleryImageGUI == true)
                    {
                        AddImageToGallery(plugin, galleryImageCount); //used to add our image to the gallery
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

                    //if true, reorders the gallery
                    if (ReorderGallery == true)
                    {
                        ReorderGallery = false;
                        
                        bool nextExists = ImageExists[NextAvailableImageIndex() + 1];//bool to check if the next image in the list exists
                        int firstOpen = NextAvailableImageIndex(); //index of the first image that does not exist
                        ImageExists[firstOpen] = true; //set the image to exist again
                        
                        if (nextExists) // if our next image in the list exists
                        {
                            for (int i = firstOpen; i < galleryImageCount; i++)
                            {
                                //swap the image behind it to the one ahead, along with hte imageUrl and such
                                galleryImages[i] = galleryImages[i + 1];
                                galleryThumbs[i] = galleryThumbs[i + 1];
                                imageURLs[i] = imageURLs[i + 1];
                                NSFW[i] = NSFW[i + 1];
                                TRIGGER[i] = TRIGGER[i + 1];

                            }
                        }
                        //lower the overall image count
                        galleryImageCount--;
                        //set the gallery image we removed back to the base picturetab.png
                        galleryImages[galleryImageCount] = pictureTab;
                        galleryThumbs[galleryImageCount] = pictureTab;
                        //set the image to not exist until added again
                        ImageExists[galleryImageCount] = false;

                    }
                    //pretty much the same logic but with our hooks
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
                    //same for chapters aswell
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
                //if our content is not all loaded use the loader
                Misc.StartLoader(loaderInd, percentage, loading);
            }
            
        }
        public void CreateChapter()
        {
            if (storyChapterCount < 30)
            {
                storyChapterCount++; //increase chapter count
                storyChapterExists[storyChapterCount] = true; //set our chapter to exist
                ChapterNames[storyChapterCount] = "New Chapter"; //set a base title
                currentChapter = storyChapterCount; //switch our current selected chapter to the one we just made
                viewChapter[storyChapterCount] = true; //view the chapter we just made aswell
            }

        }
        public void RemoveChapter(int index)
        {
            storyChapterCount--; //reduce our chapter count
            storyChapterExists[index] = false; //set the image to not exist
            ChapterNames[index] = string.Empty; //reset the name
            ChapterContents[index] = string.Empty; //reset the contents
            //if the story behind it exists
            if (storyChapterExists[index - 1] == true)
            {
                //we switch to that chapter to view it instead.
                currentChapter = index - 1;
                viewChapter[index - 1] = true;
            }
            ReorderChapters = true; //finally reorder chapters

        }
        public void ClearChaptersInView() //not used at the moment
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
                //if our chapter exists and we are viewing it
                if (storyChapterExists[i] == true && viewChapter[i] == true)
                {
                    //create a new child with the scale of the window size but inset slightly
                    Vector2 windowSize = ImGui.GetWindowSize();
                    using var profileTable = ImRaii.Child("##Chapter" + i, new Vector2(windowSize.X - 20, windowSize.Y - 130));
                    if(profileTable)
                    {
                        //set an input size for our input text as well to adjust with window scale
                        Vector2 inputSize = new Vector2(windowSize.X - 30, windowSize.Y - 200); // Adjust as needed
                        ImGui.InputTextMultiline("##ChapterContent" + i, ref ChapterContents[i], 5000, inputSize);

                        using var chapterControlTable = ImRaii.Child("##ChapterControls" + i);
                        if(chapterControlTable)
                        {
                            using (OtterGui.Raii.ImRaii.Disabled(!Plugin.CtrlPressed()))
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
                    }


                }
            }
        }
        //simply draws the hook with the specified index and controls for said hook to the window
        public void DrawHook(int i, Plugin plugin)
        {
            if (hookExists[i] == true)
            {
                
                using var hookChild = ImRaii.Child("##Hook" + i, new Vector2(550, 250));
                if(hookChild)
                {
                    ImGui.InputTextWithHint("##HookName" + i, "Hook Name", ref HookNames[i], 300);
                    ImGui.InputTextMultiline("##HookContent" + i, ref HookContents[i], 5000, new Vector2(500, 200));

                    try
                    {

                        using var hookControlsTable = ImRaii.Child("##HookControls" + i);
                        if(hookControlsTable)
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
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        //adds an image to the gallery with the specified index with a table 4 columns wide
        public void AddImageToGallery(Plugin plugin, int imageIndex)
        {
            if (TabOpen[TabValue.Gallery])
            {
                using var table = ImRaii.Table("table_name", 4);
                if (table)
                {
                    for (int i = 0; i < imageIndex; i++)
                    {
                        ImGui.TableNextColumn();
                        DrawGalleryImage(i);
                    }
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

        //gets the next image index that does not exist
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
        //gets the next chapter index that does not exist
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
        //gets the next hook index that does not exist
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

                using var galleryImageChild = ImRaii.Child("##GalleryImage" + i, new Vector2(150, 280));
                if(galleryImageChild)
                {
                    ImGui.Text("Will this image be 18+ ?");
                    if (ImGui.Checkbox("Yes 18+", ref NSFW[i]))
                    {
                        for (int g = 0; g < galleryImageCount; g++)
                        {
                            //send galleryImages on value change of 18+ incase the user forgets to hit submit gallery
                            DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                              NSFW[g], TRIGGER[g], imageURLs[g], g);

                        }
                    }
                    ImGui.Text("Is this a possible trigger ?");
                    if (ImGui.Checkbox("Yes Triggering", ref TRIGGER[i]))
                    {
                        for (int g = 0; g < galleryImageCount; g++)
                        {
                            //same for triggering, we don't want to lose this info if the user is forgetful
                            DataSender.SendGalleryImage(configuration.username, player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(),
                                              NSFW[g], TRIGGER[g], imageURLs[g], g);
                        }
                    }
                    ImGui.InputTextWithHint("##ImageURL" + i, "Image URL", ref imageURLs[i], 300);
                    try
                    {
                        //maximize the gallery image to preview it.
                        ImGui.Image(galleryThumbs[i].ImGuiHandle, new Vector2(galleryThumbs[i].Width, galleryThumbs[i].Height));
                        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Click to enlarge"); }
                        if (ImGui.IsItemClicked())
                        {
                            ImagePreview.width = galleryImages[i].Width;
                            ImagePreview.height = galleryImages[i].Height;
                            ImagePreview.PreviewImage = galleryImages[i];
                            loadPreview = true;
                        }


                        using var galleryImageControlsTable = ImRaii.Child("##GalleryImageControls" + i);
                        if (galleryImageControlsTable)
                        {
                            using (OtterGui.Raii.ImRaii.Disabled(!InfiniteRoleplay.Plugin.CtrlPressed()))
                            {
                                //button to remove the gallery image
                                if (ImGui.Button("Remove##" + "gallery_remove" + i))
                                {
                                    ImageExists[i] = false;
                                    ReorderGallery = true;
                                    //remove the image immediately once pressed
                                    DataSender.RemoveGalleryImage(player.Name.ToString(), player.HomeWorld.GameData.Name.ToString(), i, galleryImageCount);
                                }
                            }
                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                            {
                                ImGui.SetTooltip("Ctrl Click to Enable");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }






        }
        //method to reset the entire gallery to default (NOT CURRENTLY IN USE)
        public async void ResetGallery()
        {
            try
            {
                for (int g = 0; g < galleryImages.Length; g++)
                {
                    galleryImageCount = 0;
                    ReorderGallery = true;
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
                plugin.logger.Error("Could not reset gallery:: Results may be incorrect.");
            }
        }
        //method ot reset the entire story section
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

        //reset our tabs and go back to base ui with no tab selected
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


