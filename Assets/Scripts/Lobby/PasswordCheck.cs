using Mirror;
using TMPro;
using UnityEngine;

public class PasswordCheck : MonoBehaviour
{
    private string _password;
    private string _port;

    [SerializeField] private TMP_InputField _inputField;

    public void SetData(string password, string port)
    {
        _password = password;
        _port = port;
    }

    public void CheckData()
    {
        var inputPassword = _inputField.text;

        if(_password == inputPassword)
        {
            var netManager = NetworkManager.singleton as ProjectNetworkManager;

            netManager.LoggingIntoGame(_port);
        }
    }
}
