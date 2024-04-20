using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoader : MonoBehaviour
{
    [SerializeField, Scene] private string _menuScene;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(_menuScene);
    }
}
