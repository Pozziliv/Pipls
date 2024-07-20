using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Weapon : NetworkBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected PlayerManager _parentManager;

    [SerializeField] protected AudioSource _attackAudio;

    [SerializeField] private int _index;
    [SerializeField] private bool _rangeWeapon;

    private List<Vector3> _startPositions = new List<Vector3>();
    private List<Vector3> _startRotations = new List<Vector3>();

    public int Index => _index;
    public bool RangeWeapon => _rangeWeapon;

    private void Awake()
    {
        foreach(var child in transform.GetComponentsInChildren<Transform>())
        {
            _startPositions.Add(child.localPosition);
            _startRotations.Add(child.localEulerAngles);
        }
    }

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

    public void ResetPositions()
    {
        if(_startPositions.Count == 0)
        {
            return;
        }

        int index = 0;

        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if(_startPositions.Count - 1 < index)
            {
                return;
            }

            child.localPosition = _startPositions[index];
            child.localEulerAngles = _startRotations[index];

            index++;
        }
    }

    [ClientCallback]
    public virtual void PlayAttackSound()
    {
        _attackAudio.Play();
    }
}
