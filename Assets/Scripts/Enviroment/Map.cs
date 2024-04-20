using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map
{
    [SerializeField] private GameObject _mapObject;
    [SerializeField] private Transform[] _playersSpawnPoints;
    [SerializeField] private Transform[] _weaponsSpawnPoints;

    public GameObject MapObject => _mapObject;
    public Transform[] PlayersSpawnPoints => _playersSpawnPoints;
    public Transform[] WeaponsSpawnPoints => _weaponsSpawnPoints;
}
