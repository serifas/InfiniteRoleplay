
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using InfiniteRoleplay;
using InfiniteRoleplay.Windows;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Timers;

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
        // public static LoadCharacter assets = new LoadCharacter();
        public static async void SendHelloServer(string internalIP, string externalIP)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CHelloServer);
            buffer.WriteString(internalIP);
            buffer.WriteString(externalIP);
            buffer.WriteString("===New Connection===");
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void SendLocation(string location)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendLocation);
            buffer.WriteString(location);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        
        public static async void SendImagesReceived(string username, string world)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendGalleryImagesReceived);
            buffer.WriteString(username);
            buffer.WriteString(world);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void Login(string username, string password, string playerName, string playerWorld)
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
        public static async void Register(string username, string password, string email)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CRegister);
            buffer.WriteString(username);
            buffer.WriteString(password);
            buffer.WriteString(email);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void ReportProfile(string characterName, string characterWorld, string reporterAccountName, string reportInfo)
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
        public static async void SendSystemStats(string username, string SystemName, string Msg)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendSystemStats);
            buffer.WriteString(username);
            buffer.WriteString(SystemName);
            buffer.WriteString(Msg);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void RequestGalleryImage(string imageName)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendGalleryImageRequest);
            buffer.WriteString(imageName);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void ReorderGallery(string playername, string playerworld, int oldKey, int newKey, int endIndex, int removedIndex)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CReorderGallery);
            buffer.WriteString(playername);
            buffer.WriteString(playerworld);
            buffer.WriteInteger(oldKey);
            buffer.WriteInteger(newKey);
            buffer.WriteInteger(endIndex);
            buffer.WriteInteger(removedIndex);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void MarkImageNSFW(string playerName, string playerWorld, int index, bool nsfw)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendNSFWStatus);
            buffer.WriteString(playerName);
            buffer.WriteString(playerWorld);
            buffer.WriteInteger(index);
            buffer.WriteBool(nsfw);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void SendGalleryImage(string username, string playername, string playerworld, bool NSFW, bool TRIGGER, string url, int index)
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
        public static async void RemoveGalleryImage(string playername, string playerworld, int index, int imageCount)
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
        public static async void SendStory(string username, string worldname, string title, string chapters)
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
        public static async void SendNewSystem(string username, string name, string description, byte[] systemImage, int max_stats, int max_stat_points_per_stat, int max_stat_reduction, int max_stat_reduction_per_stat, int stat_allocation_allowed, int stat_reduction_allowed)
        {

            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendNewSystem);
            buffer.WriteString(username);
            buffer.WriteString(name);
            buffer.WriteString(description);
            buffer.WriteInteger(systemImage.Length);
            buffer.WriteBytes(systemImage);
            buffer.WriteInteger(max_stats);
            buffer.WriteInteger(max_stat_points_per_stat);
            buffer.WriteInteger(max_stat_reduction);
            buffer.WriteInteger(max_stat_reduction_per_stat);
            buffer.WriteInteger(stat_allocation_allowed);
            buffer.WriteInteger(stat_reduction_allowed);
           
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void FetchProfile( string characterName, string world)
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
        public static async void CreateProfile(string username, string playerName, string playerServer)
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
        public static async void StrikeAccount(string senderName, string receiverName)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CStrikeAccount);
            buffer.WriteString(senderName);
            buffer.WriteString(receiverName);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void BookmarkPlayer(string username, string playerName, string playerWorld)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendPlayerBookmark);
            buffer.WriteString(username);
            buffer.WriteString(playerName);
            buffer.WriteString(playerWorld);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }   
        public static async void RemoveBookmarkedPlayer(string username, string playerName, string playerWorld)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendRemovePlayerBookmark);
            buffer.WriteString(username);
            buffer.WriteString(playerName);
            buffer.WriteString(playerWorld);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public static async void RequestBookmarks(string username)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendBookmarkRequest);
            buffer.WriteString(username);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void EditProfileBio(string playerName, string playerServer, byte[] avatarBytes, string name, string race, string gender, int age,
                                            string height, string weight, string atFirstGlance,
                                            int lawful_good, int neutral_good, int chaotic_good,
                                            int lawful_neutral, int true_neutral, int chaotic_neutral,
                                            int lawful_evil, int neutral_evil, int chaotic_evil)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CEditProfileBio);
            buffer.WriteString(playerName);
            buffer.WriteString(playerServer);
            buffer.WriteInteger(avatarBytes.Length);
            buffer.WriteBytes(avatarBytes);
            buffer.WriteString(name);
            buffer.WriteString(race);
            buffer.WriteString(gender);
            buffer.WriteInteger(age);
            buffer.WriteString(height);
            buffer.WriteString(weight);
            buffer.WriteString(atFirstGlance);
            buffer.WriteInteger(lawful_good);
            buffer.WriteInteger(neutral_good);
            buffer.WriteInteger(chaotic_good);
            buffer.WriteInteger(lawful_neutral);
            buffer.WriteInteger(true_neutral);
            buffer.WriteInteger(chaotic_neutral);
            buffer.WriteInteger(lawful_evil);
            buffer.WriteInteger(neutral_evil);
            buffer.WriteInteger(chaotic_evil);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void SubmitProfileBio(string playerName, string playerServer, byte[] avatarBytes, string name, string race, string gender, string age, 
                                            string height, string weight, string atFirstGlance, int alignment, int personality_1, int personality_2, int personality_3)
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

        public static async void SendRulebookPage(string username, string title)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendRulebookPage);
            buffer.WriteString(username);
            buffer.WriteString(title);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void SendRulebookPageContent(string username, string title, string content)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendRulebookPageContent);
            buffer.WriteString(username);
            buffer.WriteString(title);
            buffer.WriteString(content);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void RequestTargetProfile(string targetPlayerName, string targetPlayerWorld, string requesterUsername)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SRequestTargetProfile);
            buffer.WriteString(requesterUsername);
            buffer.WriteString(targetPlayerName);
            buffer.WriteString(targetPlayerWorld);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void SendHooks(string charactername, string characterworld, string hooks)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendHooks);
            buffer.WriteString(charactername);
            buffer.WriteString(characterworld);
            buffer.WriteString(hooks);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void DeleteHooks(string charactername, string characterworld, string hookMsg)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CDeleteHook);
            buffer.WriteString(charactername);
            buffer.WriteString(characterworld);
            buffer.WriteString(hookMsg);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();

        }
        
        public static async void UpdateSheetStatus(int sheetID, int status)
        {

            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendSheetVerify);
            buffer.WriteInteger(sheetID);
            buffer.WriteInteger(status);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }
        public static async void CreateSystem(string username, string name, string description, byte[] systemImage, int max_stats, int max_stat_points_per_stat, int max_stat_reduction, int max_stat_reduction_per_stat, int stat_allocation_allowed, int stat_reduction_allowed,int statCount)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CSendNewSystem);
            buffer.WriteString(username);
                buffer.WriteString(name);
                buffer.WriteString(description);
                buffer.WriteInteger(systemImage.Length);
                buffer.WriteBytes(systemImage);
                buffer.WriteInteger(max_stats);
                buffer.WriteInteger(max_stat_points_per_stat);
                buffer.WriteInteger(max_stat_reduction);
                buffer.WriteInteger(max_stat_reduction_per_stat);
                buffer.WriteInteger(stat_allocation_allowed);
                buffer.WriteInteger(stat_reduction_allowed);
                buffer.WriteInteger(statCount);
          
           
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public static async void AddProfileNotes(string username, string characterNameVal, string characterWorldVal, string notes)
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

        internal static async void SendVerification(string username, string verificationKey)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SSubmitVerificationKey);
            buffer.WriteString(username);
            buffer.WriteString(verificationKey);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();

        }

        internal static async void SendRestorationRequest(string restorationEmail)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SSubmitRestorationRequest);
            buffer.WriteString(restorationEmail);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        internal static async void SendRestoration(string email, string password, string restorationKey)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SSubmitRestorationKey);
            buffer.WriteString(password);
            buffer.WriteString(restorationKey);
            buffer.WriteString(email);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        internal static async void SendOOCInfo(string username, string password, string OOC)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.SSendOOC);
            buffer.WriteString(username);
            buffer.WriteString(password);
            buffer.WriteString(OOC);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        /*
         * PACKET STRUCTURE
         * 
        public static void EXAMPLE_PACKET_FUNCTION()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.PACKETNAME_REFERENCED_IN_CLIENTPACKETS); 
            //buffer.WriteBool(true);
            //buffer.WriteString("Sup");
            //buffer.WriteInteger(1);
            await ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        */
    }
}
