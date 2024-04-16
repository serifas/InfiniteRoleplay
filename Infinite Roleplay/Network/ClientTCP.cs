using InfiniteRoleplay;
using Networking;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class ClientTCP : IDisposable
{
    private static TcpClient _clientSocket;
    private static NetworkStream _myStream;
    private static byte[] _recBuffer;
    private static readonly string _server = "185.33.84.184";
    private static readonly int _port = 25565;
    public static bool Connected { get; private set; }
    public static bool LoadCallback { get; set; }
    public static Plugin Plugin { get; set; }
    public int CheckCounter { get; set; } = 5;

    public static async Task<bool> IsConnectedToServer()
    {
        try
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                if (_clientSocket.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (_clientSocket.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        // Client is not connected
                        // Handle UI or logging here
                        return false;
                    }
                    // Client is connected
                    return true;
                }
                return true;
            }
            // Client is not connected
            return false;
        }
        catch (Exception ex)
        {
            // Handle exception
            return false;
        }
    }

    public static async Task CheckStatus()
    {
        try
        {
            if (await IsConnectedToServer())
            {
                if (LoadCallback)
                {
                    ClientConnectionCallback();
                    LoadCallback = false;
                }
                if (!Plugin.uiLoaded)
                {
                    Plugin.LoadUI();
                }
            }
            else
            {
                await ConnectToServer();
            }
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static async Task ConnectToServer()
    {
        try
        {
            await InitializingNetworking(true);
            LoadCallback = true;
            await CheckStatus();
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static async Task InitializingNetworking(bool start)
    {
        try
        {
            if (start)
            {
                await EstablishConnection();
            }
            else
            {
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    Disconnect();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static async Task EstablishConnection()
    {
        try
        {
            _clientSocket = new TcpClient();
            _clientSocket.ReceiveBufferSize = 65535;
            _clientSocket.SendBufferSize = 65535;
            _recBuffer = new byte[65535 * 2];
            await _clientSocket.ConnectAsync(_server, _port);
            ClientConnectionCallback();
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static void ClientConnectionCallback()
    {
        try
        {
            Connected = true;
            _clientSocket.NoDelay = true;
            _myStream = _clientSocket.GetStream();
            _myStream.BeginRead(_recBuffer, 0, 4096 * 2, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    private static void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            var length = _myStream.EndRead(result);
            if (length <= 0)
            {
                return;
            }
            var newBytes = new byte[length];
            Array.Copy(_recBuffer, newBytes, length);
            ClientHandleData.HandleData(newBytes);
            _myStream.BeginRead(_recBuffer, 0, 4096 * 2, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static async Task SendData(byte[] data)
    {
        try
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger(data.Length);
            buffer.WriteBytes(data);
            await _myStream.WriteAsync(buffer.ToArray(), 0, buffer.ToArray().Length);
            buffer.Dispose();
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public static void Disconnect()
    {
        try
        {
            Connected = false;
            _clientSocket?.Close();
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public void Dispose()
    {
        _myStream?.Dispose();
        _clientSocket?.Dispose();
    }
}
