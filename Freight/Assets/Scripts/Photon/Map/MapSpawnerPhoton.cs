using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Photon.Pun;
using System.IO;

public class MapSpawnerPhoton : MonoBehaviourPun
{
    // variables for scene creation
    int seed;
    public GameObject tree;

    // variables for spawning trains
    const float gap = 7.07f;
    private Vector3 positionStart = new Vector3(325.0f, 5.1f, 260.0f);
    private Vector3 position = new Vector3(325.0f, 5.1f, 260.0f);
    private const int instantiations = 11;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPrefabs();
        }
    }

    public void SpawnPrefabs()
    {
        seed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        
        SpawnTrains();
        SpawnBrokenFence();
        //SpawnBushes();
        // SpawnBridges();
        
        //BuildNavMesh();
        photonView.RPC("BuildNavMesh", RpcTarget.All);
        
    }

    [PunRPC]
    void BuildNavMesh()
    {
        GameObject navGameObject = GameObject.FindGameObjectWithTag("TrainStationNavMesh");
        NavMeshSurface surface = navGameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    void SpawnBushes()
    {
        for (int i = 0; i < 100; i++)
        {
            Debug.Log("bush " + i);
            Vector3 pos = Vector3.zero;
            bool validPos = false;

            while (!validPos)
            {
                pos.x = UnityEngine.Random.Range(200.0f, 600.0f);
                pos.z = UnityEngine.Random.Range(100.0f, 500.0f);
                pos.y = 0.0f;
                pos.y = Terrain.activeTerrain.SampleHeight(pos);

                validPos = true;
                Collider[] colliders = Physics.OverlapSphere(pos, 5.0f);

                foreach (Collider col in colliders)
                {
                    if (col.tag == "Track" || col.tag == "Building" || col.tag == "Fence" || col.tag == "Tree" || col.tag == "Water")
                    {
                        Debug.Log("overlap");
                        validPos = false;
                    }
                }
            }

            if (validPos)
            {
                float y_rot = UnityEngine.Random.Range(0.0f, 360.0f);

                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/bush_basic Variant", pos, Quaternion.Euler(0f, y_rot, 0f));
            }
        }

        Debug.Log("bushes");
    }

    // void SpawnBridges()
    // {
    //     Vector3 pos;
    //     pos.x = 468.25f;
    //     pos.y = 4.9f;
    //     pos.z = 209.0f;

    //     PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/bridge_flat Variant", pos, Quaternion.Euler(0f, 90f, 0f));
    // }

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
        //GameObject freight_train_coal_loc = PhotonNetwork.Instantiate("PhotonPrefabs/freight_train_coal_ladder Variant 1", new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        //Debug.Log("loca" + freight_train_coal_loc);
        //GameObject track = GameObject.FindWithTag(clearTrack.ToString());
       // Debug.Log("track" + track);
        //freight_train_coal_loc.GetComponent<SplineWalkerPhoton>().spline = track.GetComponent<BezierSpline>();

        for (int j = 0; j < 5; j++)
        {
            bool instantiatedTrainWithTag = false;

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
                        //GameObject trainGo;
                        if (instantiatedTrainWithTag == false)
                        {
                            //trainGo = Instantiate(trainWithTag, position, Quaternion.Euler(0f, 0f, 0f));
                            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Trains/freight_train_stationary Variant with tag 1", position, Quaternion.Euler(0f, 0f, 0f));
                            //Debug.Log(trainGo.transform.Find("TrainNumber").gameObject.transform.GetChild(0));
                            //trainGo.transform.Find("trainNumber").gameObject.transform.GetChild(0);
                            instantiatedTrainWithTag = true;
                            //Debug.Log(trainGo.transform.Find("TrainNumber").gameObject.transform.GetChild(0));
                            if (j == 4)
                            {
                                //trainGo.transform.Find("TrainNumber").gameObject.transform.GetChild(0).transform.GetComponent<TextMeshPro>().text = ("Train number " + "5");

                            }
                            else
                            {
                                //trainGo.transform.Find("TrainNumber").gameObject.transform.GetChild(0).transform.GetComponent<TextMeshPro>().text = ("Train number " + j.ToString());

                            }
                            Debug.Log("burst train");
                        }
                        else
                        {
                            Debug.Log("got to the normal train");
                            //trainGo = Instantiate(train, position, Quaternion.Euler(0f, 0f, 0f));
                            int ran = UnityEngine.Random.Range(0, 3);
                            Debug.Log(ran);
                            if (ran == 1)
                            {
                                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Trains/freight_train_stationary Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                            }
                            else if (ran == 2)
                            {
                                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Trains/freight_train_stationary_1 Variant", position, Quaternion.Euler(0f, 0f, 0f));
                            }
                            else
                            {
                                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Trains/freight_train_stationary_2 Variant", position, Quaternion.Euler(0f, 0f, 0f));
                            }
                        }
                        //NetworkServer.Spawn(trainGo);
                    }
                    position.z += 8.15f;
                }
            }
            else
            {
                //freight_train_coal_loc.transform.localPosition = track.GetComponent<BezierSpline>().GetPoint(0.0f);
                GameObject track = GameObject.FindWithTag(clearTrack.ToString());
                GameObject freight_train_coal_loc = PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Trains/freight_train_coal_ladder Variant 1", track.GetComponent<BezierSpline>().GetPoint(0.0f), Quaternion.Euler(0f, 0f, 0f));
                
                freight_train_coal_loc.GetComponent<SplineWalkerPhoton>().spline = track.GetComponent<BezierSpline>();
                //NetworkServer.Spawn(freight_train_coal_loc);
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

    void SpawnBrokenFence() {
        int brokenPartPos = UnityEngine.Random.Range(0,5);
        Vector3 position = new Vector3(277.42f, 6.5f, 257.82f);
        for (int i = 0; i < 5; i++)
        {
            if (i == brokenPartPos) {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_broken_closed Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            } else {
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
            position.z += 5.0f;
        }
    }

}
