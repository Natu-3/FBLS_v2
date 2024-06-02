using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Simple TCP listen server that accepts TCP clients but does no message handling.
/// Runs on a coroutine.
/// </summary>
public class SimpleTcpListener : IMessageSource
{
    private TcpClient client = null;
    private TcpListener listener = null;
    private bool isListening = false;

    private NetworkStream nwStream;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public event Func<string, Task> OnMessageRecieved;
    public bool IsListening => isListening;

    public SimpleTcpListener(string ip, int port)
    {
        IPAddress addr = IPAddress.Parse(ip);
        listener = new TcpListener(addr, port);
    }

    public async void Start()
    {
        isListening = true;
        listener.Start();

        Debug.Log("Listening for connections on " +
            IPAddress.Parse(((IPEndPoint)listener.LocalEndpoint).Address.ToString()) +
            " on port number " + ((IPEndPoint)listener.LocalEndpoint).Port.ToString());

        //Start update loop
        await Task.Run(ListenLoop, tokenSource.Token);
    }

    public void Send(string message)
    {
        if (!client.Connected) 
        {
            Debug.Log("Cannot send message as the client is not connected!");
            return;
        }

        if (!nwStream.CanWrite)
        {
            Debug.Log("Cannot write to the message stream at this time!");
            return;
        }

        byte[] bytes = Encoding.ASCII.GetBytes(message);
        Debug.Log("Sending message of size: " + bytes.Length + " bytes.");
        nwStream.Write(bytes, 0, bytes.Length);
    }

    private async Task ListenLoop()
    {
        Debug.Log("Listening...");
        while (client == null)
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            if (!listener.Pending())
            {
                Debug.Log("Sleeping (waiting for connection)..");
                await Task.Delay(1000);
            }
            else
            {
                client = listener.AcceptTcpClient();
                IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                Debug.Log($"TCP Messaging Client Connected: '{remoteIpEndPoint.Address}'");
            }
        }

        // Get a stream object for reading and writing
        nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = -1;

        // Message loop
        while (client.Connected)
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            try
            {
                if (nwStream.CanRead && nwStream.DataAvailable)
                {
                    // Read incoming stream
                    Debug.Log("Reading data...");
                    bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //Single byte packets are not serializable data - may be used as pings or acks.
                    if (bytesRead < 2) 
                    {
                        continue;
                    }

                    // Convert the data received into a string
                    // We may get a null terminator in the first message so we sanitize it away
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.Log($"Received: '{dataReceived}'");
                    string cleanedData = dataReceived.Replace("\0", string.Empty);
                    Debug.Log($"Received (clean): '{cleanedData}'");

                    //Execute any registered callbacks
                    await OnMessageRecieved?.Invoke(cleanedData);
                }
                else
                {
                    Debug.Log("Sleeping (waiting for next message)..");
                    await Task.Delay(1000);
                }
            }
            catch (SocketException ex)
            {
                HandleClose(ex.Message);
                return;
            }

            if (bytesRead == 0)
            {
                HandleClose("Connection closed.");
                return;
            }
        }
    }

    public void CloseAllConnections()
    {
        Debug.Log("Closing TCP listener and all client connections.");
        tokenSource.Cancel();
        isListening = false;

        if (listener?.Server?.Connected == true)
        {
            listener?.Server?.Shutdown(SocketShutdown.Both);   
        }
        if (client?.Client?.Connected == true) 
        {
            client?.Client?.Shutdown(SocketShutdown.Both);
        }

        listener?.Server?.Close();
        listener?.Stop();
        client?.Client?.Close();
        nwStream?.Close();
        client?.Close();
    }

    //Handle a close request or disconnect from the client
    static void HandleClose(string message)
    {
        Debug.Log(message);
        Application.Quit(0);
    }
}
