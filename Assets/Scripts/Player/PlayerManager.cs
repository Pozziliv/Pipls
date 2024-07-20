using System;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [Header("For Colors")]
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer[] _hands;
    [SerializeField] private ParticleSystem[] _particles;
    [SerializeField] private SpriteRenderer _upperTag;
    [SerializeField] private SpriteRenderer _crownTag;

    [SerializeField] private Sprite[] _bodySprites;
    [SerializeField] private Sprite[] _handsSprites;
    [SerializeField] private Color[] _colors;

    [Space, SerializeField] private GameObject[] _main;
    [SerializeField] private GameObject[] _die;
    [SerializeField] private GameObject[] _instaParts;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerLooking _playerLooking;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private PlayerHealth _playerHealth;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _bodyCollider;

    [SyncVar(hook = nameof(SetSprites))] private int _index = -1;

    [SyncVar] private bool _isDead = false;
    [SyncVar] private bool _isInvulnerable = false;
    /*[SyncVar(hook = nameof(SetCrown))] */private bool _hasCrown = false;

    private bool _gameStarted = false;
    private bool _gameRestarted = false;
    private GameManager _gameManager;
    private int _gamesCount = 0;

    public Action OnDead;

    public int Index => _index;
    public bool IsDead => _isDead;
    public bool GameStarted => _gameStarted;
    public bool GameRestarted => _gameRestarted;
    public bool IsInvulnerable => _isInvulnerable;
    public Quaternion BodyRotation => _playerLooking.GameModel.rotation;

    private void OnEnable()
    {
        if(_gameManager != null)
        {
            _gameManager.OnGameStartRestarting += HideMainParts;
            _gameManager.OnGameStartRestarting += GameEnd;

            if (isServer)
            {
                _gameManager.OnGameStarted += HealthReset;
            }
            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
        }
    }

    private void OnDisable()
    {
        if(_gameManager != null)
        {
            _gameManager.OnGameStartRestarting -= HideMainParts;
            _gameManager.OnGameStartRestarting -= GameEnd;

            _gameManager.OnGameStarted -= GameStart;
            _gameManager.OnGameStarted -= ShowMainParts;
            _gameManager.OnGameStarted -= HideDieParts;

            if (isServer)
            {
                _gameManager.OnGameStarted -= HealthReset;
            }

            _gameManager.ResolvePlayerIndex(_index);
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
            _gameManager.OnGameStarted += HealthReset;
            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager != null) { 
            if(isLocalPlayer)
                _gameManager.SetPlayerManager(this);

            _gameManager.OnGameStartRestarting += HideMainParts;
            _gameManager.OnGameStartRestarting += GameEnd;

            _gameManager.OnGameStarted += GameStart;
            _gameManager.OnGameStarted += ShowMainParts;
            _gameManager.OnGameStarted += HideDieParts;
        }

        if (isLocalPlayer)
        {
            _upperTag.gameObject.SetActive(true);
        }
    }

    public void GameStart()
    {
        if(_isDead is false && _gamesCount != 0)
        {
            _hasCrown = true;
            SetCrown(true);
        }

        _gameStarted = true;
        _gameRestarted = false;
        _isDead = false;

        _gamesCount++;
    }

    public void GameEnd()
    {
        _gameStarted = false;
        _gameRestarted = true;
        _playerAttack.StopWeaponAttack();
    }

    private void SetCrown(bool crownHasable)
    {
        _crownTag.gameObject.SetActive(crownHasable);
    }

    private void SetSprites(int oldIndex, int newIndex)
    {
        _body.sprite = _bodySprites[newIndex % 4];
        foreach (var hand in _hands)
        {
            hand.sprite = _handsSprites[newIndex % 4];
        }
        _upperTag.color = _colors[newIndex % 4];
        foreach (var particle in _particles)
        {
            var mainPart = particle.main;
            mainPart.startColor = _colors[newIndex % 4];
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

        _hasCrown = false;
        SetCrown(false);
    }

    private void HideMainParts()
    {
        foreach (var part in _main)
        {
            part.SetActive(false);
        }

        if (isLocalPlayer)
        {
            _upperTag.gameObject.SetActive(false);
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

        if (isLocalPlayer)
        {
            _upperTag.gameObject.SetActive(true);
        }

        _rigidbody.velocity = Vector3.zero;
        _bodyCollider.enabled = true;

        PlayInstaParts();
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

    private void PlayInstaParts()
    {
        foreach (var part in _instaParts)
        {
            part.GetComponent<ParticleSystem>().Play();
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

    [Command]
    public void CmdResetWeapon()
    {
        _playerAttack.ServerChangeWeapon("Arms");
    }
}
