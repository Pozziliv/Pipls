using Mirror;
using UnityEngine;

public class Arrow : ShootAmmo
{
    [SerializeField] protected Collider2D _floorCollider;
    [SerializeField] protected Collider2D _flyCollider;
    [SerializeField] private int _ammoCount = 1;

    private int _ricochets = 1;

    private bool _flyMode = false;
    [SerializeField] private string _weaponName;

    [ServerCallback]
    private void Update()
    {
        if (_flyMode == false) return;

        if (_rigidbody.velocity.sqrMagnitude <= 1f)
        {
            StopFly();
        }

        if (_rigidbody.velocity.sqrMagnitude <= 0.3f)
        {
            _flyCollider.enabled = false;
        }
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerAttack playerAttack) && playerAttack.Weapon.RangeWeapon == true)
        {
            var rangeWeapon = playerAttack.Weapon as RangeWeapon;
            if (rangeWeapon.gameObject.name == _weaponName)
            {
                rangeWeapon.SetReady(_ammoCount);

                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.TryGetComponent(out PlayerManager playerManager);

        if (isPlayer && _flyMode && _ricochets > 0)
        {
            Vector3 diff = (collision.transform.position - transform.position).normalized;

            playerManager.GetDamage(_damage, diff * _knockbackForce);
        }

        _ricochets--;

        if (_ricochets == 0)
        {
            StopFly();
        }
    }

    public override void StartFly(float rotation)
    {
        _floorCollider.enabled = false;

        _flyMode = true;
        _rigidbody.AddForce(Quaternion.Euler(0, 0, rotation - 90f) * Vector2.up * 9f, ForceMode2D.Impulse);

        _ricochets = 1;
    }

    public override void StopFly()
    {
        _floorCollider.enabled = true;

        _flyMode = false;
    }
}
