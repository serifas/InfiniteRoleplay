using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using InfiniteRoleplay.Windows.Functions;
using InfiniteRoleplay.Windows;
using InfiniteRoleplay;

namespace Networking
{
    public class ClientTCP
    {
        public static bool loadCallback;
        public static bool Connected;
        public static TcpClient clientSocket;
        private static NetworkStream myStream;
        private static byte[] recBuffer;
        private static string server = "185.33.84.184";
        private static DataSender dataSender;
        private static int port = 25565;
        public static Plugin plugin;
        public static int CheckCounter = 5;

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
                            LoginWindow.status = "Could not connect to server";
                            LoginWindow.statusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                            return false;
                        }
                        else
                        {
                            LoginWindow.status = "Connected to server";
                            LoginWindow.statusColor = new System.Numerics.Vector4(0, 255, 0, 255);
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    LoginWindow.status = "Could not connect to server";
                    LoginWindow.statusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                    return false;
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not check IsConnectedToServer " + ex.Message, LogLevels.LogWarning);
                return false;
            }
        }

        public static Task CheckStatus()
        {
            try
            {
                DataSender.PrintMessage("Checking connection status", LogLevels.Log);
                if (IsConnectedToServer(clientSocket))
                {
                    if (loadCallback)
                    {
                        ClientConnectionCallback();
                        loadCallback = false;
                    }
                    if (!plugin.uiLoaded)
                    {
                        plugin.LoadUI();
                    }
                }
                else
                {
                    ConnectToServer().Wait();
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not check status" + ex.ToString(), LogLevels.LogWarning);
            }

            return Task.FromResult(true);
        }

        public static Task ConnectToServer()
        {
            try
            {
                if (ClientHandleData.packets.Count < 30)
                {
                    ClientHandleData.InitializePackets(true);
                }
                InitializingNetworking(true).Wait();
                loadCallback = true;
                LoginWindow.status = "Connected to Server...";
                LoginWindow.statusColor = new System.Numerics.Vector4(0, 255, 0, 255);
                CheckStatus().Wait();
            }
            catch (Exception ex)
            {
                LoginWindow.status = "Could not connect to server.";
                LoginWindow.statusColor = new System.Numerics.Vector4(255, 0, 0, 255);
                DataSender.PrintMessage("Could not connect to server " + ex.ToString(), LogLevels.LogError);
            }

            return Task.FromResult(true);
        }

        public static Task InitializingNetworking(bool start)
        {
            try
            {
                if (start == true)
                {
                    EstablishConnection().Wait();
                }
                else
                {
                    if (clientSocket.Connected == true)
                    {
                        Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not Initialize Networking " + ex.ToString(), LogLevels.LogError);
            }

            return Task.FromResult(true);
        }

        public static Task EstablishConnection()
        {
            try
            {
                clientSocket = new TcpClient();
                clientSocket.ReceiveBufferSize = 65535;
                clientSocket.SendBufferSize = 65535;
                recBuffer = new byte[65535 * 2];
                clientSocket.Connect(server, port);
            }
            catch (Exception ex)
            {
                clientSocket.Dispose();
                DataSender.PrintMessage("Could not establish connection " + ex.ToString(), LogLevels.LogError);
            }

            return Task.FromResult(true);
        }

        public static void ClientConnectionCallback()
        {
            try
            {
                Connected = true;
                clientSocket.NoDelay = true;
                myStream = clientSocket.GetStream();
                myStream.BeginRead(recBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("ClientConnectionCallback failed " + ex.ToString(), LogLevels.LogError);
            }
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var length = myStream.EndRead(result);
                if (length <= 0)
                {
                    return;
                }
                var newBytes = new byte[length];
                Array.Copy(recBuffer, newBytes, length);
                ClientHandleData.HandleData(newBytes);
                myStream.BeginRead(recBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not receive callback " + ex.ToString(), LogLevels.LogError);
            }
        }

        public static Task SendData(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
                buffer.WriteBytes(data);
                myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not send data " + ex.ToString(), LogLevels.LogError);
            }

            return Task.FromResult(true);
        }

        public static void Disconnect()
        {
            try
            {
                Connected = false;
                myStream.Close();
                myStream.Dispose();
                clientSocket.Close();
                clientSocket.Dispose();
            }
            catch (Exception ex)
            {
                DataSender.PrintMessage("Could not disconnect " + ex.ToString(), LogLevels.LogError);
            }
        }
    }
}
