using UnityEngine;
using Mirror;

public class RangeWeapon : Weapon
{
    [SerializeField] private ShootAmmo _bullet;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private GameObject _ammoReady;
    [SerializeField] private AudioSource _nullAmmoSound;
    private int _ammo;

    [SyncVar(hook = nameof(SetBullet))] private bool _isReady = false;

    public bool IsReady => _isReady;
    public ShootAmmo Bullet => _bullet;

    [Command]
    public void CmdResetAmmo()
    {
        _isReady = false;
        _ammo = 0;
    }

    private void SetBullet(bool oldBool, bool newBool)
    {
        _ammoReady.SetActive(newBool);
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
            var angle = _parentManager.BodyRotation.eulerAngles.z;

            var spawnedBullet = Instantiate(_bullet, _shootPosition.position, Quaternion.identity);

            spawnedBullet.SetDirection(angle);

            FindFirstObjectByType<GameManager>().AddSpawnedThing(spawnedBullet);

            NetworkServer.Spawn(spawnedBullet.gameObject);

            spawnedBullet.StartFly(angle);

            _ammo--;

            if (_ammo == 0)
            {
                _isReady = false;
            }
        }
    }

    public override void PlayAttackSound()
    {
        if(_ammo == 0)
        {
            _nullAmmoSound.Play();
        }
        else
        {
            _attackAudio.Play();
        }
    }
}
