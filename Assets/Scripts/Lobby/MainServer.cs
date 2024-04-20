using UnityEngine;
using Mirror;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class MainServer : NetworkBehaviour
{
    [SerializeField] private LobbyData _lobbyUIPrefab;
    [SerializeField] private Transform _lobbyUIParent;
    public readonly SyncList<LobbyData> _lobbyDatas = new SyncList<LobbyData>();

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
        TargetSettingForConnection(sender, newPort);

        LobbyData lobbyData = Instantiate(_lobbyUIPrefab, _lobbyUIParent);
        _lobbyDatas.Add(lobbyData);
        NetworkServer.Spawn(lobbyData.gameObject);
        lobbyData.SetPort(newPort);
        lobbyData.AddPlayer();
    }

    [TargetRpc]
    private void TargetSettingForConnection(NetworkConnectionToClient connectionToClient, string port)
    {
        var netManager = NetworkManager.singleton as ProjectNetworkManager;
        netManager.LoggingIntoGame(port);
    }

    private void OnApplicationQuit()
    {
        _requester.Stop();
    }

    private IEnumerator ServerListUpdate()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(10f);

        while (true)
        {
            yield return cooldown;

            string jsonDataInString = _requester.SendMessage("GetServersData");

            PortsData data = (PortsData)JsonConvert.DeserializeObject(jsonDataInString, typeof(PortsData));

            foreach(var port in data.Ports)
            {
                foreach(var lobby in _lobbyDatas)
                {
                    if(lobby.Port == port)
                    {
                        lobby.SetPlayers(data.Players[data.Ports.IndexOf(port)]);
                    }
                    if (!data.Ports.Contains(lobby.Port))
                    {
                        _lobbyDatas.Remove(lobby);
                        NetworkServer.Destroy(lobby.gameObject);
                        Destroy(lobby.gameObject);
                    }
                }
            }
        }
    }
}
