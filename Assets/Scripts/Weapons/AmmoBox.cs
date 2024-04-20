using Mirror;
using UnityEngine;

public class AmmoBox : NetworkBehaviour, ISpawnable
{
    [SerializeField] private int _ammoCount = 1;
    [SerializeField] private string _weaponName;

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
}
