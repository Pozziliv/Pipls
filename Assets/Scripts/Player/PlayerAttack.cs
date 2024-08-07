using Mirror;
using System;
using System.Linq;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField, SyncVar] private Weapon _weapon;
    [SerializeField] private Weapon[] _allWeapons;

    [SerializeField] private PlayerManager _playerManager;

    [SerializeField] private AudioSource _throughWeapon;

    public Weapon Weapon => _weapon;

    private void Start()
    {
        foreach (var weapon in _allWeapons)
        {
            weapon.gameObject.SetActive(false);
        }

        _allWeapons[0].gameObject.SetActive(true);
    }

    [Client]
    private void OnEnable()
    {
        _playerManager.OnDead += StopWeaponAttack;
        _playerManager.OnDead += DropThisWeapon;
    }

    [Client]
    private void OnDisable()
    {
        _playerManager.OnDead -= StopWeaponAttack;
        _playerManager.OnDead -= DropThisWeapon;
    }

    private void Update()
    {
        if (isLocalPlayer == false) return;

        if (_playerManager.IsDead == true || _playerManager.GameStarted == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            _weapon.CmdStartAttack();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _weapon.CmdStopAttack();
        }

        if (Input.GetKeyDown(KeyCode.E) && _weapon.Index != 0)
        {
            if (_weapon.RangeWeapon == true && (_weapon as RangeWeapon).IsReady == true)
            {
                _weapon.CmdStartAttack();
            }
            else
            {
                DropThisWeapon();
                _throughWeapon.Play();
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && _weapon.RangeWeapon == true)
        {
            _weapon.CmdStopAttack();
        }
    }

    public void ServerChangeWeapon(string name)
    {
        //_weapon.gameObject.SetActive(false);

        Weapon newWeapon = _allWeapons.First(x => x.gameObject.name == name);
        newWeapon.gameObject.SetActive(true);

        RpcChangeWeapon(_weapon, newWeapon);

        _weapon = newWeapon;
    }

    [ClientRpc]
    public void RpcChangeWeapon(Weapon oldWeapon, Weapon newWeapon)
    {
        oldWeapon.CmdStopAttack();
        newWeapon.ResetPositions();
        oldWeapon.gameObject.SetActive(false);

        newWeapon.gameObject.SetActive(true);
    }

    [Command]
    public void CmdClearWeapon()
    {
        ServerChangeWeapon("Arms");
    }

    [Client]
    public void StopWeaponAttack()
    {
        _weapon.CmdStopAttack();
    }

    private void DropThisWeapon()
    {
        if(_weapon.Index == 0) 
        {
            return;
        }

        if(_weapon.RangeWeapon == true)
        {
            (_weapon as RangeWeapon).CmdResetAmmo();
        }

        _playerManager.DropWeapon(_weapon);
        CmdClearWeapon();
    }
}
