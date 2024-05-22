using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using InfiniteRoleplay.Windows;
using InfiniteRoleplay;
using Dalamud.Plugin.Services;

namespace Networking
{
    public class ClientTCP
    {
        public static bool Connected;
        public static TcpClient clientSocket;
        private static NetworkStream myStream;
        private static byte[] recBuffer;
        private static string server = "185.33.84.184";
        private static int port = 25565;
        private static int bufferSize = 8192;
        public static Plugin plugin;

        public static void StartReceiving()
        {
            Task.Run(() => ReceiveData());
        }

        private static void ReceiveData()
        {
            try
            {
                while (Connected)
                {
                    int length = myStream.Read(recBuffer, 0, recBuffer.Length);
                    if (length <= 0) break;

                    var newBytes = new byte[length];
                    Array.Copy(recBuffer, newBytes, length);
                    ClientHandleData.HandleData(newBytes);
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error receiving data: " + ex.ToString(), LogLevels.LogError);
                Connected = false;
            }
        }
        public static string GetConnectionStatus(TcpClient _tcpClient)
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                {
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            return "Connected";
                        }
                        return "Connected";
                    }
                    return "Connected";
                }
                return "Disconnected";
            }
            catch (Exception ex)
            {
                return "Disconnected";
            }
        }
        public static bool IsConnectedToServer(TcpClient _tcpClient)
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                {
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            MainPanel.serverStatus = "Not connected to server";
                            MainPanel.serverStatusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                            return false;
                        }
                        MainPanel.serverStatus = "Connected to server";
                        MainPanel.serverStatusColor = new System.Numerics.Vector4(0, 255, 0, 255);
                        return true;
                    }
                    MainPanel.serverStatus = "Connected to server";
                    MainPanel.serverStatusColor = new System.Numerics.Vector4(0, 255, 0, 255);
                    return true;
                }
                MainPanel.serverStatus = "Not connected to server";
                MainPanel.serverStatusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                return false;
            }
            catch (Exception ex)
            {
                MainPanel.serverStatus = "Not connected to server";
                MainPanel.serverStatusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                DataSender.PrintMessage("Error checking server connection: " + ex.ToString(), LogLevels.LogWarning);
                return false;
            }
        }

        public static void CheckStatus(Plugin plugin, IDtrBar DtBar)
        {
            try
            {
                if (!IsConnectedToServer(clientSocket))
                {
                    ConnectToServer();
                }
                if (DtrBarHelper.BarAdded == false)
                {                   
                    DtrBarHelper.AddIconToDtrBar(plugin, DtBar);
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error checking status: " + ex.ToString(), LogLevels.LogWarning);
            }
        }

        public static void ConnectToServer()
        {
            try
            {
                if (ClientHandleData.packets.Count < 30)
                {
                    ClientHandleData.InitializePackets(true);
                }
                EstablishConnection();
                myStream = clientSocket.GetStream();
                recBuffer = new byte[bufferSize];

                Connected = true;
                StartReceiving();
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not connect to server: " + ex.ToString(), LogLevels.LogError);
                Disconnect();
            }
        }

        public static void EstablishConnection()
        {
            try
            {
                clientSocket = new TcpClient();
                clientSocket.ReceiveBufferSize = bufferSize;
                clientSocket.SendBufferSize = bufferSize;
                clientSocket.Connect(server, port);
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not establish connection: " + ex.ToString(), LogLevels.LogError);
                clientSocket.Dispose();
            }
        }

        public static void Disconnect()
        {
            Connected = false;
            if (myStream != null)
            {
                myStream.Close();
                myStream.Dispose();
            }
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket.Dispose();
            }
        }

        public static void SendData(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
                buffer.WriteBytes(data);
                myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error sending data: " + ex.ToString(), LogLevels.LogError);
            }
        }
    }
}
