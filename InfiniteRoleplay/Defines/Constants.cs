using Dalamud.Interface.Internal;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.GroupPoseModule;
using static System.Net.Mime.MediaTypeNames;

namespace InfiniteRoleplay
{
    internal class Constants
    {
        public enum StatusMessages
        {
            //Login
            LOGIN_BANNED = -1,
            LOGIN_UNVERIFIED = 0,
            LOGIN_VERIFIED = 1,
            LOGIN_WRONG_INFORMATION = 2,
            //Forgot Info
            FORGOT_REQUEST_RECEIVED = 3,
            FORGOT_REQUEST_INCORRECT = 4,
            //Registration
            REGISTRATION_DUPLICATE_EMAIL = 5,
            REGISTRATION_DUPLICATE_USERNAME = 6,
            //Password Change
            PASSCHANGE_PASSWORD_CHANGED = 7,
            PASSCHANGE_INCORRECT_RESTORATION_KEY = 8,
            //Verification
            VERIFICATION_INCORRECT_KEY = 9,
            VERIFICATION_KEY_VERIFIED = 10,
            //Gallery
            GALLERY_INCORRECT_IMAGE = 11,

        }
        public enum BioFieldTypes
        {
            name = 0,
            race = 1,
            gender = 2,
            age = 3,
            height = 4,
            weight = 5,
            afg = 6,
        }
        public static bool nameLoaded = false, raceLoaded = false, genderLoaded = false, ageLoaded = false, heightLoaded = false, weightLoaded = false, afgLoaded = false;
        public enum InputTypes
        {
            multiline = 1,
            single = 2,
        }
        public enum Alignments
        {
            LawfulGood = 0,
            NeutralGood = 1,
            ChaoticGood = 2,
            LawfulNeutral = 3,
            TrueNeutral = 4,
            ChaoticNeutral = 5,
            LawfulEvil = 6,
            NeutralEvil = 7,
            ChaoticEvil = 8,
            None = 9,
        }
        public enum ConnectionStatus
        {
            blocked = -1,
            canceled = 0,
            refused = 1,
            pending = 2,
            accepted = 3,
            removed = 4,
            sendPending = 5,
            receivedPending = 6,
        }
        public enum Personalities
        {
            Abrasive = 0,
            AbsentMinded = 1,
            Aggressive = 2,
            Artistic = 3,
            Cautious = 4,
            Charming = 5,
            Compassionate = 6,
            Daredevil = 7,
            Dishonest = 8,
            Dutiful = 9,
            Easygoing = 10,
            Eccentric = 11,
            Honest = 12,
            Knowledgable = 13,
            Optimistic = 14,
            Polite = 15,
            Relentless = 16,
            Resentful = 17,
            Reserved = 18,
            Romantic = 19,
            Spiritual = 20,
            Superior = 21,
            Tormented = 22,
            Tough = 23,
            Wild = 24,
            Worldly = 25,
            None = 26,
        }
        public enum BodyForms
        {
            Emaciated = 1,
            Thin = 2,
            Healthy = 3,
            Fit = 4,
            Stocky = 5,
            Husky = 6,
            Overwheight = 7,
            Obese = 8,
        }
        public enum CommonImageTypes
        {
            discordBtn = 1,
            kofiBtn = 2,
            blankPictureTab = 3,
            avatarHolder = 4,
            profileSection = 5,
            eventsSection = 6,
            systemsSection = 7,
            connectionsSection = 8,
            //profiles
            profileCreateProfile = 9,
            profileCreateNPC = 10,
            profileBookmarkProfile = 11,
            profileBookmarkNPC = 12,
            //events and venues
            eventEvents = 13,
            eventBookmarkEvent = 14,
            //systems
            systemsCombatSystem = 15,
            systemSheetSystem = 16,
            //connections
            //targets
            targetConnections = 17,
            targetBookmark = 18,
            targetGroupInvite = 19,
            targetViewProfile = 20,
            //gallery nsfw and triggers
            NSFW = 21,
            TRIGGER = 22,
            NSFWTRIGGER = 23,
            //Connection Button
            reconnect = 24,

        }
        public static IDalamudTextureWrap UICommonImage(Plugin plugin, CommonImageTypes imageType)
        {
            IDalamudTextureWrap commonImage = null;
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {

                if (imageType == CommonImageTypes.discordBtn) { commonImage = plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/disc_btn.png")));}
                if (imageType == CommonImageTypes.kofiBtn) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/kofi_btn.png")));}
                if (imageType == CommonImageTypes.blankPictureTab) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/galleries/picturetab.png")));}
                if (imageType == CommonImageTypes.NSFW) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/galleries/nsfw.png")));}
                if (imageType == CommonImageTypes.NSFWTRIGGER) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/galleries/nsfw_trigger.png")));}
                if (imageType == CommonImageTypes.TRIGGER) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/galleries/trigger.png")));}
                if (imageType == CommonImageTypes.avatarHolder) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/avatar_holder.png")));}
                if (imageType == CommonImageTypes.profileSection) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/section_profiles.png")));}
                if (imageType == CommonImageTypes.systemsSection) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/section_systems.png")));}
                if (imageType == CommonImageTypes.eventsSection) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/section_events.png")));}
                if (imageType == CommonImageTypes.connectionsSection) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/section_connections.png")));}
                //profiles
                if (imageType == CommonImageTypes.profileCreateProfile) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/profile_create.png")));}
                if (imageType == CommonImageTypes.profileCreateNPC) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/npc_create.png")));}
                if (imageType == CommonImageTypes.profileBookmarkProfile) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/profile_bookmarks.png")));}
                if (imageType == CommonImageTypes.profileBookmarkNPC) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/npc_bookmarks.png")));}
                //target images
                if (imageType == CommonImageTypes.targetConnections) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/targets/assign_connection.png")));}
                if (imageType == CommonImageTypes.targetBookmark) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/targets/bookmark.png")));}
                if (imageType == CommonImageTypes.targetGroupInvite) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/targets/group_invite.png")));}
                if (imageType == CommonImageTypes.targetViewProfile) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/targets/profile_view.png")));}
                if (imageType == CommonImageTypes.reconnect) { commonImage =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/connect.png")));}
            }

            return commonImage;
            
        }

        public static string AlignmentName(int alignment)
        {
            string alignmentName = string.Empty;
            if (alignment == (int)Alignments.LawfulGood) { alignmentName = "Lawful Good"; };
            if (alignment == (int)Alignments.NeutralGood) { alignmentName = "Neutral Good"; };
            if (alignment == (int)Alignments.ChaoticGood) { alignmentName = "Chaotic Good"; };
            if (alignment == (int)Alignments.LawfulNeutral) { alignmentName = "Lawful Neutral"; };
            if (alignment == (int)Alignments.TrueNeutral) { alignmentName = "True Neutral"; };
            if (alignment == (int)Alignments.ChaoticNeutral) { alignmentName = "Chaotic Neutral"; };
            if (alignment == (int)Alignments.LawfulEvil) { alignmentName = "Lawful Evil"; };
            if (alignment == (int)Alignments.NeutralEvil) { alignmentName = "Neutral Evil"; };
            if (alignment == (int)Alignments.ChaoticEvil) { alignmentName = "Chaotic Evil"; };
            return alignmentName;
        }
        public static string PersonalityNames(int personality)
        {
            string personalityName = string.Empty;
            if (personality == (int)Personalities.Abrasive) { personalityName = "Abrasive"; };//Abrasive
            if (personality == (int)Personalities.AbsentMinded) { personalityName = "Absent-Minded"; };//Absent-Minded
            if (personality == (int)Personalities.Aggressive) { personalityName = "Aggressive"; };//Agressive
            if (personality == (int)Personalities.Artistic) { personalityName = "Artistic"; };//Artistic
            if (personality == (int)Personalities.Cautious) { personalityName = "Cautious"; };//Cautious
            if (personality == (int)Personalities.Charming) { personalityName = "Charming"; };//Charming
            if (personality == (int)Personalities.Compassionate) { personalityName = "Compassionate"; };//Compassionate
            if (personality == (int)Personalities.Daredevil) { personalityName = "Daredevil"; };//Daredevil
            if (personality == (int)Personalities.Dishonest) { personalityName = "Dishonest"; };//Dishonest
            if (personality == (int)Personalities.Dutiful) { personalityName = "Dutiful"; };//Dutiful
            if (personality == (int)Personalities.Easygoing) { personalityName = "Easygoing"; };//Easygoing
            if (personality == (int)Personalities.Eccentric) { personalityName = "Eccentric"; };//Eccentric
            if (personality == (int)Personalities.Honest) { personalityName = "Honest"; };//Honest
            if (personality == (int)Personalities.Knowledgable) { personalityName = "Knowledgable"; };//Knowledgable
            if (personality == (int)Personalities.Optimistic) { personalityName = "Optimistic"; };//Optimistic
            if (personality == (int)Personalities.Polite) { personalityName = "Polite"; };//Polite
            if (personality == (int)Personalities.Relentless) { personalityName = "Relentless"; };//Relentless
            if (personality == (int)Personalities.Resentful) { personalityName = "Resentful"; };//Resentful
            if (personality == (int)Personalities.Reserved) { personalityName = "Reserved"; }; //Reserved
            if (personality == (int)Personalities.Romantic) { personalityName = "Romantic"; }; //Romantic
            if (personality == (int)Personalities.Spiritual) { personalityName = "Spiritual"; };//Spiritual
            if (personality == (int)Personalities.Superior) { personalityName = "Superior"; };//Superior
            if (personality == (int)Personalities.Tormented) { personalityName = "Tormented"; };//Tormentex
            if (personality == (int)Personalities.Tough) { personalityName = "Tough"; };//Tough
            if (personality == (int)Personalities.Wild) { personalityName = "Wild"; };//Wild
            if (personality == (int)Personalities.Worldly) { personalityName = "Worldly"; };//Worldly
            if (personality == (int)Personalities.None) { personalityName = "None"; };//None
            return personalityName;
        }

        public static string BodyFormNames(int form)
        {
            string formName = string.Empty;
            if (form == (int)BodyForms.Emaciated) { formName = "Emaciated"; };
            if (form == (int)BodyForms.Thin) { formName = "Thin"; };
            if (form == (int)BodyForms.Healthy) { formName = "Healthy"; };
            if (form == (int)BodyForms.Fit) { formName = "Fit"; };
            if (form == (int)BodyForms.Stocky) { formName = "Stocky"; };
            if (form == (int)BodyForms.Husky) { formName = "Husky"; };
            if (form == (int)BodyForms.Overwheight) { formName = "Overweight"; };
            if (form == (int)BodyForms.Obese) { formName = "Obese"; };
            return formName;
        }


        public static SortedList<int, Tuple<string, string>> BodyFormValues()
        {
            SortedList<int, Tuple<string, string>> bodyFormValues = new SortedList<int, Tuple<string, string>>
            {
                { (int) BodyForms.Emaciated, Tuple.Create(BodyFormNames((int)BodyForms.Emaciated), "You are underfed and starving. Unhealthy")},
                { (int) BodyForms.Thin, Tuple.Create(BodyFormNames((int) BodyForms.Thin), "You are underweight, usually from high metabolism") },
                { (int) BodyForms.Healthy, Tuple.Create(BodyFormNames((int) BodyForms.Healthy), "Your body is not muscular, but not overweight. You take good care of yourself, without too much excersise or weight watching.") },
                { (int) BodyForms.Fit, Tuple.Create(BodyFormNames((int) BodyForms.Fit), "Your body is lean and muscular, you take good care of yourself and excercise regularly.") },
                { (int) BodyForms.Stocky, Tuple.Create(BodyFormNames((int) BodyForms.Stocky), "Your body is large but muscular, like a body builder.") },
                { (int) BodyForms.Husky, Tuple.Create(BodyFormNames((int) BodyForms.Husky), "Your body is heavy set, but not to an unhealthy extent.") },
                { (int) BodyForms.Overwheight, Tuple.Create(BodyFormNames((int) BodyForms.Overwheight), "You are overweight, you do little excercise and do not watch much of what you eat.") },
                { (int) BodyForms.Obese, Tuple.Create(BodyFormNames((int) BodyForms.Obese), "Your body is large and you have little muscle. Most of your body consists of fat. you do not excercise or watch your diet at all.") },
            };
            return bodyFormValues;
        }

        public static IDalamudTextureWrap AlignementIcon(Plugin plugin, int id)
        {
            IDalamudTextureWrap alignmentIcon = null;
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {
                if (id == (int)Alignments.LawfulGood) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/lawful_good.png")));}
                if (id == (int)Alignments.NeutralGood) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/neutral_good.png")));}
                if (id == (int)Alignments.ChaoticGood) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/chaotic_good.png")));}
                if (id == (int)Alignments.LawfulNeutral) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/lawful_neutral.png")));}
                if (id == (int)Alignments.TrueNeutral) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/true_neutral.png")));}
                if (id == (int)Alignments.ChaoticNeutral) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/chaotic_neutral.png")));}
                if (id == (int)Alignments.LawfulEvil) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/lawful_evil.png")));}
                if (id == (int)Alignments.NeutralEvil) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/neutral_evil.png")));}
                if (id == (int)Alignments.ChaoticEvil) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/alignments/chaotic_evil.png")));}
                if (id == (int)Alignments.None) { alignmentIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/none.png")));}
            }

            return alignmentIcon;
        }
        public static IDalamudTextureWrap PersonalityIcon(Plugin plugin, int id)
        {
            IDalamudTextureWrap personalityIcon = null;
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {
                if (id == (int)Personalities.Abrasive) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/abrasive.png")));}
                if (id == (int)Personalities.AbsentMinded) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/absentminded.png")));}
                if (id == (int)Personalities.Artistic) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/artistic.png")));}
                if (id == (int)Personalities.Cautious) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/cuatious.png")));}
                if (id == (int)Personalities.Charming) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/charming.png")));}
                if (id == (int)Personalities.Compassionate) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/compassionate.png")));}
                if (id == (int)Personalities.Daredevil) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/daredevil.png")));}
                if (id == (int)Personalities.Dishonest) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/dishonest.png")));}
                if (id == (int)Personalities.Dutiful) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/dutiful.png")));}
                if (id == (int)Personalities.Easygoing) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/easygoing.png")));}
                if (id == (int)Personalities.Eccentric) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/eccentric.png")));}
                if (id == (int)Personalities.Honest) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/honest.png")));}
                if (id == (int)Personalities.Knowledgable) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/knowledgable.png")));}
                if (id == (int)Personalities.Optimistic) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/optimistic.png")));}
                if (id == (int)Personalities.Polite) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/polite.png")));}
                if (id == (int)Personalities.Relentless) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/relentless.png")));}
                if (id == (int)Personalities.Resentful) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/resentful.png")));}
                if (id == (int)Personalities.Reserved) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/reserved.png")));}
                if (id == (int)Personalities.Romantic) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/romantic.png")));}
                if (id == (int)Personalities.Spiritual) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/spiritual.png")));}
                if (id == (int)Personalities.Superior) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/superior.png")));}
                if (id == (int)Personalities.Tormented) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/tormented.png")));}
                if (id == (int)Personalities.Tough) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/tough.png")));}
                if (id == (int)Personalities.Wild) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/wild.png")));}
                if (id == (int)Personalities.Worldly) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/worldly.png")));}
                if (id == (int)Personalities.None) { personalityIcon =plugin.TextureProvider.GetTextureFromFile(new FileInfo(Path.Combine(path, "UI/common/profiles/personalities/none.png")));}
            }
            return personalityIcon;
        }

        public static readonly (string, string, string, InputTypes)[] BioFieldVals =
        {

           // string[] nameFields = new string[] { "Name:   ", "##playername", $"Character Name (The name or nickname of the character you are currently playing as)", name};
            ("NAME:   ", "##playername",  $"Character Name (The name or nickname of the character you are currently playing as)",  InputTypes.single),
            ("RACE:    ", "##race", $"The IC Race of your character", InputTypes.single),
            ("GENDER: ", "##gender", $"The IC gender of your character", InputTypes.single),
            ("AGE:   ", "##age", $"If not 18+, do not put nsfw images in your gallery", InputTypes.single),
            ("HEIGHT:", "##height", $"You're OC's IC Height", InputTypes.single),
            ("WEIGHT:", "##weight", $"You're OC's IC Weight", InputTypes.single),
            ("AT FIRST GLANCE:", "##afg", $"What people see when they first glance at your character", InputTypes.multiline),

        };


        public static readonly (string, string)[] PersonalityValues =
        {
            
           
                //Abrasive
                (PersonalityNames((int)Personalities.Abrasive), "• I am usually hostile or offensive in social interactions \n " +
                                                                                                           "• I am very standoffish and do not like being approached. \n" +
                                                                                                           "• I do not respond kindly to advice and especially orders."),
                //Absent-Minded
                (PersonalityNames((int)Personalities.AbsentMinded), "• I’m easily distracted and always have a string of unfinished \n" +
                                                                                                                   "   tasks on my to-do list. \n" +
                                                                                                                   "• I’m pretty oblivious to my surroundings.\n" +
                                                                                                                   "• I’m always wandering off to look at inconsequential \n" +
                                                                                                                   "   things and get lost easily.\n" +
                                                                                                                   "• I flake out on promises a lot. I don’t mean to…I just forget!\n" +
                                                                                                                   "• If there’s a plan, I’ll forget it. If I don’t forget it… \n" +
                                                                                                                   "   I usually end up ignoring it."),
                //Agressive
                (PersonalityNames((int)Personalities.Aggressive),     "• I feel the most peaceful in the heat of battle as I vanquish my foes.\n" +
                                                                                                                   "• I’m always the first to run into battle.\n" +
                                                                                                                   "• I’ll start a fight over anything. I’ll even do it just because I’m bored!\n" +
                                                                                                                   "• Violence is my first response to any challenge.\n" +
                                                                                                                   "• I get unreasonably angry at the slightest insult.\n" +
                                                                                                                   "• I use language so foul that I can make sailors blush—and not afraid to give someone a tongue lashing."),
                //Artistic
                (PersonalityNames((int)Personalities.Artistic),  "• I’m always doodling on the margins of my spellbook.\n" +
                                                                                                            "• I paint all the fascinating flora and fauna I see in my travel journal.\n" +
                                                                                                            "• I can see the true beauty in anything…even a beholder!\n" +
                                                                                                            "• I love to improvise new songs, but not everyone appreciates the fact that I’m always humming under my breath."),
                //Cautious
                (PersonalityNames((int)Personalities.Cautious),  "• I’m slow to trust and often assume the worst of people, but once they win me over, I’m a friend for life.\n" +
                                                                                                            "• I never trust anyone other than myself and don’t intend to change that.\n" +
                                                                                                            "• I make sure to know my enemy before I act, lest I bite off more than I can chew.\n" +
                                                                                                            "• I’m the first one to panic (or advocate a speedy retreat) when a threat arises."),
                //Charming
                (PersonalityNames((int)Personalities.Charming),  "• I can (and do) sweet-talk just about anyone. Flattery gets me everywhere!\n" +
                                                                                                            "• I absolutely adore a well-crafted insult, even one that’s directed at me.\n" +
                                                                                                            "• I make witty jokes and quips at all the worst moments.\n" +
                                                                                                            "• I love gossip and know how to pry the juicy details out of anyone’s mouth."),
                //Compassionate
                (PersonalityNames((int)Personalities.Compassionate),  "• I can empathize and find common ground between even the most hostile enemies.\n" +
                                                                                                                      "• I fight for those who can’t fight for themselves.\n" +
                                                                                                                      "• I go out of my way to help people, even if it puts me in danger.\n" +
                                                                                                                      "• I can never say “no” when someone asks me for a favor.\n" +
                                                                                                                      "• I watch over my friends like a protective mother, whether they want me to or not."),
                //Daredevil
                (PersonalityNames((int)Personalities.Daredevil),  "• I like helping people…but I also like the rush I get from throwing myself into danger.\n" +
                                                                                                              "• I’ve always wanted to be an adventurer. Travel and constant danger sound fun!\n" +
                                                                                                              "• I’ll say “yes” to anything—it doesn’t matter what I’m asked to do.\n" +
                                                                                                              "• I'm a gambler. I can't resist the thrill of taking a risk for a possible payoff."),
                //Dishonest
                (PersonalityNames((int)Personalities.Dishonest),  "• I lie about almost everything, even when there's no reason to do it.\n" +
                                                                                                              "• I have a false identity and will tell any lie to protect it.\n" +
                                                                                                              "• I steal anything I see that might have some value.\n" +
                                                                                                              "• I love swindling people who are more powerful than me!\n" +
                                                                                                              "• I don’t consider it “stealing” when I need something more than the person who has it."),
                //Dutiful
                (PersonalityNames((int)Personalities.Dutiful),  "• I would lay down my life for my comrades in a heartbeat.\n" +
                                                                                                          "• I feel obligated to fulfill a mission given to me by my order, even though I don’t want to.\n" +
                                                                                                          "• I tend to judge people who forsake their duties harshly.\n" +
                                                                                                          "• My parent taught me a sense of duty, and I’ll always uphold it, even when the odds are against me."),
                //Easygoing
                (PersonalityNames((int)Personalities.Easygoing),  "• I’m always on the lookout for new friends—I love introducing myself to people wherever I go.\n" +
                                                                                                              "• I’m pretty likable, so I tend to assume everyone wants to be friends with me.\n" +
                                                                                                              "• I tend to let other people do all the planning. I prefer to go with the flow.\n" +
                                                                                                              "• I’m comfortable in any social situation, no matter how tense it gets."),
                //Eccentric
                (PersonalityNames((int)Personalities.Eccentric),  "• I always leave a calling card, no matter where I go.\n" +
                                                                                                              "• Sometimes I mutter about myself in the third person…even when others can hear me.\n" +
                                                                                                              "• I change my mood or my mind as quickly as the wind changes directions.\n" +
                                                                                                              "• I always dress in formal clothes, even when I’m slogging through a cave or exploring a dungeon.\n" +
                                                                                                              "• I can’t control my reaction when surprised or afraid. One time I burned down a building that had a spider in it!"),
                //Honest
                (PersonalityNames((int)Personalities.Honest),  "• I’m very earnest and unusually direct. Sometimes people are taken aback by my bluntness.\n" +
                                                                                                        "• It’s hard to conceal my emotions. I always wear my heart on my sleeve!\n" +
                                                                                                        "• I made a vow never to tell a lie, and I intend to keep it.\n" +
                                                                                                        "• I’m bad at keeping secrets. I always end up blurting out the truth!"),
                //Knowledgable
                (PersonalityNames((int)Personalities.Knowledgable),  "• I read every book I can get my hands on and travel with a dozen in my pack.\n" +
                                                                                                                    "• I love a good puzzle! Once I get wind of a mystery, I’ll stop at nothing to solve it.\n" +
                                                                                                                    "• I would risk life and limb (my own or someone else’s) to obtain new knowledge.\n" +
                                                                                                                    "• I don’t believe in “forbidden” knowledge. What matters is what you do with it, right?\n" +
                                                                                                                    "• I tend to assume I know more about a particular subject than anyone else around me.\n" +
                                                                                                                    "• I have a single obscure hobby and will eagerly discuss it in detail."),
                //Optimistic
                (PersonalityNames((int)Personalities.Optimistic),  "• Absolutely nothing can shake my sunny disposition!\n" +
                                                                                                                "• In a bad situation, I’m the one telling everyone to look on the bright side.\n" +
                                                                                                                "• I’m more likely to laugh or crack a joke than cry, which sometimes rubs people the wrong way.\n" +
                                                                                                                "• I encourage everyone to be the best version of themselves that they can be."),
                //Polite
                (PersonalityNames((int)Personalities.Polite),  "• I genuinely believe that manners are important, no matter my situation.\n" +
                                                                                                                "• My elegance and refinement are tools I use to avoid arousing suspicion from others.\n" +
                                                                                                                "• No one can fake a smile, a handshake, or an interested nod like me!\n" +
                                                                                                                "• I was raised to have the manners of a noble, and I can’t imagine a world where I don’t use them."),
                //Relentless
                (PersonalityNames((int)Personalities.Relentless),  "• I’m convinced I’ll find riches beyond my imagination if I keep looking for it.\n" +
                                                                                                                "• I fail often, but I’ll never, ever give up.\n" +
                                                                                                                "• I will stop at nothing to achieve my goals, even if I make a few enemies along the way.\n" +
                                                                                                                "• If someone questions my courage, I’ll never back down, no matter how dangerous the situation is.\n" +
                                                                                                                "• I’m going to recover something that was taken from me if it’s the last thing I do, and I have no time for distractions…or friends."),
                //Resentful
                (PersonalityNames((int)Personalities.Resentful),  "• I always remember an insult, no matter how inconsequential.\n" +
                                                                                                              "• I never show my anger—but I do plot my revenge.\n" +
                                                                                                              "• I get upset when I’m not the center of attention.\n" +
                                                                                                              "• I’m slow to forgive when I feel like someone has slighted me.\n" +
                                                                                                              "• I’ll never forget the crushing defeat I suffered at my enemy’s hands, and I’ll pay them back dearly for it."),
                //Reserved
                (PersonalityNames((int)Personalities.Reserved),  "• I speak very slowly and carefully like I’m choosing each word before I say it.\n" +
                                                                                                            "• I’m more likely to communicate with a grunt or hand gesture than with actual words.\n" +
                                                                                                            "• I’d rather stand back and observe people than get involved.\n" +
                                                                                                            "• I endure any injury or insult with quiet, steely discipline.\n" +
                                                                                                            "• I always wait for the other person to talk first. There’s no such thing as an awkward silence!"),
                //Romantic
                (PersonalityNames((int)Personalities.Romantic),  "• I'm a hopeless romantic. Wherever I go, I’m always looking for “the one.”\n" +
                                                                                                            "• I fall in love in the blink of an eye…and fall out of love just as quickly.\n" +
                                                                                                            "• I have a weakness for great beauty—from breathtaking landscapes to pretty faces.\n" +
                                                                                                            "• I got rejected by someone I’m convinced is the love of my life, and I hope to prove myself worthy of them through my daring adventures!"),
                //Spiritual
                (PersonalityNames((int)Personalities.Spiritual),  "• I put too much trust in my religious institution and its hierarchy.\n" +
                                                                                                              "• I constantly quote (or misquote) sacred texts and proverbs.\n" +
                                                                                                              "• I idolize a particular hero of my faith and constantly revisit their deeds.\n" +
                                                                                                              "• I believe everything that happens to me is part of a greater divine plan.\n" +
                                                                                                              "• I keep holy symbols from every pantheon with me. Who knows which one I’ll need next?\n" +
                                                                                                              "• I see omens—both good and bad—everywhere. The gods are speaking to us, and we must listen."),
                //Superior
                (PersonalityNames((int)Personalities.Superior),  "• I never settle for anything less than perfection.\n" +
                                                                                                            "• I never admit to any mistakes because I’m scared they’ll be used against me.\n" +
                                                                                                            "• I'm used to the very best in life, so I don’t appreciate the rustic adventuring life.\n" +
                                                                                                            "• I’d kill to get a noble title (and the respect that comes with it)."),
                //Tormented
                (PersonalityNames((int)Personalities.Tormented),  "• I have awful visions of the future, but I don’t know how to prevent them from happening.\n" +
                                                                                                              "• I’m plagued by bloodthirsty urges that won’t let up no matter what I do.\n" +
                                                                                                              "• I’m haunted by my past and wake at night frightened by horrors I can barely remember.\n" +
                                                                                                              "• I faced the worst a vampire could throw at me and survived. I’m fearless, and my resolve is unwavering."),
                //Tough
                (PersonalityNames((int)Personalities.Tough),  "• I feel the need to prove that I’m the toughest person in the room.\n" +
                                                                                                      "• I’m thick-skinned. It’s very hard to get a rise out of me!\n" +
                                                                                                      "• It’s hard for me to respect anyone unless they’re a proven warrior (like me).\n" +
                                                                                                      "• Anyone who wants to earn my trust has to spar with me first.\n" +
                                                                                                      "• I have an iron stomach. I’ve never entered a drinking contest that I haven’t won!"),
                //Wild
                (PersonalityNames((int)Personalities.Wild),  "• I prefer animals to people by a long shot.\n" +
                                                                                                    "• I’m always learning how to be among others—when to stay quiet and when to crack a joke.\n" +
                                                                                                    "• My personal hygiene is nonexistent, and so are my manners.\n" +
                                                                                                    "• I’m a forest-dweller who grew up in a tent in the woods, so I’m totally ignorant of city life.\n" +
                                                                                                    "• I was actually raised by wolves (or some other wild animal)."),
                //Worldly
                (PersonalityNames((int)Personalities.Worldly),  "• I'm tolerant of people different from me, and I love exploring other cultures.\n" +
                                                                                                          "• I love to tell stories of my travels to faraway lands…even if I tend to embellish a little!\n" +
                                                                                                          "• I’m filled with glee at the idea of seeing things most people don’t. The more unsettling, the better.\n" +
                                                                                                          "• I’m desperately trying to escape my past and never stay in one place—so I’ve been everywhere."),


                //Worldly
                (PersonalityNames((int)Personalities.None),  "• Unspecified."),
        };

        public static readonly (string, string)[] AlignmentVals =
        {
            ("Lawful Good",     "These characters always do the right thing as expected by society.\n" +
                                "They always follow the rules, tell the truth and help people out.\n" +
                                "They like order, trust and believe in people with social authority, and they aim to be an upstanding citizen.\n"),

            ("Neutral Good",    "These characters do their best to help others\n" +
                                "but they do it because they want to, not because they have\n" +
                                "been told to by a person in authority or by society’s laws.\n" +
                                "A Neutral Good person will break the rules if they are doing it\n" +
                                "for good reasons and they will feel confident\n" +
                                "and justified in their actions."),

            ("Chaotic Good",    "Chaotic Good characters do what their conscience tells\n" +
                                "them to for the greater good. They do not care about following society’s rules,\n" +
                                "they care about doing what’s right.\n" +
                                "A Chaotic Good character will speak up for and help, those who are being needlessly\n" +
                                "held back because of arbitrary rules and laws. They do not like seeing people\n" +
                                "being told what to do for nonsensical reasons.\n"),

            ("Lawful Neutral",  "A Lawful Neutral character behaves in a way that matches\n" +
                                "the organization, authority or tradition they follow.\n" +
                                "They live by this code and uphold it above all else, taking actions\n" +
                                "that are sometimes considered Good and sometimes considered Evil by others.\n" +
                                "The Lawful Neutral character does not care about what others think of\n" +
                                "their actions, they only care about their actions being correct according\n" +
                                "to their code.But they do not preach their code to others and try to convert them. \n"),

            ("True Neutral",    "True Neutral characters don’t like to take sides.\n" +
                                "They are pragmatic rather than emotional in their actions,\n" +
                                "choosing the response which makes the most sense for them in each situation.\n " +
                                "Neutral characters don’t believe in upholding the rules and laws of society, but nor\n" +
                                "do they feel the need to rebel against them. There will be times when a Neutral character\n" +
                                "has to make a choice between siding with Good or Evil, perhaps casting the deciding vote\n" +
                                "in a party. They will make a choice in these situations, usually siding with whichever causes\n" +
                                "them the least hassle, or they stand to gain the most from."),

            ("Chaotic Neutral", "Chaotic Neutral characters are free spirits.\n" +
                                "They do what they want but don’t seek to disrupt the usual norms and laws of society.\n" +
                                "These individuals don’t like being told what to do, following traditions,\n" +
                                "or being controlled. That said, they will not work to change these restrictions,\n" +
                                "instead, they will just try to avoid them in the first place.\n" +
                                "Their need to be free is the most important thing.\n"),

            ("Lawful Evil",     "Lawful Evil characters operate within a strict code of laws and traditions.\n" +
                                "Upholding these values and living by these is more important than anything,\n" +
                                "even the lives of others. They may not consider themselves to be Evil,\n" +
                                "they may believe what they are doing is right.\n" +
                                "These characters enforce their system of control through force.\n" +
                                "Anyone who doesn’t follow their code or acts out of line will face consequences.\n" +
                                "Lawful Evil characters feel no guilt or remorse for causing harm to others in this way."),

            ("Neutral Evil",    "Neutral Evil characters are selfish. Their actions are driven by their own wants\n" +
                                "whether that’s power, greed, attention, or something else.\n" +
                                "They will follow laws if they happen to align with their ambitions, but they will not\n" +
                                "hesitate to break them if they don’t.They don’t believe that following laws\n" +
                                "and traditions makes anyone a better person.\n" +
                                "Instead, they use other people’s beliefs in codes and loyalty against them, using it\n" +
                                "as a tool to influence their behaviour. "),

            ("Chaotic Evil",    "Chaotic Evil characters care only for themselves with a complete disregard\n" +
                                "for all law and order and for the welfare and freedom of others.\n" +
                                "They harm others out of anger or just for fun.\n" +
                                "Characters aligned with Chaotic Evil usually operate alone\n" +
                                "because they do not work well with others."),

            ("None",            "Not Specified"),
        };
        public static readonly (string, string)[] ConnectionListingVals =
                {
            ("Connected",     "Hidden profiles you have access to."),

            ("Sent",    "Sent requests to view hidden profiles"),

            ("Received",    "Received requests to see your profile"),

            ("Blocked",  "Blocked requests to see your current profile"),
        };





    }

}

