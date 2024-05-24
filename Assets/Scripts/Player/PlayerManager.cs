using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer[] _hands;
    [SerializeField] private ParticleSystem[] _particles;

    [SerializeField] private Sprite[] _bodySprites;
    [SerializeField] private Sprite[] _handsSprites;
    [SerializeField] private Color[] _particleColors;

    [SyncVar(hook = nameof(SetSprites))] private int _index = -1;

    [SerializeField] private GameObject[] _main;
    [SerializeField] private GameObject[] _die;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerLooking _playerLooking;
    [SerializeField] private PlayerHealth _playerHealth;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _bodyCollider;

    [SyncVar] private bool _isDead = false;
    [SyncVar] private bool _isInvulnerable = false;

    private bool _gameStarted = false;
    private GameManager _gameManager;

    public Action OnDead;

    public int Index => _index;
    public bool IsDead => _isDead;
    public bool GameStarted => _gameStarted;
    public bool IsInvulnerable => _isInvulnerable;

    private void OnEnable()
    {
        if(_gameManager != null)
        {
            _gameManager.OnGameStartRestarting += HideMainParts;

            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
            _gameManager.OnGameStarted += HealthReset;
        }
    }

    private void OnDisable()
    {
        if(_gameManager != null)
        {
            _gameManager.OnGameStartRestarting -= HideMainParts;

            _gameManager.OnGameStarted -= GameStart;
            _gameManager.OnGameStarted -= ShowMainParts;
            _gameManager.OnGameStarted -= HideDieParts;
            _gameManager.OnGameStarted -= HealthReset;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager != null && _index == -1)
        {
            _index = _gameManager.SetPlayerIndex();
        }

        if(_gameManager != null) 
        {
            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
            _gameManager.OnGameStarted += HealthReset;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager != null) { 
            _gameManager.SetPlayerManager(this);

            _gameManager.OnGameStartRestarting += HideMainParts;

            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
            _gameManager.OnGameStarted += HealthReset;
        }
    }

    public void GameStart()
    {
        _gameStarted = true;
        _isDead = false;
    }

    private void SetSprites(int oldIndex, int newIndex)
    {
        _body.sprite = _bodySprites[newIndex % 4];
        foreach (var hand in _hands)
        {
            hand.sprite = _handsSprites[newIndex % 4];
        }
        foreach (var particle in _particles)
        {
            var mainPart = particle.main;
            mainPart.startColor = _particleColors[newIndex % 4];
        }
    }

    private void HealthReset()
    {
        _playerHealth.HealthReset();
    }

    [Server]
    public void ServerDie()
    {
        _isDead = true;
        HideMainParts();
        RpcDie();
        _gameManager.PlayerDie();
    }

    [ClientRpc]
    private void RpcDie()
    {
        OnDead.Invoke();
        HideMainParts();
        ShowDieParts();
    }

    private void HideMainParts()
    {
        foreach (var part in _main)
        {
            part.SetActive(false);
        }
        _rigidbody.velocity = Vector3.zero;
        _bodyCollider.enabled = false;
    }

    private void ShowMainParts()
    {
        foreach (var part in _main)
        {
            part.SetActive(true);
        }
        _rigidbody.velocity = Vector3.zero;
        _bodyCollider.enabled = true;
    }

    private void HideDieParts()
    {
        foreach (var part in _die)
        {
            part.SetActive(false);
        }
    }

    private void ShowDieParts()
    {
        foreach (var part in _die)
        {
            part.SetActive(true);
        }
    }

    public void GetDamage(int damage, Vector3 knockbackDirection)
    {
        _playerMovement.TargetKnockBack(knockbackDirection);
        _playerHealth.GetDamage(damage);
    }

    public float GetLookDirection()
    {
        return _playerLooking.GetLookDirection();
    }

    public void DropWeapon(Weapon weapon)
    {
        var angle = GetLookDirection();
        CmdDropWeaponOnServer(weapon.Index, transform.position + (Quaternion.Euler(0, 0, angle) * Vector3.right), angle, netIdentity.connectionToClient);
    }

    [Command]
    private void CmdDropWeaponOnServer(int weaponIndex, Vector2 startPoint, float angle, NetworkConnectionToClient connectionToClient) 
    {
        _gameManager.DropWeapon(weaponIndex, startPoint, angle, connectionToClient);
    }

    [Command]
    public void CmdSetInvulnerability(bool invulnerability)
    {
        _isInvulnerable = invulnerability;
    }
}
