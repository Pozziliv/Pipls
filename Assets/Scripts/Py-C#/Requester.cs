using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class Requester : RunAbleThread
{
    private RequestSocket _client;

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet

        _client = new RequestSocket();
        _client.Connect("tcp://localhost:5555");

        SendMessage("Starting...");

        while (Running) { }

        _client.Close();

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    public string SendMessage(string message)
    {
        Debug.Log("Send " + message);

        if (Running)
            _client.SendFrame(message);

        bool gotMessage = false;
        string gottedMessage = "";

        while (gotMessage == false && Running)
        {
            gotMessage = _client.TryReceiveFrameString(out gottedMessage); // this returns true if it's successful

            if (gotMessage) Debug.Log("Received " + gottedMessage);
        }

        return gottedMessage;
    }
}