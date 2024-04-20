using kcp2k;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transistor : MonoBehaviour
{
    [Scene, SerializeField] public string _offlineScene;

    private IEnumerator Start()
    {
        NetworkManager networkManager = NetworkManager.singleton;
        KcpTransport transport = networkManager.transport as KcpTransport;

        uint port = transport.port;

        if(port == 0)
        {
            SceneManager.LoadScene(_offlineScene);
        }
        else
        {
            yield return new WaitForSecondsRealtime(2f);

            networkManager.StartClient();
        }
    }
}
