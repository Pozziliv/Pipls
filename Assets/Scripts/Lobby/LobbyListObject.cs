using UnityEngine;

public class LobbyListObject : MonoBehaviour
{
    [SerializeField] private PasswordCheck _passwordCheck;
    [SerializeField] private Transform _lobbyParent;

    public PasswordCheck PasswordCheck => _passwordCheck;
    public Transform LobbyParent => _lobbyParent;
}
