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
    public GameObject gameOverScreen;

    public static event Action<NetworkConnection> OnServerReadied;

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

    public void EndGame()
    {
        Debug.Log("game over");
        ServerChangeScene("Assets/Scenes/TrainStation.unity");
    }

    public override void ServerChangeScene(string newSceneName)
    {
        //if (newSceneName == "Assets/Scenes/TrainStationGuards.unity")
        //{
        //    for (int i = GamePlayers.Count - 1; i >= 0; i--)
        //    {
        //        var conn = GamePlayers[i].connectionToClient;
        //        var gameplayerInstance = Instantiate(playerPrefab);

        //        GamePlayers.Remove(GamePlayers[i]);

        //        NetworkServer.Destroy(conn.identity.gameObject);

        //        NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance, true);
        //    }
        //}

        GamePlayers.Clear();

        base.ServerChangeScene(newSceneName);
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

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}
