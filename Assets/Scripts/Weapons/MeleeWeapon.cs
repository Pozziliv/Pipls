using UnityEngine;
using Mirror;
using System.Linq;

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

            DisableAttackTriggers();
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
            PlayerManager playerManager = null;
            Physics2D.OverlapCircleAll(_parentManager.BodyRotation *( Vector3.right * 1f) + _parentManager.transform.position, 0.1f).FirstOrDefault(x => (x.TryGetComponent(out playerManager) && playerManager != _parentManager));

            if(playerManager != null && playerManager != _parentManager)
            {
                Vector3 diff = (collision.transform.position - _parentManager.transform.position).normalized;

                playerManager.GetDamage(_damage, diff * _knockbackForce);
            }
        }
    }

    public void ActivateAttackTriggers()
    {
        foreach(var trigger in _damageColliders)
        {
            trigger.enabled = true;
        }
    }

    public void DisableAttackTriggers()
    {
        foreach (var trigger in _damageColliders)
        {
            trigger.enabled = false;
        }
    }
}
