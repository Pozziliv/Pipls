using Mirror;
using System;
using UnityEngine;

public class FloorWeapon : NetworkBehaviour, ISpawnable
{
    [SerializeField] private string _name;
    [SerializeField] private Collider2D _floorCollider;
    [SerializeField] private Collider2D _flyCollider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _knockbackForce = 2;
    [SerializeField] private float _flyForce = 4;
    [SerializeField] private GameObject _ammo;

    private bool _flyMode = false;

    private NetworkConnectionToClient _sender;
    private float _flyTimer;
    private float _cantPickUpTime = 1f;

    private int _ricochets = 1;

    public GameObject Ammo => _ammo;

    public GameObject ThisGO { get ; set; }

    private void Start()
    {
        ThisGO = gameObject;
    }

    [ServerCallback]
    private void Update()
    {
        if (_flyCollider.enabled && _rigidbody.velocity.sqrMagnitude <= 0.3f)
        {
            RpcOffCollider();
        }

        if (_flyTimer >= _cantPickUpTime && _sender != null)
        {
            _sender = null;
        }
        else if(_flyTimer <  _cantPickUpTime)
        {
            _flyTimer += Time.deltaTime;
        }

        if(_flyMode == false) return;

        if(_rigidbody.velocity.sqrMagnitude <= 2f)
        {
            StopFly();
        }
    }

    [ClientRpc]
    private void RpcOffCollider()
    {
        _flyCollider.enabled = false;
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerAttack playerAttack) && playerAttack.netIdentity.connectionToClient != _sender)
        {
            if(playerAttack.Weapon.gameObject.name == "Arms")
            {
                playerAttack.ServerChangeWeapon(_name);

                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.TryGetComponent(out PlayerManager playerManager);

        if (isPlayer  && _flyMode && _ricochets > 0 
            && playerManager.GetComponent<NetworkIdentity>().connectionToClient != _sender)
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

    public void SetDirection(float angle)
    {
        transform.eulerAngles = Vector3.forward * (angle - 90f);
    }

    public void StartFly(float rotation)
    {
        _floorCollider.enabled = false;
        _rigidbody.AddForce(Quaternion.Euler(0, 0, rotation - 90f) * Vector2.up * _flyForce, ForceMode2D.Impulse);
        _flyCollider.enabled = true;

        _flyMode = true;

        _ricochets = 1;
    }

    public void StopFly()
    {
        _floorCollider.enabled = true;

        _flyMode = false;
    }

    public void SetSender(NetworkConnectionToClient connectionToClient)
    {
        _sender = connectionToClient;
    }
}
