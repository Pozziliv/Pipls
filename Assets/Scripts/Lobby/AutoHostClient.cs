using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
        _camera.orthographicSize = _camera.orthographicSize + 450;
        if (!Application.isBatchMode)
        { //Headless build
            Debug.Log($"=== Client Build ===");
            //networkManager.StartClient();
        }
        else
        {
            Debug.Log($"=== Server Build ===");
            //networkManager.StartServer();
        }
    }

    public void JoinLocal()
    {
        networkManager.StartClient();
    }

    public void HostLocal()
    {
        networkManager.StartServer();
    }

    public void SetAdress(string ip)
    {
        networkManager.networkAddress = ip;
    }
}