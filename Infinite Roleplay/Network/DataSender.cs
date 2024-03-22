using InfiniteRoleplay;
using System;

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
    }
    public class DataSender
    {
        public static int userID;
        public static Plugin plugin;
        public static async void SendHelloServer(string internalIP, string externalIP)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CHelloServer);
                buffer.WriteString(internalIP);
                buffer.WriteString(externalIP);
                buffer.WriteString("===New Connection===");
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendHelloServer: " + ex.ToString());
            }
        }
     
        public static async void Login(string username, string password, string playerName, string playerWorld)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CLogin);
                buffer.WriteString(username);
                buffer.WriteString(password);
                buffer.WriteString(playerName);
                buffer.WriteString(playerWorld);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in Login: " + ex.ToString());
            }
        }
        public static async void Register(string username, string password, string email)
        {
            try
            {

                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CRegister);
                buffer.WriteString(username);
                buffer.WriteString(password);
                buffer.WriteString(email);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in Register: " + ex.ToString());
            }
        }
        public static async void ReportProfile(string characterName, string characterWorld, string reporterAccountName, string reportInfo)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CReportProfile);
                buffer.WriteString(characterName);
                buffer.WriteString(characterWorld);
                buffer.WriteString(reporterAccountName);
                buffer.WriteString(reportInfo);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in ReportProfile: " + ex.ToString());
            }

        }    
      
        public static async void SendGalleryImage(string username, string playername, string playerworld, bool NSFW, bool TRIGGER, string url, int index)
        {
            try
            {

                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendGallery);
                buffer.WriteString(playername);
                buffer.WriteString(playerworld);
                buffer.WriteString(url);
                buffer.WriteBool(NSFW);
                buffer.WriteBool(TRIGGER);
                buffer.WriteInteger(index);

                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendGalleryImage: " + ex.ToString());
            }
        }
        public static async void RemoveGalleryImage(string playername, string playerworld, int index, int imageCount)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendGalleryRemoveRequest);
                buffer.WriteString(playername);
                buffer.WriteString(playerworld);

                buffer.WriteInteger(index);
                buffer.WriteInteger(imageCount);

                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendGalleryImage: " + ex.ToString());
            }
        }
        public static async void SendStory(string username, string worldname, string title, string chapters)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendStory);
                buffer.WriteString(username);
                buffer.WriteString(worldname);
                buffer.WriteString(title);
                buffer.WriteString(chapters);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendStory: " + ex.ToString());
            }
        }
     
        public static async void FetchProfile( string characterName, string world)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CFetchProfiles);
                buffer.WriteString(plugin.Configuration.username);
                buffer.WriteString(plugin.Configuration.password);
                buffer.WriteString(characterName);
                buffer.WriteString(world);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch(Exception ex) 
            {
                Dalamud.Logging.PluginLog.LogError("Error in FetchProfile: " + ex.ToString());
            }
        }
        public static async void CreateProfile(string username, string playerName, string playerServer)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CCreateProfile);
                buffer.WriteString(plugin.Configuration.username);
                buffer.WriteString(plugin.Configuration.password);
                buffer.WriteString(playerName);
                buffer.WriteString(playerServer);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch(Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in CreateProfile: " + ex.ToString());
            }
        }
        public static async void BookmarkPlayer(string username, string playerName, string playerWorld)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendPlayerBookmark);
                buffer.WriteString(username);
                buffer.WriteString(playerName);
                buffer.WriteString(playerWorld);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in BookmarkProfile: " + ex.ToString());
            }

        }   
        public static async void RemoveBookmarkedPlayer(string username, string playerName, string playerWorld)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendRemovePlayerBookmark);
                buffer.WriteString(username);
                buffer.WriteString(playerName);
                buffer.WriteString(playerWorld);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in RemoveBookmarkedPlayer: " + ex.ToString());
            }
        }
        public static async void RequestBookmarks(string username)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendBookmarkRequest);
                buffer.WriteString(username);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in RequestBookmarks: " + ex.ToString());
            }

        }
       
        public static async void SubmitProfileBio(string playerName, string playerServer, byte[] avatarBytes, string name, string race, string gender, string age, 
                                            string height, string weight, string atFirstGlance, int alignment, int personality_1, int personality_2, int personality_3)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CCreateProfileBio);
                buffer.WriteString(plugin.Configuration.username);
                buffer.WriteString(plugin.Configuration.password);
                buffer.WriteString(playerName);
                buffer.WriteString(playerServer);
                buffer.WriteInteger(avatarBytes.Length);
                buffer.WriteBytes(avatarBytes);
                buffer.WriteString(name);
                buffer.WriteString(race);
                buffer.WriteString(gender);
                buffer.WriteString(age);
                buffer.WriteString(height);
                buffer.WriteString(weight);
                buffer.WriteString(atFirstGlance);
                buffer.WriteInteger(alignment);
                buffer.WriteInteger(personality_1);
                buffer.WriteInteger(personality_2);
                buffer.WriteInteger(personality_3);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SubmitProfileBio: " + ex.ToString());
            }

        }

        
        public static async void RequestTargetProfile(string targetPlayerName, string targetPlayerWorld, string requesterUsername)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.SRequestTargetProfile);
                buffer.WriteString(requesterUsername);
                buffer.WriteString(targetPlayerName);
                buffer.WriteString(targetPlayerWorld);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SubmitProfileBio: " + ex.ToString());
            }

        }
        public static async void SendHooks(string charactername, string characterworld, string hooks)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendHooks);
                buffer.WriteString(charactername);
                buffer.WriteString(characterworld);
                buffer.WriteString(hooks);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendHooks: " + ex.ToString());
            }

        }
       
        
     
        public static async void AddProfileNotes(string username, string characterNameVal, string characterWorldVal, string notes)
        {
            try
            {

                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.CSendProfileNotes);
                buffer.WriteString(username);
                buffer.WriteString(characterNameVal);
                buffer.WriteString(characterWorldVal);
                buffer.WriteString(notes);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in AddProfileNotes: " + ex.ToString());
            }
}

        internal static async void SendVerification(string username, string verificationKey)
        {
            try 
            { 
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.SSubmitVerificationKey);
                buffer.WriteString(username);
                buffer.WriteString(verificationKey);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendVerification: " + ex.ToString());
            }

        }

        internal static async void SendRestorationRequest(string restorationEmail)
        {
            try 
            { 
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.SSubmitRestorationRequest);
                buffer.WriteString(restorationEmail);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendRestorationRequest: " + ex.ToString());
            }
        }

        internal static async void SendRestoration(string email, string password, string restorationKey)
        {
            try
            { 
                var buffer = new ByteBuffer();
                buffer.WriteInteger((int)ClientPackets.SSubmitRestorationKey);
                buffer.WriteString(password);
                buffer.WriteString(restorationKey);
                buffer.WriteString(email);
                await ClientTCP.SendData(buffer.ToArray());
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendRestoration: " + ex.ToString());
            }
}

        internal static async void SendOOCInfo(string username, string password, string OOC)
        {
            try 
            { 
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SSendOOC);
            buffer.WriteString(username);
            buffer.WriteString(password);
            buffer.WriteString(OOC);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Error in SendOOCInfo: " + ex.ToString());
            }
        }

    }
}
