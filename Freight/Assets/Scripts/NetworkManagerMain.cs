using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NetworkManagerMain : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

    public override void OnStartServer()
    {
        //spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs/ModelPrefabs").ToList();
    }

    public List<Player> GamePlayers { get; } = new List<Player>();

    public override void OnStartClient()
    {
        //var spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs/ModelPrefabs");
        //foreach (var prefab in spawnablePrefabs)
        //{
        //    Debug.Log(prefab);
        //    ClientScene.RegisterPrefab(prefab);
        //}
    }

    private void Update()
    {
    
    }
}
