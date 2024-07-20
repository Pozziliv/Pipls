using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private PlayerManager _manager;

    [SerializeField] private int _maxHealth = 100;
    [SerializeField, SyncVar(hook = nameof(ChangeDamageIndicator))] private int _health = 100;

    [SerializeField] private SpriteRenderer _damageIndicator;

    [SerializeField] private AudioSource _damageSource;
    [SerializeField] private AudioSource _dieSource;

    public void GetDamage(int damage)
    {
        if(_manager.IsInvulnerable == true) return;

        if(damage < 0) damage = 0;

        _health -= damage;

        if (_health <= 0)
        {
            _manager.ServerDie();
        }
        else
        {
            _damageSource.Play();
        }
    }

    [ClientRpc]
    private void RpcPlayDieSound()
    {
        _dieSource.Play();
    }

    [ClientRpc]
    private void RpcPlayDamageSound()
    {
        _damageSource.Play();
    }

    private void ChangeDamageIndicator(int oldHealth, int newHealth)
    {
        _damageIndicator.color = new Color(1, 1, 1, 1 - (float)newHealth / _maxHealth);
    }

    public void HealthReset()
    {
        _health = 100;
        RpcResetHPBar();
    }

    [ClientRpc]
    private void RpcResetHPBar()
    {
        _damageIndicator.color = new Color(1, 1, 1, 0);
    }
}
