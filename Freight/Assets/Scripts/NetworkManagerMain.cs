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
    public NavMeshSurface surface;
    public GameObject tree;
    public GameObject train;
    int seed;

    // variables for spawning trains
    const float gap = 7.075f;
    private Vector3 positionStart = new Vector3(325.0f, 5.1f, 260.0f);
    private Vector3 position = new Vector3(325.0f, 5.1f, 260.0f);
    private const int instantiations = 12;

    [Server]
    void SpawnTrees()
    {
        UnityEngine.Random.InitState(seed);
        for (int i = 0; i < 25; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(250.0f, 270.0f), 5, UnityEngine.Random.Range(200.0f, 420.0f));
            GameObject treeGo = Instantiate(tree, position, Quaternion.identity);
            NetworkServer.Spawn(treeGo);
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(275.0f, 450.0f), 5, UnityEngine.Random.Range(185.0f, 165.0f));
            GameObject treeGo = Instantiate(tree, position, Quaternion.identity);
            NetworkServer.Spawn(treeGo);
        }

        for (int i = 0; i < 25; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(510.0f, 530.0f), 5, UnityEngine.Random.Range(200.0f, 400.0f));
            GameObject treeGo = Instantiate(tree, position, Quaternion.identity);
            NetworkServer.Spawn(treeGo);
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(275.0f, 450.0f), 5, UnityEngine.Random.Range(445.0f, 465.0f));
            GameObject treeGo = Instantiate(tree, position, Quaternion.identity);
            NetworkServer.Spawn(treeGo);
        }

        Debug.Log("Trees");
    }
    
    [Server]
    void SpawnTrains()
    {
        UnityEngine.Random.InitState(seed);
        int clearTrack = UnityEngine.Random.Range(0, 5);
        Debug.Log("skipped track: " + clearTrack);
        for (int j = 0; j < 5; j++)
        {

            position = positionStart;
            position.x += (j * gap);
            position.z -= (j * gap);

            if (!(clearTrack == j))
            {

                int gaps = UnityEngine.Random.Range(0, 6);
                int[] positions = new int[gaps];
                for (int i = 0; i < gaps; i++)
                {
                    positions[i] = UnityEngine.Random.Range(0, instantiations);
                }
                for (int i = 0; i < instantiations; i++)
                {
                    if (!inSkip(i, positions, gaps))
                    {
                        GameObject trainGo = Instantiate(train, position, Quaternion.Euler(0f, 0f, 0f));
                        NetworkServer.Spawn(trainGo);
                    }
                    position.z += 8.15f;
                }
            }

        }
        Debug.Log("Trains");
    }

    bool inSkip(int i, int[] positions, int gaps)
    {
        for (int j = 0; j < gaps; j++)
        {
            if (i == positions[j]) return true;
        }
        return false;
    }

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs/ModelPrefabs").ToList();
        seed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        SpawnTrees();
        SpawnTrains();
        surface.BuildNavMesh();
    }

    public List<Player> GamePlayers { get; } = new List<Player>();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs/ModelPrefabs");
        UnityEngine.Random.InitState(seed);
        SpawnTrees();
        SpawnTrains();
        foreach (var prefab in spawnablePrefabs)
        {
            Debug.Log(prefab);
            ClientScene.RegisterPrefab(prefab);
        }
    }

    private void Update()
    {
        //Debug.Log(GamePlayers.Count);
    }
}
