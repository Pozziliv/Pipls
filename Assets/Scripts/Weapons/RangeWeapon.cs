using UnityEngine;
using Mirror;

public class RangeWeapon : Weapon
{
    [SerializeField] private ShootAmmo _bullet;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private int _ammo;

    [SyncVar(hook = nameof(SetBullet))] private bool _isReady = false;

    public bool IsReady => _isReady;
    public ShootAmmo Bullet => _bullet;

    private void SetBullet(bool oldBool, bool newBool)
    {
        _spriteRenderer.enabled = newBool;
    }

    public void SetReady(int ammoCount)
    {
        _isReady = true;
        _ammo += ammoCount;
    }

    [Server]
    public void Shoot()
    {
        if (_isReady)
        { 
            var angle = _parentManager.transform.localEulerAngles.z;

            var spawnedBullet = Instantiate(_bullet, _shootPosition.position, Quaternion.identity);

            spawnedBullet.SetDirection(angle);
            NetworkServer.Spawn(spawnedBullet.gameObject);

            spawnedBullet.StartFly(angle);

            _ammo--;

            if (_ammo == 0)
            {
                _isReady = false;
            }
        }
    }
}
