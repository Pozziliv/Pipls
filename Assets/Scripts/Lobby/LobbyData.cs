using Mirror;
using TMPro;
using UnityEngine;

public class LobbyData : NetworkBehaviour
{
    [SerializeField] private TMP_Text _playersCount;
    [SerializeField, SyncVar] private string _port;
    [SerializeField, SyncVar(hook = nameof(ChangePlayersCount))] private int _players = 0;

    public int PlayersCount => _players;
    public string Port => _port;

    private void Start()
    {
        _playersCount.text = _players.ToString();
    }

    [Server]
    public void AddPlayer()
    {
        _players++;
    }

    [Server]
    public void SetPort(string port)
    {
        _port = port;
    }

    public void SetPlayers(int count)
    {
        _players = count;
    }

    private void ChangePlayersCount(int oldCOunt, int newCount)
    {
        _playersCount.text = newCount.ToString() + "/4";
    }
}
