using Mirror;
using System.Collections;
using UnityEngine;

public class StartZone : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpriteRenderer _backgroundSprite;
    [SerializeField] private AnimationCurve _fillCurve;
    [SerializeField] private float _fillTime = 3f;

    private float _process = 0f;

    private Material _material;

    private int _playersCount;
    private int _playersInZone;

    private bool _activated = false;

    private void Start()
    {
        _material = _backgroundSprite.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _playersCount = _gameManager.GetPlayersCount();

        _playersInZone += 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _playersInZone -= 1;

        _material.SetFloat("_Arc2", 360);

        _process = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_playersCount == _playersInZone && _fillTime > _process)
        {
            _material.SetFloat("_Arc2", 360 - _fillCurve.Evaluate(_process / _fillTime) * 360);

            _process += Time.deltaTime;
        }

        if(_process > _fillTime && _activated == false)
        {
            _gameManager.RestartRound();
            _activated = true;
        }
    }
}
