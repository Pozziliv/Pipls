using UnityEngine;
using Mirror;

public class MeleeWeapon : Weapon
{
    [SerializeField] private Collider2D[] _damageColliders;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private int _damage;
    private bool _attackThroughWall = false;

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayer = collision.TryGetComponent(out PlayerManager playerManager);

        if (isPlayer && _attackThroughWall == false && playerManager != _parentManager)
        {
            Vector3 diff = (collision.transform.position - _parentManager.transform.position).normalized;

            playerManager.GetDamage(_damage, diff * _knockbackForce);
        }

        if(collision.tag == "Wall")
        {
            _attackThroughWall = true;
        }
    }

    [Server]
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            _attackThroughWall = false;
        }
    }
}
