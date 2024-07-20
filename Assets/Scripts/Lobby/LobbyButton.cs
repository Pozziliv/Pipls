using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LobbyData))]
public class LobbyButton : NetworkBehaviour
{
    [SerializeField] private Button _button;
    private LobbyData _lobbyData;
    private PasswordCheck _passwordCheck;
    private LobbyListObject _lobbyListObject;

    private void Start()
    {
        _lobbyData = GetComponent<LobbyData>();
        _lobbyListObject = FindFirstObjectByType<LobbyListObject>();
        transform.parent = _lobbyListObject.LobbyParent;
        transform.localScale = Vector3.one;
        _passwordCheck = _lobbyListObject.PasswordCheck;
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
        if(_lobbyData.Locked == true)
        {
            _lobbyListObject.gameObject.SetActive(false);
            _passwordCheck.gameObject.SetActive(true);

            _passwordCheck.SetData(_lobbyData.Password, _lobbyData.Port);
        }
        else
        {
            var netManager = NetworkManager.singleton as ProjectNetworkManager;

            netManager.LoggingIntoGame(_lobbyData.Port);
        }

    }
}
