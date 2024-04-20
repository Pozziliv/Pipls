using kcp2k;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class ProjectNetworkManager : NetworkManager
{
    [SerializeField, Scene] private string _gameScene;
    [SerializeField, Scene] private string _lobbyScene;

    public void LoggingIntoGame(string port)
    {
        TransitionsData.Port = port;
        TransitionsData.Scene = _gameScene;
        StopClient();
    }

    public void BackToLobby()
    {
        TransitionsData.Port = "7000";
        TransitionsData.Scene = _lobbyScene;
    }

    public override void Start()
    {
        if(TransitionsData.Port != null && TransitionsData.Scene != null)
        {
            ChangeOnlineScene(TransitionsData.Scene);
            SetPort(TransitionsData.Port);

            StartCoroutine(LoadToServer());
        }
    }

    private IEnumerator LoadToServer()
    {
        yield return new WaitForSecondsRealtime(10f);

        StartClient();
    }

    public void SetPort(string port)
    {
        (transport as KcpTransport).port = Convert.ToUInt16(port);
    }

    public void ChangeOnlineScene(string scene)
    {
        onlineScene = scene;
    }
}
