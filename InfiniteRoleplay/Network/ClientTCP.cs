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
        private static readonly string server = "185.33.84.184";
        private static readonly int port = 25565;
        private static readonly int bufferSize = 8192;
        public static Plugin plugin;

        public static void StartReceiving()
        {
            Task.Run(ReceiveDataAsync);
        }

        private static async Task ReceiveDataAsync()
        {
            try
            {
                while (Connected)
                {
                    int length = await myStream.ReadAsync(recBuffer, 0, recBuffer.Length);
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

        public static async Task<string> GetConnectionStatusAsync(TcpClient _tcpClient)
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                {
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (await _tcpClient.Client.ReceiveAsync(new ArraySegment<byte>(buff), SocketFlags.Peek) == 0)
                        {
                            return "Disconnected";
                        }
                        return "Connected";
                    }
                    return "Connected";
                }
                return "Disconnected";
            }
            catch
            {
                return "Disconnected";
            }
        }

        public static async Task<bool> IsConnectedToServerAsync(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (await tcpClient.Client.ReceiveAsync(new ArraySegment<byte>(buff), SocketFlags.Peek) == 0)
                        {
                            UpdateServerStatus("Not connected to server", new System.Numerics.Vector4(255, 0, 0, 255));
                            return false;
                        }
                        UpdateServerStatus("Connected to server", new System.Numerics.Vector4(0, 255, 0, 255));
                        return true;
                    }
                    UpdateServerStatus("Connected to server", new System.Numerics.Vector4(0, 255, 0, 255));
                    return true;
                }
                UpdateServerStatus("Not connected to server", new System.Numerics.Vector4(255, 0, 0, 255));
                return false;
            }
            catch (Exception ex)
            {
                UpdateServerStatus("Not connected to server", new System.Numerics.Vector4(255, 0, 0, 255));
                DataSender.PrintMessage("Error checking server connection: " + ex.ToString(), LogLevels.LogWarning);
                return false;
            }
        }

        private static void UpdateServerStatus(string status, System.Numerics.Vector4 color)
        {
            // Ensure thread safety if this method updates UI elements
            // Assuming MainPanel updates must be done on the main/UI thread
            Task.Run(() =>
            {
                MainPanel.serverStatus = status;
                MainPanel.serverStatusColor = color;
            });
        }

        public static async void CheckStatus()
        {
            try
            {
                bool connected = await IsConnectedToServerAsync(clientSocket);
                if (!connected)
                {
                    ConnectToServer();
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
                clientSocket?.Dispose();
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

        public static async Task SendDataAsync(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
                buffer.WriteBytes(data);
                await myStream.WriteAsync(buffer.ToArray(), 0, buffer.ToArray().Length);
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Error sending data: " + ex.ToString(), LogLevels.LogError);
            }
        }
    }
}
