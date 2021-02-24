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

    public readonly List<Player> GamePlayers = new List<Player>();

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

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach(Player playerObject in GamePlayers)
        {
            if (conn.clientOwnedObjects.Contains(playerObject.netIdentity))
            {
                GamePlayers.Remove(playerObject);
                break; //should have only one such Player object
            }
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        foreach (NetworkIdentity netId in conn.clientOwnedObjects)
        {
            if (netId.gameObject.GetComponent<Player>() != null)
            {
                GamePlayers.Add(netId.gameObject.GetComponent<Player>());
                break;
            }
        }
    }

}
