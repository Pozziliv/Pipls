using UnityEngine;
using Mirror;
using System;

public class Weapon : NetworkBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected PlayerManager _parentManager;

    [SerializeField] private int _index;
    [SerializeField] private bool _rangeWeapon;

    public int Index => _index;
    public bool RangeWeapon => _rangeWeapon;

    [Command]
    public void CmdStartAttack()
    {
        _animator.SetBool("IsAttack", true);
    }

    [Command]
    public void CmdStopAttack()
    {
        _animator.SetBool("IsAttack", false);
    }
}
