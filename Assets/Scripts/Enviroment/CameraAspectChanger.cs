using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectChanger : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private Vector2 _aspect;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        var camAspect = _camera.aspect;
        var normalAspect = _aspect.x/_aspect.y;

        if(camAspect < normalAspect)
        {
            _camera.orthographicSize = 5 * normalAspect / camAspect;
        }
    }

}
