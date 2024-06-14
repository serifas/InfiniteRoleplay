using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using InfiniteRoleplay.Windows;
using InfiniteRoleplay;
using Dalamud.Plugin.Services;
using System.Drawing;
using System.Numerics;

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
        public static Vector4 Green = new Vector4(0, 255, 0, 255);
        public static Vector4 Red = new Vector4(255, 0, 0, 255);
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
                plugin.logger.Error("Error receiving data: " + ex.ToString());
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
                            MainPanel.serverStatusColor = Red;
                            return "Disconnected";
                        }
                        MainPanel.serverStatusColor = Green;
                        return "Connected";
                    }
                    MainPanel.serverStatusColor = Green;
                    return "Connected";
                }
                MainPanel.serverStatusColor = Red;
                return "Disconnected";
            }
            catch
            {
                MainPanel.serverStatusColor = Red;
                return "Disconnected";
            }
        }
        public static async Task<bool> PingHostAsync(string host, int port, int timeout = 1000)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var connectTask = tcpClient.ConnectAsync(host, port);
                    var delayTask = Task.Delay(timeout);

                    var completedTask = await Task.WhenAny(connectTask, delayTask);

                    if (completedTask == connectTask)
                    {
                        return tcpClient.Connected;
                    }
                    else
                    {
                        return false; // Timed out
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                // PluginLog.LogError(ex, "Error pinging host");
                return false;
            }
        }
        public static bool IsConnected()
        {
            bool isConnected = Task.Run(async () => await IsConnectedToServerAsync(clientSocket)).GetAwaiter().GetResult();
            return isConnected;
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
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                plugin.logger.Error("Error checking server connection: " + ex.ToString());
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
                bool pinged = await PingHostAsync(server, port);
                if(pinged)
                {
                    bool connected = await IsConnectedToServerAsync(clientSocket);
                    if (!connected)
                    {
                        ConnectToServer();
                    }
                }
            }
            catch (Exception ex)
            {
                plugin.logger.Error("Error checking status: " + ex.ToString());
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
                plugin.logger.Error("Could not connect to server: " + ex.ToString());
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
                plugin.logger.Error("Could not establish connection: " + ex.ToString());
                clientSocket?.Dispose();
            }
        }

        public static void Disconnect()
        {
            Connected = false;
            myStream?.Close();
            myStream?.Dispose();
            clientSocket?.Close();
            clientSocket?.Dispose();
            MainPanel.serverStatus = "Disconnected";
            MainPanel.serverStatusColor = new System.Numerics.Vector4(255, 0, 0, 255);
        }
        public static void AttemptConnect()
        {
            try
            {
                CheckStatus();
            }
            catch (Exception ex)
            {
                plugin.logger.Error("Could not establish reconnect: " + ex);
            }
        }
        public static async Task SendDataAsync(byte[] data)
        {
            try
            {
                using (var buffer = new ByteBuffer())
                {
                    buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
                    buffer.WriteBytes(data);
                    await myStream.WriteAsync(buffer.ToArray(), 0, buffer.ToArray().Length);
                }
            }
            catch (Exception ex)
            {
                plugin.logger.Error("Error sending data: " + ex.ToString());
            }
        }
    }
}
