using Mirror;
using UnityEngine;

public class PlayerLooking : NetworkBehaviour
{
    [SerializeField] private PlayerManager _playerManager;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (isLocalPlayer == false) return;

        if(_playerManager.IsDead == true) return;

        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var diff = mousePosition - transform.position;
        var angle = Vector2.SignedAngle(Vector3.right, diff);

        transform.eulerAngles = Vector3.forward * angle;
    }

    public float GetLookDirection()
    {
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var diff = mousePosition - transform.position;
        var angle = Vector2.SignedAngle(Vector3.right, diff);

        return angle;
    }
}
