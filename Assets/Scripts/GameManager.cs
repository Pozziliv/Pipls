using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private FloorWeapon[] _floorWeapons;

    [SerializeField] private Map _lobbyMap;

    [SerializeField] private Map[] _maps;
    private Map _chosenMap;
    private int _mapIndex = 0;

    [SerializeField] private int _weaponCount = 5;
    private List<ISpawnable> _spawnedWeapons = new List<ISpawnable>();

    [SyncVar] private int _playerIndex = 0;

    private void Start()
    {
        _lobbyMap.MapObject.SetActive(false);
        /*foreach (var map in _maps)
        {
            map.MapObject.SetActive(false);
        }

        if(_chosenMap != null)
        {
            _chosenMap.MapObject.SetActive(true);
        }
        else
        {
            _lobbyMap.MapObject.SetActive(true);
        }*/
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _mapIndex = 0;
    }

    public int SetPlayerIndex()
    {
        return _playerIndex++;
    }

    public void DropWeapon(int weaponIndex, Vector2 startPoint, float angle, NetworkConnectionToClient connectionToClient)
    {
        var chosedWeapon = _floorWeapons[weaponIndex - 1]; // 0 - arms
        var spawnedWeapon = Instantiate(chosedWeapon, startPoint, Quaternion.identity);
        spawnedWeapon.SetDirection(angle);
        NetworkServer.Spawn(spawnedWeapon.gameObject);
        spawnedWeapon.SetSender(connectionToClient);
        spawnedWeapon.StartFly(angle);
    }

    public void RestartRound()
    {
        ChooseMap();
        RpcChangeMap(_chosenMap);
        SpawnWeapons();
    }

    private void ChooseMap()
    {
        _chosenMap = _maps[_mapIndex++ % _maps.Count()];
    }

    private void RpcChangeMap(Map map)
    {

    }

    private void SpawnWeapons()
    {

    }

    public int GetPlayersCount()
    {
        return _playerIndex;
    }
}
