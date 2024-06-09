using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using InfiniteRoleplay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Networking
{
    public enum ClientPackets
    {
        CHelloServer = 1,
        CLogin = 2,
        CCreateProfile = 3,
        CFetchProfiles = 4,
        CSendNewSystem = 5,
        CSendRulebookPageContent = 6,
        CSendRulebookPage = 7,
        CSendSheetVerify = 8,
        CSendSystemStats = 9,
        CCreateProfileBio = 10,
        CBanAccount = 11,
        CStrikeAccount = 12,
        CEditProfileBio = 13,
        CSendHooks = 14,
        SRequestTargetProfile = 15,
        CRegister = 16,
        CDeleteHook = 17,
        CSendStory = 18,
        CSendLocation = 19,
        CSendBookmarkRequest = 20,
        CSendPlayerBookmark = 21,
        CSendRemovePlayerBookmark = 22,
        CSendGalleryImage = 23,
        CSendGalleryImagesReceived = 24,
        CSendGalleryImageRequest = 25,
        CSendGalleryRemoveRequest = 26,
        CReorderGallery = 27,
        CSendNSFWStatus = 28,
        CSendGallery = 29,
        CReportProfile = 30,
        CSendProfileNotes = 31,
        SSubmitVerificationKey = 32,
        SSubmitRestorationRequest = 33,
        SSubmitRestorationKey = 34,
        SSendOOC = 35,
        SSendUserConfiguration = 36,
        SendProfileConfiguration = 37,
        SSendProfileViewRequest = 38,
        SSendProfileAccessUpdate = 39,
        SSendConnectionsRequest = 40,
        SSendProfileStatus = 41,
    }
    public enum LogLevels
    {
        Log = 0,
        LogInformation = 1,
        LogDebug = 2,
        LogWarning = 3,
        LogError = 4,
    }
    public class DataSender
    {
        public static int userID;
        public static Plugin plugin;

        public static void PrintMessage(string message, LogLevels logLevel)
        {
            if (logLevel == LogLevels.Log)
            {
                Dalamud.Logging.PluginLog.Log(message);
            }
            if (logLevel == LogLevels.LogInformation)
            {
                Dalamud.Logging.PluginLog.LogInformation(message);
            }
            if (logLevel == LogLevels.LogDebug)
            {
                Dalamud.Logging.PluginLog.LogDebug(message);
            }
            if (logLevel == LogLevels.LogWarning)
            {
                Dalamud.Logging.PluginLog.Warning(message);
            }
            if (logLevel == LogLevels.LogError)
            {
                Dalamud.Logging.PluginLog.LogError(message);
            }

        }

        public static async void Login(string username, string password, string playerName, string playerWorld)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.CLogin);
                    buffer.WriteString(username);
                    buffer.WriteString(password);
                    buffer.WriteString(playerName);
                    buffer.WriteString(playerWorld);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in Login: " + ex.ToString(), LogLevels.LogError);
            }
        }
      
        public static async void Register(string username, string password, string email)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CRegister);
                buffer.WriteString(username);
                buffer.WriteString(password);
                buffer.WriteString(email);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in Register: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void ReportProfile(string reporterAccount, string playerName, string playerWorld, string reportInfo)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.CReportProfile);
                    buffer.WriteString(playerName);
                    buffer.WriteString(playerWorld);
                    buffer.WriteString(reporterAccount);
                    buffer.WriteString(reportInfo);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in ReportProfile: " + ex.ToString(), LogLevels.LogError);
            }

        }
        public static async void SendGalleryImage(string username, string playername, string playerworld, bool NSFW, bool TRIGGER, string url, int index)
        {
            try
            {

                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendGallery);
                buffer.WriteString(playername);
                buffer.WriteString(playerworld);
                buffer.WriteString(url);
                buffer.WriteBool(NSFW);
                buffer.WriteBool(TRIGGER);
                buffer.WriteInt(index);

                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendGalleryImage: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void RemoveGalleryImage(string playername, string playerworld, int index, int imageCount)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendGalleryRemoveRequest);
                buffer.WriteString(playername);
                buffer.WriteString(playerworld);

                buffer.WriteInt(index);
                buffer.WriteInt(imageCount);

                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendGalleryImage: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void SendStory(string playername, string worldname, string storyTitle, List<Tuple<string, string>> storyChapters)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendStory);
                buffer.WriteString(playername);
                buffer.WriteString(worldname);
                buffer.WriteInt(storyChapters.Count);
                buffer.WriteString(storyTitle);
                for (int i = 0; i < storyChapters.Count; i++)
                {
                    buffer.WriteString(storyChapters[i].Item1);
                    buffer.WriteString(storyChapters[i].Item2);
                }
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendStory: " + ex.ToString(), LogLevels.LogError);
            }
        }
       
        public static async void SendProfileAccessUpdate(string connectionName, string connectionWorld, int status)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.SSendProfileAccessUpdate);
                    buffer.WriteString(plugin.Configuration.username.ToString());
                    buffer.WriteString(plugin.ClientState.LocalPlayer.Name.ToString());
                    buffer.WriteString(plugin.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString());
                    buffer.WriteString(connectionName);
                    buffer.WriteString(connectionWorld);
                    buffer.WriteInt(status);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in Login: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void FetchProfile(string characterName, string world)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CFetchProfiles);
                buffer.WriteString(characterName);
                buffer.WriteString(world);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in FetchProfile: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void CreateProfile(string playerName, string playerServer)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CCreateProfile);
                buffer.WriteString(playerName);
                buffer.WriteString(playerServer);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in CreateProfile: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void BookmarkPlayer(string username, string playerName, string playerWorld)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendPlayerBookmark);
                buffer.WriteString(playerName);
                buffer.WriteString(playerWorld);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in BookmarkProfile: " + ex.ToString(), LogLevels.LogError);
            }

        }
        public static async void RemoveBookmarkedPlayer(string username, string playerName, string playerWorld)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendRemovePlayerBookmark);
                buffer.WriteString(playerName);
                buffer.WriteString(playerWorld);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in RemoveBookmarkedPlayer: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void RequestBookmarks(string username)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendBookmarkRequest);
                buffer.WriteString(username);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in RequestBookmarks: " + ex.ToString(), LogLevels.LogError);
            }

        }

        public static async void SubmitProfileBio(string playerName, string playerServer, byte[] avatarBytes, string name, string race, string gender, string age,
                                            string height, string weight, string atFirstGlance, int alignment, int personality_1, int personality_2, int personality_3)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CCreateProfileBio);
                buffer.WriteString(playerName);
                buffer.WriteString(playerServer);
                buffer.WriteInt(avatarBytes.Length);
                buffer.WriteBytes(avatarBytes);
                buffer.WriteString(name);
                buffer.WriteString(race);
                buffer.WriteString(gender);
                buffer.WriteString(age);
                buffer.WriteString(height);
                buffer.WriteString(weight);
                buffer.WriteString(atFirstGlance);
                buffer.WriteInt(alignment);
                buffer.WriteInt(personality_1);
                buffer.WriteInt(personality_2);
                buffer.WriteInt(personality_3);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SubmitProfileBio: " + ex.ToString(), LogLevels.LogError);
            }

        }
        public static async void SaveUserConfiguration(bool showProfilesPublicly)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.SSendUserConfiguration);
                    buffer.WriteBool(showProfilesPublicly);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in sending user configuration: " + ex.ToString(), LogLevels.LogError);
            }
        }
        public static async void SaveProfileConfiguration(bool showProfilePublicly, string playerName, string playerWorld)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.SendProfileConfiguration);
                    buffer.WriteBool(showProfilePublicly);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in sending user configuration: " + ex.ToString(), LogLevels.LogError);
            }
        }

        public static async void RequestTargetProfile(string targetPlayerName, string targetPlayerWorld, string requesterUsername)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.SRequestTargetProfile);
                buffer.WriteString(requesterUsername);
                buffer.WriteString(targetPlayerName);
                buffer.WriteString(targetPlayerWorld);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SubmitProfileBio: " + ex.ToString(), LogLevels.LogError);
            }

        }
        public static async void SendHooks(string charactername, string characterworld, List<Tuple<int, string, string>> hooks)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendHooks);
                buffer.WriteString(charactername);
                buffer.WriteString(characterworld);
                buffer.WriteInt(hooks.Count);
                for (int i = 0; i < hooks.Count; i++)
                {
                    buffer.WriteInt(hooks[i].Item1);
                    buffer.WriteString(hooks[i].Item2);
                    buffer.WriteString(hooks[i].Item3);
                }
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendHooks: " + ex.ToString(), LogLevels.LogError);
            }

        }



        public static async void AddProfileNotes(string username, string characterNameVal, string characterWorldVal, string notes)
        {
            try
            {

                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.CSendProfileNotes);
                buffer.WriteString(username);
                buffer.WriteString(characterNameVal);
                buffer.WriteString(characterWorldVal);
                buffer.WriteString(notes);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in AddProfileNotes: " + ex.ToString(), LogLevels.LogError);
            }
        }

        internal static async void SendVerification(string username, string verificationKey)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.SSubmitVerificationKey);
                buffer.WriteString(username);
                buffer.WriteString(verificationKey);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendVerification: " + ex.ToString(), LogLevels.LogError);
            }

        }

        internal static async void SendRestorationRequest(string restorationEmail)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.SSubmitRestorationRequest);
                buffer.WriteString(restorationEmail);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendRestorationRequest: " + ex.ToString(), LogLevels.LogError);
            }
        }

        internal static async void SendRestoration(string email, string password, string restorationKey)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.SSubmitRestorationKey);
                buffer.WriteString(password);
                buffer.WriteString(restorationKey);
                buffer.WriteString(email);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendRestoration: " + ex.ToString(), LogLevels.LogError);
            }
        }

        internal static async void SendOOCInfo(string charactername, string characterworld, string OOC)
        {
            try
            {
                using (var buffer = new ByteBuffer()){
                buffer.WriteInt((int)ClientPackets.SSendOOC);
                buffer.WriteString(charactername);
                buffer.WriteString(characterworld);
                buffer.WriteString(OOC);
                await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SendOOCInfo: " + ex.ToString(), LogLevels.LogError);
            }
        }


        internal static async void RequestConnections(string username)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.SSendConnectionsRequest);
                    buffer.WriteString(username);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in RequestConnections: " + ex.ToString(), LogLevels.LogError);
            }
        }


        internal static async void SetProfileStatus(string username, string characterName, string characterWorld, bool status)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt((int)ClientPackets.SSendProfileStatus);
                    buffer.WriteString(username);
                    buffer.WriteString(characterName);
                    buffer.WriteString(characterWorld);
                    buffer.WriteBool(status);
                    await ClientTCP.SendDataAsync(buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                PrintMessage("Error in SetProfileStatus: " + ex.ToString(), LogLevels.LogError);
            }
        }
    }
}
