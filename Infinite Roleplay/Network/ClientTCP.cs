using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.CompilerServices;
using InfiniteRoleplay.Windows.Functions;
using InfiniteRoleplay;
using System.IO;
using System.Net.Http;
using FFXIVClientStructs.Interop;

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

        private static int port = 5392;
        public static Plugin plugin;
      
       
        public static bool IsConnectedToServer(TcpClient _tcpClient)
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                {
                    // Detect if client disconnected
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not run IsConnectedToServer" + ex.ToString());
                return false;
            }
        }

        public static async Task CheckStatus()
        {
            try
            {
                if (IsConnectedToServer(ClientTCP.clientSocket))
                {
                    if (loadCallback)
                    {
                        ClientConnectionCallback();
                        loadCallback = false;
                        DataSender.SendHelloServer("", "");
                    }
                    if (!plugin.uiLoaded)
                    {
                        plugin.LoadUI();
                    }
                }
                else
                {
                    await ConnectToServer();
                }
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not check status" + ex.ToString());
            }
        }

        public static async Task ConnectToServer()
        {
            try
            {
                ClientHandleData.InitializePackets(true);
                await InitializingNetworking(true);
                loadCallback = true;
                await CheckStatus();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not connect to server " + ex.ToString());
            }
        }

        public static async Task InitializingNetworking(bool start)
        {
            try
            {
                if (start == true)
                {
                    await EstablishConnection();
                }
                else
                {
                    if (clientSocket.Connected == true)
                    {
                        Disconnect();
                }
                }

            }catch(Exception ex) 
            {
                Dalamud.Logging.PluginLog.LogError("Could not Initialize Networking " + ex.ToString());
            }
        }

        public static async Task EstablishConnection()
        {
            try
            {
                clientSocket = new TcpClient();
                clientSocket.ReceiveBufferSize = 65535;
                clientSocket.SendBufferSize = 65535;
                recBuffer = new byte[65535 * 2];
                await clientSocket.ConnectAsync(server, port);
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not establish connection " + ex.ToString());
            }
        }

        public static void ClientConnectionCallback()
        {
            try
            {
            Connected = true;
            clientSocket.NoDelay = true;
            myStream = clientSocket.GetStream();
            myStream.BeginRead(recBuffer, 0, 4096 * 2, ReceiveCallback, null);

            }catch(Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("ClientConnectionCallback failed " + ex.ToString());
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
                Dalamud.Logging.PluginLog.LogError("Could not receive callback " + ex.ToString());
            }
        }

        public static async Task SendData(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer();
                buffer.WriteInteger(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
                buffer.WriteBytes(data);
                await myStream.WriteAsync(buffer.ToArray(), 0, buffer.ToArray().Length);
                buffer.Dispose();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not send data " + ex.ToString());
            }
        }

        public static void Disconnect()
        {
            try
            {
                Connected = false;
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.LogError("Could not disconnect " + ex.ToString());
            }
        }

    }
}
