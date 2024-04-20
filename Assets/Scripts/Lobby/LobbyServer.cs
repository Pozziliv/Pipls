using kcp2k;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class LobbyServer : NetworkBehaviour
{
    private Requester _requester;

    private void Start()
    {
        _requester = new Requester();
        _requester.Start();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        CmdUpdatePlayers();
    }

    [Client]
    private void OnDisable()
    {
        CmdUpdatePlayers();
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdatePlayers()
    {
        int players = NetworkServer.connections.Count;
        ushort port = (NetworkManager.singleton.transport as KcpTransport).port;
        _requester.SendMessage("SetPlayers " + port + " " + players);

        if(players == 0)
        {
            _requester.SendMessage("DeleteServer " + port);
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        if (_requester != null)
            _requester.Stop();
    }

    private void OnApplicationQuit()
    {
        if (_requester != null)
            _requester.Stop();
    }
}
