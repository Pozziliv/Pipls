using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyData : NetworkBehaviour
{
    [SerializeField] private TMP_Text _lobbyNumber;
    [SerializeField] private TMP_Text _lobbyName;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Image _unlockImage;
    [SyncVar] private string _port;
    [SyncVar(hook=nameof(ChangeName))] private string _name = string.Empty;
    [SyncVar] private string _password;
    [SyncVar(hook =nameof(ChangeLock))] private bool _locked;

    public string Name => _name;
    public string Password => _password;
    public bool Locked => _locked;
    public string Port => _port;

    private void Start()
    {
        _lobbyName.text = _name;
    }

    [Server]
    public void SetPort(string port)
    {
        _port = port;
    }

    [Server]
    public void SetData(bool locked, string name)
    {
        _locked = locked;
        _name = name;
    }

    [Server]
    public void SetPassword(string password)
    {
        _password = password;
    }

    [ClientRpc]
    public void RpcChangeNumber(int number)
    {
        _lobbyNumber.text = number.ToString();
    }

    private void ChangeName(string oldName, string newName)
    {
        _lobbyName.text = newName;
    }

    private void ChangeLock(bool oldLock, bool newLock)
    {
        _unlockImage.gameObject.SetActive(oldLock);
        _lockImage.gameObject.SetActive(newLock);
    }
}
