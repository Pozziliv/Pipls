using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LobbyData))]
public class LobbyButton : NetworkBehaviour
{
    [SerializeField] private Button _button;
    private LobbyData _lobbyData;

    private void Start()
    {
        _lobbyData = GetComponent<LobbyData>();
        transform.parent = GameObject.FindGameObjectWithTag("LobbyUIParent").transform;
    }

    public void OnEnable()
    {
        _button.onClick.AddListener(ConnectToServer);
    }

    public void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    [Client]
    public void ConnectToServer()
    {
        if(_lobbyData.PlayersCount < 4)
        {
            var netManager = NetworkManager.singleton as ProjectNetworkManager;
            netManager.LoggingIntoGame(_lobbyData.Port);
        }
    }
}
