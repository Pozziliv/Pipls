using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private MainServer _mainServer;
    [SerializeField] private Toggle _lockToggle;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    public void HostButton()
    {
        var name = _nameInputField.text;
        var isLocked = _lockToggle.isOn;
        var password = _passwordInputField.text;

        if (name == "")
        {
            name = "Room";
        }

        if (isLocked == false)
        {
            password = string.Empty;
        }

        _mainServer.CmdStartNewServer();
        _mainServer.CmdCreateLobby(isLocked, name, password);
    }
}
