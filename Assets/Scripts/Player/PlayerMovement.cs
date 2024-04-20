using Mirror;
using System.Collections;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private float _knockbackForce = 1;
    
    private bool _isKnockBack = false;
    private float _knockBackTime = 0f;
    [SerializeField] private float _knockBackTimer = 0.3f;

    [SerializeField] private PlayerManager _playerManager;

    [Header("Dash")]
    [SerializeField] private float _dashForce = 4f;
    [SerializeField] private float _dashTime = 0.5f;
    [SerializeField] private float _dashCooldown = 5f;
    [SerializeField] private SpriteRenderer _dashIndicator;
    [SerializeField] private ParticleSystem _dashParticles;

    private Vector2 _input;

    private bool _canDash = true;
    [SyncVar(hook = nameof(SetDashParticles))] private bool _inDash = false;

    private void Start()
    {
        var particlesMain = _dashParticles.main;
        particlesMain.duration = _dashTime;
    }

    private void Update()
    {
        if (_playerManager.IsDead == true || _playerManager.GameStarted == false || _inDash) return;

        _input = (Vector2.up * Input.GetAxisRaw("Vertical") + Vector2.right * Input.GetAxisRaw("Horizontal")).normalized;

        if (Input.GetKey(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(Dash(_input, transform.rotation));
        }
    }

    void FixedUpdate()
    {
        if(isLocalPlayer == false) return;

        if(_playerManager.IsDead == true || _playerManager.GameStarted == false || _inDash) return;

        if (_isKnockBack == true)
        {
            if (_knockBackTime >= _knockBackTimer)
            {
                _isKnockBack = false;
            }
            _knockBackTime += Time.fixedDeltaTime;
            return;
        }

        _rigidbody.velocity = _input * _speed;
    }

    [TargetRpc]
    public void TargetKnockBack(Vector3 diff)
    {
        if (_isKnockBack == true) return;

        _rigidbody.AddForce(diff * _knockbackForce, ForceMode2D.Impulse);
        _isKnockBack = true;
        _knockBackTime = 0;
    }

    private IEnumerator Dash(Vector2 direction, Quaternion playerRotation)
    {
        _canDash = false;
        _inDash = true;

        SetDashParticles(false, true);

        _playerManager.CmdSetInvulnerability(true);

        _rigidbody.velocity = (direction == Vector2.zero ? (Vector2)(playerRotation * Vector2.right) : direction) * _dashForce;

        yield return new WaitForSeconds(_dashTime);

        _inDash = false;

        SetDashParticles(true, false);

        _playerManager.CmdSetInvulnerability(false);

        _dashIndicator.enabled = true;

        float timer = 0f;
        float fillPartInSecond = 360 / _dashCooldown;

        while(timer < _dashCooldown)
        {
            _dashIndicator.sharedMaterial.SetFloat("_Arc2", fillPartInSecond * timer);
            _dashIndicator.transform.eulerAngles = Vector3.zero;
            timer += Time.deltaTime;
            yield return null;
        }

        _dashIndicator.enabled = false;

        _canDash = true;
    }

    private void SetDashParticles(bool oldBool, bool newBool)
    {
        if(newBool == true)
        {
            _dashParticles.Play();
        }
    }
}
