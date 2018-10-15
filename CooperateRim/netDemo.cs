using CooperateRim;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

public class NetDemo
{
    public static Action<string> log = u => { new Action(Console.WriteLine).BeginInvoke(k => { Console.WriteLine(u); }, u); };
    public static CSteamID m_RemoteSteamId;
    static Callback<P2PSessionRequest_t> a1;
    static Callback<P2PSessionConnectFail_t> a2;
    static Callback<SocketStatusCallback_t> a3;
    public static int mClientCount;
    
    static Dictionary<int, FrameData> datalist = new Dictionary<int, FrameData>();



    public enum MessageType
    {
        Connection,
        StatePush,
        StateRequest,
        StateReturn,
        StateDeny,
        Junk
    }

    [Serializable]
    public class FrameData
    {
        public int sourceID;
        public int frameID;
        public byte[][] frameData = new byte[mClientCount][];
        public MessageType type;

        public string GetStringKey()
        {
            return sourceID + " :: " + frameID;
        }
    }

    public static FrameData GetFrameData(int frameID)
    {
        return datalist[frameID];
    }

    public static void WaitForConnection(CSteamID id, int clientCount)
    {
        mClientCount = clientCount;
        NetDemo.SendTo(-1, -1, NetDemo.m_RemoteSteamId, null, MessageType.Connection, 0);

        P2PSessionState_t st;
        SteamNetworking.GetP2PSessionState(id, out st);

        for (; 0 == st.m_bConnectionActive;)
        {
            System.Threading.Thread.Sleep(50);
            SteamAPI.RunCallbacks();
            SteamNetworking.GetP2PSessionState(id, out st);
        }
    }

    public static bool HasAllDataForFrame(int frameID)
    {
        return datalist.ContainsKey(frameID) && datalist[frameID].frameData.All(u => u != null);
    }

    public static void TryGetPackets(bool isHost, Action<FrameData> dataRecv = null)
    {
        uint packetSize;

        Steamworks.SteamAPI.RunCallbacks();
        for (; SteamNetworking.IsP2PPacketAvailable(out packetSize, isHost ? 0 : SyncTickData.cliendID + 1);)
        {
            byte[] buffer = new byte[packetSize];
            uint msgSize;
            CSteamID userID;

            if (SteamNetworking.ReadP2PPacket(buffer, packetSize, out msgSize, out userID, isHost ? 0 : SyncTickData.cliendID + 1))
            {
                //log("data from user " + userID + " received, " + msgSize);

                MemoryStream ms = new MemoryStream(buffer);
                BinaryFormatter bf = new BinaryFormatter();
                FrameData ud = (FrameData)bf.Deserialize(ms);

                switch (ud.type)
                {
                    case MessageType.StatePush:
                        if (isHost)
                        {
                            if (!datalist.ContainsKey(ud.frameID))
                            {
                                log("allocating frame : " + ud.GetStringKey());
                                datalist.Add(ud.frameID, new FrameData());
                            }

                            if (datalist[ud.frameID].frameData[ud.sourceID] == null)
                            {
                                log("server has data for frame " + ud.frameID + " and client " + ud.sourceID);
                            }

                            datalist[ud.frameID].frameData[ud.sourceID] = ud.frameData[0];
                        }
                        break;
                    case MessageType.StateRequest:
                        {
                            if (isHost)
                            {
                                if (HasAllDataForFrame(ud.frameID))
                                {
                                    SendTo(ud.frameID, ud.sourceID, userID, datalist[ud.frameID].frameData, MessageType.StateReturn, ud.sourceID + 1);
                                }
                                else
                                {
                                    SendTo(ud.frameID, ud.sourceID, userID, null, MessageType.StateDeny, ud.sourceID + 1);
                                }
                            }
                        }
                        break;
                    case MessageType.Connection:
                        if (isHost)
                        {
                            log("new incoming client : " + userID);
                        }
                        break;
                    case MessageType.StateDeny:
                        if (!isHost)
                        {
                            log("frame request denied :" + ud.frameID);
                            if (lastPushedDatalist.ContainsKey(ud.sourceID))
                            {
                                PushStateToDirectory(ud.sourceID, ud.frameID, lastPushedDatalist[ud.sourceID], 0);
                            }
                        }
                        break;
                    case MessageType.StateReturn:
                        if (!isHost)
                        {
                            log("frame data received");
                            dataRecv?.Invoke(ud);
                        }
                        break;
                }
            }
            else
            {
                log("ReadP2PPacket failed");
            }
        }
    }

    public static void setupCallbacks()
    {
        bool steamApiInit = SteamAPI.Init();
        m_RemoteSteamId = SteamUser.GetSteamID();

        if (steamApiInit)
        {
            log("Packet relaying setup : " + SteamNetworking.AllowP2PPacketRelay(true));

            a1 = Callback<P2PSessionRequest_t>.Create(u =>
            {
                log("Session request : " + u.m_steamIDRemote);
                bool res = SteamNetworking.AcceptP2PSessionWithUser(u.m_steamIDRemote);
                log("Session accept result : " + res);
            });

            a2 = Callback<P2PSessionConnectFail_t>.Create(u =>
            {
                log("Session connect fail : " + u.m_steamIDRemote);
                bool res = SteamNetworking.AcceptP2PSessionWithUser(u.m_steamIDRemote);
            });

            a3 = Callback<SocketStatusCallback_t>.Create(pCallback =>
            {
                log("[" + SocketStatusCallback_t.k_iCallback + " - SocketStatusCallback] - " + pCallback.m_hSocket + " -- " + pCallback.m_hListenSocket + " -- " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eSNetSocketState);
            });
        }
    }

    public static void SendStateRequest(int frameID, int sourceID)
    {
        SendTo(frameID, sourceID, m_RemoteSteamId, null, MessageType.StateRequest, 0);
    }

    static Dictionary<int, byte[]> lastPushedDatalist = new Dictionary<int, byte[]>();

    public static void PushStateToDirectory(int sourceID, int tickID, byte[] data, int channelID)
    {
        if (!lastPushedDatalist.ContainsKey(sourceID))
        {
            lastPushedDatalist.Add(sourceID, null);
        }

        lastPushedDatalist[sourceID] = data;
        SendTo(tickID, sourceID, m_RemoteSteamId, new byte[][] { data }, MessageType.StatePush, 0);
    }

    public static void SendTo(int frameID, int sourceID, CSteamID remoteSteamID, byte[][] data, MessageType type, int channelID)
    {
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            BinaryFormatter b = new BinaryFormatter();
            {
                b.Serialize(ms, new FrameData() { frameData = data, frameID = frameID, sourceID = sourceID, type = type });
            }

            ms.Flush();
            byte[] bytes = ms.GetBuffer();

            bool ret = SteamNetworking.SendP2PPacket(remoteSteamID, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable, channelID);
            //log(type +  " :: SteamNetworking.SendP2PPacket(" + remoteSteamID + ", " + bytes + ", " + (uint)bytes.Length + ", " + EP2PSend.k_EP2PSendReliable + ") : " + ret);
        }
    }
}
