using kcp2k;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class LobbyServerStarter : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private KcpTransport _kcpTransport;

    private Requester _requester;

    private IEnumerator Start()
    {
        _requester = new Requester();
        _requester.Start();

        yield return new WaitForSecondsRealtime(2f);

        _kcpTransport.port = Convert.ToUInt16(_requester.SendMessage("GetPort"));
        _networkManager.StartServer();
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
