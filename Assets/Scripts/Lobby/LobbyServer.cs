using kcp2k;
using Mirror;
using System.Collections;
using UnityEngine;

public class LobbyServer : NetworkBehaviour
{
    private Requester _requester;

    [Server]
    private void Start()
    {
        _requester = new Requester();
        _requester.Start();

        StartCoroutine(UpdatePlayers());
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        var netManager = NetworkManager.singleton as ProjectNetworkManager;
        netManager.BackToLobby();
    }

    private IEnumerator UpdatePlayers()
    {
        var waitTime = new WaitForSeconds(10f);

        while (true)
        {
            yield return waitTime;

            int players = NetworkServer.connections.Count;

            if (players == 0)
            {
                ushort port = (NetworkManager.singleton.transport as KcpTransport).port;
                string tempString = _requester.SendMessage("DeleteServer " + port);
                break;
            }
        }
        _requester.Stop();
        Application.Quit();
    }

    [Server]
    private void OnDestroy()
    {
        if (_requester != null)
            _requester.Stop();
    }
}
