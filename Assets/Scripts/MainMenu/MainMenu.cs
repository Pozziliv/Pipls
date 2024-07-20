using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private AnimationCurve _earthTransitionCurve;
    [SerializeField] private SpriteRenderer _whitePanel;
    [SerializeField] private float _cameraRollTime;

    private void Start()
    {
        _camera = Camera.main;
    }

    public void StartButton()
    {
        StartCoroutine(LoadToGameCutscene());
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator LoadToGameCutscene()
    {
        float progress = 0f;

        Color whiteTransparent = new Color(1, 1, 1, 0);

        float cameraStartSize = _camera.orthographicSize;

        while (progress <= 1f)
        {
            _camera.orthographicSize = Mathf.Lerp(cameraStartSize, cameraStartSize + 450f, _earthTransitionCurve.Evaluate(progress));
            _whitePanel.color = Color.Lerp(whiteTransparent, Color.white, _earthTransitionCurve.Evaluate(progress));

            progress += Time.deltaTime / _cameraRollTime;

            yield return null;
        }

        SceneManager.LoadScene(1);
    }
}
