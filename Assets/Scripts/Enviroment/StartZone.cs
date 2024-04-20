using Mirror;
using System.Collections;
using UnityEngine;

public class StartZone : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpriteRenderer _backgroundSprite;
    [SerializeField] private AnimationCurve _fillCurve;
    [SerializeField] private float _fillTime = 3f;

    private float process = 0f;

    private Material _material;

    private int _playersCount;
    private int _playersInZone;

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

        _material.SetFloat("_Arc2", 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_playersCount == _playersInZone && _fillTime > process)
        {
            _material.SetFloat("_Arc2", 360 - _fillCurve.Evaluate(process / _fillTime) * 360);

            process += Time.deltaTime;
        }

        if(process > _fillTime)
        {
            _gameManager.RestartRound();
        }
    }
}
