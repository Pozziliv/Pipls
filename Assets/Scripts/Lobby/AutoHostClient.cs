using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    void Start()
    {
        if (!Application.isBatchMode)
        { //Headless build
            Debug.Log($"=== Client Build ===");
            //networkManager.StartClient();
        }
        else
        {
            Debug.Log($"=== Server Build ===");
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