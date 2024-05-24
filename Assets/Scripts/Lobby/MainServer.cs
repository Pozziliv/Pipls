using UnityEngine;
using Mirror;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

public class MainServer : NetworkBehaviour
{
    [SerializeField] private LobbyData _lobbyUIPrefab;
    [SerializeField] private Transform _lobbyUIParent;
    private List<LobbyData> _lobbyDatas = new List<LobbyData>();

    private string _lobbyPort;

    private NetworkConnectionToClient _localConnection;

    private Requester _requester;

    public NetworkConnectionToClient LocalConnection => _localConnection;

    [Server]
    private void Start()
    {
        _requester = new Requester();
        _requester.Start();

        StartCoroutine(ServerListUpdate());
    }

    [ServerCallback]
    private void OnDestroy()
    {
        _requester.Stop();
    }

    [Command(requiresAuthority = false)]
    public void CmdStartNewServer(NetworkConnectionToClient sender = null)
    {
        string newPort = _requester.SendMessage("StartNewServer");

        LobbyData lobbyData = Instantiate(_lobbyUIPrefab, _lobbyUIParent);
        _lobbyDatas.Add(lobbyData);
        lobbyData.SetPort(newPort);

        TargetSettingForConnection(sender, newPort);
    }

    [Command(requiresAuthority = false)]
    public void CmdCreateLobby(bool locked, string name, string password)
    {
        LobbyData lobbyData = _lobbyDatas.First(x => x.Name == string.Empty);

        if (lobbyData == null) return;

        NetworkServer.Spawn(lobbyData.gameObject);
        lobbyData.SetData(locked, name);
        lobbyData.RpcChangeNumber(_lobbyDatas.Count);
        lobbyData.SetPassword(password);
    }

    [TargetRpc]
    private void TargetSettingForConnection(NetworkConnectionToClient connectionToClient, string port)
    {
        var netManager = NetworkManager.singleton as ProjectNetworkManager;
        netManager.LoggingIntoGame(port);
    }

    [Server]
    private void OnApplicationQuit()
    {
        _requester.Stop();
    }

    private IEnumerator ServerListUpdate()
    {
        var waitTime = new WaitForSeconds(10f);

        while (true)
        {
            yield return waitTime;

            string jsonDataInString = _requester.SendMessage("GetServersData");

            PortsData data = (PortsData)JsonConvert.DeserializeObject(jsonDataInString, typeof(PortsData));

            List<LobbyData> dataToRemove = new List<LobbyData>();

            foreach (var lobby in _lobbyDatas)
            {
                if (!data.Ports.Contains(lobby.Port))
                {
                    dataToRemove.Add(lobby);
                    NetworkServer.Destroy(lobby.gameObject);
                    Destroy(lobby.gameObject);
                }
            }

            foreach(var lobby in dataToRemove)
            {
                _lobbyDatas.Remove(lobby);
            }
        }
    }
}
