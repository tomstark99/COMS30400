using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class MapSpawner : NetworkBehaviour
{
    // variables for scene creation
    public NavMeshSurface surface;
    public GameObject tree;
    public GameObject train;
    public GameObject trainMoving;
    public GameObject trainLadder;
    public GameObject fence;
    public GameObject bottomFence;
    public GameObject fenceBrokenPartial;
    int seed;

    // variables for spawning trains
    const float gap = 7.075f;
    private Vector3 positionStart = new Vector3(325.0f, 5.1f, 260.0f);
    private Vector3 position = new Vector3(325.0f, 5.1f, 260.0f);
    private const int instantiations = 12;

    public override void OnStartServer()
    {
        SpawnPrefabs();
    }

    [Server]
    public void SpawnPrefabs()
    {
        seed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        SpawnTrees();
        SpawnTrains();
        SpawnFences();
        BuildNavMesh();
    }

    [Server]
    void BuildNavMesh()
    {
        GameObject navGameObject = GameObject.FindGameObjectWithTag("TrainStationNavMesh");
        surface = navGameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

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
        /*UnityEngine.Random.InitState(seed);
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

        }*/

        int clearTrack = UnityEngine.Random.Range(0, 5);
        Debug.Log("skipped track: " + clearTrack);
        GameObject freight_train_coal_loc = GameObject.FindWithTag("locomotive");
        Debug.Log("loca" + freight_train_coal_loc);
        GameObject track = GameObject.FindWithTag(clearTrack.ToString());
        Debug.Log("track" + track);
        freight_train_coal_loc.GetComponent<SplineWalker>().spline = track.GetComponent<BezierSpline>();
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
            } else {
                freight_train_coal_loc.transform.localPosition = track.GetComponent<BezierSpline>().GetPoint(0.0f);
            }
            /*else {
                bool ladderPlaced = false;
                for (int i = 0; i < instantiations; i++){
                    if (Random.Range(1,5) == 3 && ladderPlaced == false)
                    {
                        Instantiate(trainLadder, position, Quaternion.Euler(0f, 0f, 0f));
                        position.z += 8.2f;
                        ladderPlaced = true;
                    }
                    else
                    {
                        Instantiate(trainMoving, position, Quaternion.Euler(0f, 0f, 0f));
                        position.z += 8.2f;
                    }
                }
            }*/

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

    void SpawnFences()
    {
        //Random.seed = (int)System.DateTime.Now.Ticks/(Random.Range(1,50000));
        //int brokenPartPos = Random.Range(10,30);
        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(270.0f, 6.5f, (193.0f + i * 5.0f));
            if (i == 10)
            {
                GameObject fenceGo = Instantiate(fenceBrokenPartial, position, Quaternion.Euler(0f, 90f, 0f));
                NetworkServer.Spawn(fenceGo);
            }
            else
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
                NetworkServer.Spawn(fenceGo);
            }
        }

        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(500.0f, 6.5f, (193.0f + i * 5.0f));
            if (i > 4 || i < 2)
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
                NetworkServer.Spawn(fenceGo);
            }
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 190.5f);
            if (i > 36 && i < 42)
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo);
                position.y -= 3;
                GameObject fenceGo2 = Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo2);
                position.y -= 3;
                GameObject fenceGo3 = Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo3);
            }
            else
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo);
            }

        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 440.5f);
            if (i > 27 && i < 34)
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo);
                position.y -= 3;
                GameObject fenceGo2 = Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo2);
                position.y -= 3;
                GameObject fenceGo3 = Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo3);
            }
            else
            {
                GameObject fenceGo = Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                NetworkServer.Spawn(fenceGo);
            }
        }
        Debug.Log("Fences");
    }
}
