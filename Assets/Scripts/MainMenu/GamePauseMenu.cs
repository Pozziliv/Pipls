using Mirror;
using UnityEngine;

public class GamePauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pauseMenu.activeInHierarchy)
            {
                _pauseMenu.SetActive(false);
            }
            else
            {
                _pauseMenu.SetActive(true);
            }
        }
    }

    public void Resume()
    {
        _pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        NetworkManager.singleton.StopClient();
    }
}
