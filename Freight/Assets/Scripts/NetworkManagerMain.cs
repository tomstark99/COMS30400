using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class NetworkManagerMain : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs").ToList();

    public List<Player> GamePlayers { get; } = new List<Player>();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    private void Update()
    {
       // Debug.Log(GamePlayers.Count);
    }
}
