using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private MainServer _mainServer;

    [SerializeField] private Button _hostButton;

    [ClientCallback]
    private void OnEnable()
    {
        _hostButton.onClick.AddListener(HostButton);
    }

    [ClientCallback]
    private void OnDisable()
    {
        _hostButton.onClick.RemoveAllListeners();
    }

    public void HostButton()
    {
        _mainServer.CmdStartNewServer();
    }
}
