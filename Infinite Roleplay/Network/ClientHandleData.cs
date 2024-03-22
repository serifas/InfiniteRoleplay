using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking
{ 
    static class ClientHandleData
    {
        private static ByteBuffer playerBuffer;
        public static DataReceiver dr = new DataReceiver();
        public delegate void Packet(byte[] data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        //add our packets so we don't need to load them on the go.
        //should be added to start of client loading up
        public static void InitializePackets(bool start)
        {
            if(start == true)
            {
                packets.Add((int)ServerPackets.SWelcomeMessage, DataReceiver.HandleWelcomeMessage);
                packets.Add((int)ServerPackets.SRecLoginStatus, DataReceiver.StatusMessage);
                packets.Add((int)ServerPackets.SRecAccPermissions, DataReceiver.RecPermissions);
                packets.Add((int)ServerPackets.SRecProfileBio, DataReceiver.ReceiveProfileBio);
                packets.Add((int)ServerPackets.SNoProfileBio, DataReceiver.NoProfileBio);
                packets.Add((int)ServerPackets.SNoProfile, DataReceiver.NoProfile);
                packets.Add((int)ServerPackets.SSendProfile, DataReceiver.ReceiveProfile);
                packets.Add((int)ServerPackets.SRecExistingProfile, DataReceiver.ExistingProfile);
                packets.Add((int)ServerPackets.SSendProfileHook, DataReceiver.ReceiveProfileHooks);
                packets.Add((int)ServerPackets.SSendNoProfileHooks, DataReceiver.NoProfileHooks);
                packets.Add((int)ServerPackets.SRecProfileStory, DataReceiver.ReceiveProfileStory);
                packets.Add((int)ServerPackets.SRecNoProfileStory, DataReceiver.NoProfileStory);
                packets.Add((int)ServerPackets.SSendNoProfileNotes, DataReceiver.NoProfileNotes);
                packets.Add((int)ServerPackets.SSendProfileNotes, DataReceiver.RecProfileNotes);
                packets.Add((int)ServerPackets.SRecNoProfileGallery, DataReceiver.ReceiveNoProfileGallery);
                packets.Add((int)ServerPackets.SRecBookmarks, DataReceiver.RecBookmarks);
                packets.Add((int)ServerPackets.CProfileReportedSuccessfully, DataReceiver.RecProfileReportedSuccessfully);
                packets.Add((int)ServerPackets.CProfileAlreadyReported, DataReceiver.RecProfileAlreadyReported);
                //target packets
                packets.Add((int)ServerPackets.SRecNoTargetProfile, DataReceiver.NoTargetProfile);
                packets.Add((int)ServerPackets.SRecTargetProfile, DataReceiver.ExistingTargetProfile);
                packets.Add((int)ServerPackets.SRecNoTargetBio, DataReceiver.NoTargetBio);
                packets.Add((int)ServerPackets.SRecTargetBio, DataReceiver.ReceiveTargetBio);
                packets.Add((int)ServerPackets.SRecNoTargetHooks, DataReceiver.NoTargetHooks);
                packets.Add((int)ServerPackets.SRecTargetHooks, DataReceiver.ReceiveTargetHooks);
                packets.Add((int)ServerPackets.SRecNoTargetStory, DataReceiver.NoTargetStory);
                packets.Add((int)ServerPackets.SRecTargetStory, DataReceiver.ReceiveTargetStory);
                packets.Add((int)ServerPackets.SRecProfileGallery, DataReceiver.ReceiveProfileGalleryImage);
                packets.Add((int)ServerPackets.SRecGalleryImageLoaded, DataReceiver.ImageLoaded);
                //packets.Add((int)ServerPackets.SRecImageDeletionStatus, DataReceiver.ReceiveImageDeletionStatus);
                packets.Add((int)ServerPackets.SRecNoTargetGallery, DataReceiver.NoTargetGallery);
                packets.Add((int)ServerPackets.SRecTargetGallery, DataReceiver.ReceiveTargetGalleryImage);
                packets.Add((int)ServerPackets.SSendNoAuthorization, DataReceiver.ReceiveNoAuthorization);
                packets.Add((int)ServerPackets.SSendVerificationMessage, DataReceiver.ReceiveVerificationMessage);
                packets.Add((int)ServerPackets.SSendPasswordModificationForm, DataReceiver.ReceivePasswordModificationForm);
                packets.Add((int)ServerPackets.SSendNoOOCInfo, DataReceiver.ReceiveNoOOCInfo);
                packets.Add((int)ServerPackets.SSendOOC, DataReceiver.ReceiveProfileOOC);
                packets.Add((int)ServerPackets.SSendTargetOOC, DataReceiver.ReceiveTargetOOCInfo);
                packets.Add((int)ServerPackets.SSendNoTargetOOCInfo, DataReceiver.ReceiveNoTargetOOCInfo);
            }
            else
            {
                packets.Clear();
            }
            //simple message back from server, simply for verification that the user is connected
        }

        //LITERALLY NOT COMMENTING THIS, AS IT WASNT EVEN EXPLAINED TO ME
        public static void HandleData(byte[] data)
        {
            var buffer = (byte[])data.Clone();
            var pLength = 0;

            if (playerBuffer == null)
            {
                playerBuffer = new ByteBuffer();
            }
            playerBuffer.WriteBytes(buffer);
            if (playerBuffer.Count() == 0)
            {
                playerBuffer.Clear();
                return;
            }
            if (playerBuffer.Length() > 4)
            {
                pLength = playerBuffer.ReadInt(false);
                if (pLength <= 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }
            while (pLength > 0 & pLength <= playerBuffer.Length() - 4)
            {
                if (pLength <= playerBuffer.Length() - 4)
                {
                    playerBuffer.ReadInt();
                    data = playerBuffer.ReadBytes(pLength);
                    HandleDataPackets(data);

                }

                pLength = 0;
                if (playerBuffer.Length() > 4)
                {
                    pLength = playerBuffer.ReadInt(false);
                    if (pLength <= 0)
                    {
                        playerBuffer.Clear();
                        return;
                    }
                }
            }
            if (pLength <= 1)
            {
                playerBuffer.Clear();
            }
        }
        private static void HandleDataPackets(byte[] data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            var packetID = buffer.ReadInt();
            buffer.Dispose();
            if (packets.TryGetValue(packetID, out var packet))
            {
                packet.Invoke(data);
            }
        }
    }
}
