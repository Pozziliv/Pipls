using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private PlayerManager _manager;

    [SerializeField] private int _maxHealth = 100;
    [SerializeField, SyncVar(hook = nameof(ChangeDamageIndicator))] private int _health = 100;

    [SerializeField] private SpriteRenderer _damageIndicator;

    public void GetDamage(int damage)
    {
        if(_manager.IsInvulnerable == true) return;

        if(damage < 0) damage = 0;

        _health -= damage;

        if (_health <= 0)
        {
            _manager.ServerDie();
        }
    }

    private void ChangeDamageIndicator(int oldHealth, int newHealth)
    {
        _damageIndicator.color = new Color(1, 1, 1, 1 - (float)newHealth / _maxHealth);
    }

    public void HealthReset()
    {
        _health = _maxHealth;
    }
}
