using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Map _lobbyMap;

    [SerializeField] private List<Map> _maps;
    private Map _chosenMap;
    [SyncVar] private int _mapIndex = -1;

    private PlayerManager _pinnedPlayerManager;

    private Camera _camera;
    [Header("Transition")]
    [SerializeField] private float _transitionTime = 3f;
    [SerializeField] private float _cameraRollTime = 2f;
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private Transform[] _earthTransitionPoints;
    [SerializeField] private AnimationCurve _earthTransitionCurve;
    [SerializeField] private SpriteRenderer _whitePanel;
    [SerializeField] private GameObject _mapParent;

    [Header("Weapon Spawn")]
    [SerializeField] private FloorWeapon[] _floorWeapons;
    [SerializeField] private int _weaponCount = 5;
    private List<ISpawnable> _spawnedWeapons = new List<ISpawnable>();
    [SerializeField] private InstaParticles[] _instaParticles;

    private bool[] _playerIndex = { false, false, false, false };

    private int _diedPlayers;

    public Action OnGameStarted;
    public Action OnGameStartRestarting;



    private void Start()
    {
        _camera = Camera.main;

        StartCoroutine(UpscaleToFight());

        _lobbyMap.MapObject.SetActive(false);
        foreach (var map in _maps)
        {
            map.MapObject.SetActive(false);
        }

        if(_mapIndex != -1)
        {
            _chosenMap = _maps[_mapIndex];
            _chosenMap.MapObject.SetActive(true);

            StartCoroutine(GmaeStartForPlayerWhoGetInGameThenGameAlreadyWasPlayed());
        }
        else
        {
            _lobbyMap.MapObject.SetActive(true);
            _chosenMap = _lobbyMap;
        }
    }

    private IEnumerator GmaeStartForPlayerWhoGetInGameThenGameAlreadyWasPlayed()
    {
        yield return new WaitForSeconds(1f);

        OnGameStarted.Invoke();
    }

    private IEnumerator UpscaleToFight()
    {
        float progress = 0f;

        Color whiteTransparent = new Color(1, 1, 1, 0);

        float cameraStartSize = _camera.orthographicSize;

        while (progress <= 1f)
        {
            _camera.orthographicSize = Mathf.Lerp(cameraStartSize + 450f, cameraStartSize, _earthTransitionCurve.Evaluate(progress));
            _whitePanel.color = Color.Lerp(Color.white, whiteTransparent, _earthTransitionCurve.Evaluate(progress));

            progress += Time.deltaTime / _cameraRollTime;

            yield return null;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _mapIndex = -1;
    }

    public int SetPlayerIndex()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_playerIndex[i] is false)
            {
                _playerIndex[i] = true;
                return i;
            }
        }
        return -1;
    }

    public void ResolvePlayerIndex(int index)
    {
        _playerIndex[index] = false;
    }

    public void SetPlayerManager(PlayerManager manager)
    {
        _pinnedPlayerManager = manager;
    }

    public void DropWeapon(int weaponIndex, Vector2 startPoint, float angle, NetworkConnectionToClient connectionToClient)
    {
        var chosedWeapon = _floorWeapons[weaponIndex - 1]; // 0 - arms
        var spawnedWeapon = Instantiate(chosedWeapon, startPoint, Quaternion.identity);
        spawnedWeapon.SetDirection(angle);
        NetworkServer.Spawn(spawnedWeapon.gameObject);
        spawnedWeapon.SetSender(connectionToClient);
        spawnedWeapon.StartFly(angle);

        _spawnedWeapons.Add(spawnedWeapon);
    }

    [Server]
    public void RestartRound()
    {
        _diedPlayers = 0;

        foreach (var weapon in _spawnedWeapons)
        {
            if (weapon == null) continue;

            NetworkServer.Destroy(weapon.ThisGO);
            Destroy(weapon.ThisGO);
        }

        StartCoroutine(RoundRestarting());
    }

    private void ChooseNextMap()
    {
        _chosenMap.MapObject.SetActive(false);
        _chosenMap = _maps[++_mapIndex % _maps.Count()];
        _chosenMap.MapObject.SetActive(true);
    }

    [Server]
    private IEnumerator RoundRestarting()
    {
        Vector2 battlePointOnEarth = _earthTransitionPoints[UnityEngine.Random.Range(0, _earthTransitionPoints.Length)].position;

        if(_chosenMap != _lobbyMap)
        {
            yield return new WaitForSeconds(2f);
        }

        RpcTranslateMap(battlePointOnEarth);
        yield return new WaitForSecondsRealtime(_cameraRollTime);

        ChooseNextMap();

        RpcChangeMap(_maps.IndexOf(_chosenMap));

        RpcTranslatePlayers();

        yield return new WaitForSecondsRealtime(_cameraRollTime + _transitionTime);

        SpawnWeapons();

        OnGameStarted.Invoke();
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcTranslatePlayers()
    {
        _pinnedPlayerManager.transform.position = _chosenMap.PlayersSpawnPoints[_pinnedPlayerManager.Index].position;
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        _pinnedPlayerManager.CmdResetWeapon();
        OnGameStarted.Invoke();
    }

    [ClientRpc]
    private void RpcChangeMap(int newMapIndex)
    {
        _chosenMap.MapObject.SetActive(false);
        _chosenMap = _maps[newMapIndex];
        _chosenMap.MapObject.SetActive(true);
    }

    [ClientRpc]
    private void RpcTranslateMap(Vector2 transitionEndPoint)
    {
        StartCoroutine(MapChangingOnEarth(transitionEndPoint));
    }

    private IEnumerator MapChangingOnEarth(Vector2 transitionEndPoint)
    {
        float progress = 0f;

        Color whiteTransparent = new Color(1, 1, 1, 0);

        float cameraStartSize = _camera.orthographicSize;

        while (progress <= 1f)
        {
            _camera.orthographicSize = Mathf.Lerp(cameraStartSize, cameraStartSize+450f, _earthTransitionCurve.Evaluate(progress));
            _whitePanel.color = Color.Lerp(whiteTransparent, Color.white, _earthTransitionCurve.Evaluate(progress));

            progress += Time.deltaTime / _cameraRollTime;

            yield return null;
        }

        _mapParent.SetActive(false);

        OnGameStartRestarting.Invoke();

        progress = 0f;
        
        Vector2 startPosition = _earthTransform.position;

        while (progress <= 1f)
        {
            _earthTransform.position = Vector2.Lerp(startPosition, -transitionEndPoint, _earthTransitionCurve.Evaluate(progress));

            progress += Time.deltaTime / _transitionTime;

            yield return null;
        }

        _mapParent.SetActive(true);

        progress = 0f;

        while(progress <= 1f)
        {
            _camera.orthographicSize = Mathf.Lerp(cameraStartSize+450f, cameraStartSize, _earthTransitionCurve.Evaluate(progress));
            _whitePanel.color = Color.Lerp(Color.white, whiteTransparent, _earthTransitionCurve.Evaluate(progress));

            progress += Time.deltaTime / _cameraRollTime;

            yield return null;
        }
    }

    private void SpawnWeapons()
    {
        foreach (var weapon in _spawnedWeapons)
        {
            if (weapon == null) continue;

            NetworkServer.Destroy(weapon.ThisGO);
            Destroy(weapon.ThisGO);
        }


        List<Transform> chosenWeaponSpawnPoints = new List<Transform>();

        int index = 0;

        for(int i = 0; i < _weaponCount; i++)
        {
            FloorWeapon chosenWeapon = _floorWeapons[UnityEngine.Random.Range(0, _floorWeapons.Length)];

            Transform chosenSpawnPoint = GetSpawnPoint(ref chosenWeaponSpawnPoints, _chosenMap.WeaponsSpawnPoints);

            FloorWeapon spawnedWeapon = Instantiate(chosenWeapon, chosenSpawnPoint.position, Quaternion.identity);

            NetworkServer.Spawn(spawnedWeapon.gameObject);

            RpcPlayInstaParts(index, chosenSpawnPoint.position);

            _spawnedWeapons.Add(spawnedWeapon);

            if(spawnedWeapon.Ammo != null)
            {
                chosenSpawnPoint = GetSpawnPoint(ref chosenWeaponSpawnPoints, _chosenMap.WeaponsSpawnPoints);

                GameObject ammoGameObject = Instantiate(spawnedWeapon.Ammo, chosenSpawnPoint.position, Quaternion.identity);
                NetworkServer.Spawn(ammoGameObject);

                _spawnedWeapons.Add(ammoGameObject.GetComponent<ISpawnable>());
            }
        }
    }

    public void AddSpawnedThing(ISpawnable spawnedThing)
    {
        _spawnedWeapons.Add(spawnedThing);
    }

    [ClientRpc]
    private void RpcPlayInstaParts(int index, Vector3 position)
    {
        _instaParticles[index].transform.position = position;
        _instaParticles[index].PlayParticles();
    }

    private Transform GetSpawnPoint(ref List<Transform> choosedTransforms, Transform[] allTransforms)
    {
        bool spawnPointGetted = false;
        Transform chosenSpawnPoint = transform;

        while (spawnPointGetted == false)
        {
            chosenSpawnPoint = allTransforms[UnityEngine.Random.Range(0, allTransforms.Length)];

            if (choosedTransforms.Contains(chosenSpawnPoint) == false)
            {
                choosedTransforms.Add(chosenSpawnPoint);
                spawnPointGetted = true;
            }
        }

        return chosenSpawnPoint;
    }

    public int GetPlayersCount()
    {
        return NetworkManager.singleton.numPlayers;
    }

    public void PlayerDie()
    {
        _diedPlayers++;

        int playersCount = NetworkManager.singleton.numPlayers;

        if(playersCount - _diedPlayers == 1)
        {
            RestartRound();
        }
    }
}
