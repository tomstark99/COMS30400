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
        SpawnTrees();
        SpawnTrains();
        SpawnBridges();
        SpawnTrackGroups();
        SpawnIndividualTracks();
        SpawnFences();
        photonView.RPC("BuildNavMesh", RpcTarget.All);
        //BuildNavMesh();
        
    }

    [PunRPC]
    void BuildNavMesh()
    {
        GameObject navGameObject = GameObject.FindGameObjectWithTag("TrainStationNavMesh");
        NavMeshSurface surface = navGameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    void SpawnTrees()
    {
        UnityEngine.Random.InitState(seed);
        for (int i = 0; i < 40; i++)
        {
            Vector3 pos;
            pos.x = UnityEngine.Random.Range(200.0f, 265.0f);
            pos.z = UnityEngine.Random.Range(100.0f, 500.0f);
            pos.y = 0.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/tree_basic Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 40; i++)
        {
            Vector3 pos;
            pos.x = UnityEngine.Random.Range(270.0f, 600.0f);
            pos.z = UnityEngine.Random.Range(100.0f, 180.0f);
            if (pos.x > 310.0f && pos.x < 420.0f && pos.z < 180.0f && pos.z > 110.0f)
            {
                pos.x = UnityEngine.Random.Range(420.0f, 600.0f);
            }
            pos.y = 0.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/tree_basic Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 40; i++)
        {
            Vector3 pos;
            pos.x = UnityEngine.Random.Range(270.0f, 600.0f);
            pos.z = UnityEngine.Random.Range(445.0f, 500.0f);
            pos.y = 0.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/tree_basic Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 40; i++)
        {
            Vector3 pos;
            pos.x = UnityEngine.Random.Range(535.0f, 600.0f);
            pos.z = UnityEngine.Random.Range(100.0f, 500.0f);
            pos.y = 0.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/tree_basic Variant", pos, Quaternion.identity);
        }

        Debug.Log("Trees");
    }

    void SpawnTrackGroups()
    {

        for (int i = 0; i < 5; i++)
        {
            float int_gap = gap * i;
            Vector3 pos = new Vector3((325.0f+int_gap), 5.1f, (260.0f-int_gap));

            Vector3 angle_pos = pos;

            angle_pos.x += 0.1935f;
            angle_pos.z -= 15.4675f;

            if (i == 0) {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", angle_pos, Quaternion.Euler(0f, -45f, 0f));
            } else {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", angle_pos, Quaternion.Euler(0f, -45f, 0f));
            }
            

            for (int j = 0; j < 5; j++)
            {
                pos.z += (j == 0) ? 0.0f : 20.0f;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.identity);
            }

            angle_pos = pos;

            angle_pos.x -= 0.1935f;
            angle_pos.z += 15.4675f;

            // If 5th line, connector track needs to be left not right variant
            if (i == 4) {
                pos.z += 15.0f;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.identity);
            } else {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", angle_pos, Quaternion.Euler(0f, 135f, 0f));
            }
        }

        for (int i = 0; i < 6; i++)
        {
            Vector3 pos;
            pos.x = 309.1f;
            pos.y = 5.1f;
            pos.z = 245.9f + i * 20;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            pos.x = 295.0f;
            pos.y = 5.1f;
            pos.z = 270.0f + i * 20;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 5; i++)
        {
            Vector3 pos;
            pos.x = 386.14f;
            pos.y = 5.1f;
            pos.z = 239.0f + i * 20;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            pos.x = 465.86f;
            pos.y = 0.1f;
            pos.z = 248.85f + i * 20;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.identity);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            pos.x = 455.06f - (i * 14.14f);
            pos.y = 0.1f;
            pos.z = 334.91f + (i * 14.14f);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            pos.x = 542.16f + (i * 14.14f);
            pos.y = 5.1f;
            pos.z = 219.7f + (i * 14.14f);

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));
        }

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos;
            pos.x = 456.01f + i * 20;
            pos.y = 5.1f;
            pos.z = 209.0f;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));
        }

        for (int i = 0; i < 2; i++)
        {
            Vector3 pos;
            pos.x = 376.01f + i * 20;
            pos.y = 5.1f;
            pos.z = 209.0f;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));
        }

        for (int i = 0; i < 2; i++)
        {
            Vector3 pos;
            pos.x = 403.87f + i * 5;
            pos.y = 5.1f;
            pos.z = 216.30f;

            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));
        }

        Debug.Log("Tracks");
    }

    void SpawnIndividualTracks()
    {
        Vector3 pos;
        pos.x = 426.01f;
        pos.y = 5.1f;
        pos.z = 209.0f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 346.01f;
        pos.y = 5.1f;
        pos.z = 209.0f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 516.09f;
        pos.y = 5.1f;
        pos.z = 208.9f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 384.89f;
        pos.y = 5.1f;
        pos.z = 405.08f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 358.82f;
        pos.y = 5.1f;
        pos.z = 415.88f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 319.94f;
        pos.y = 5.1f;
        pos.z = 219.79f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 346.65f;
        pos.y = 5.1f;
        pos.z = 415.89f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/short_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 453.57f;
        pos.y = 2.66f;
        pos.z = 221.28f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/short_straight_track_detail Variant", pos, Quaternion.Euler(25f, 45f, 0f));

        pos.x = 302.26f;
        pos.y = 5.1f;
        pos.z = 237.47f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 135f, 0f));

        pos.x = 294.99f;
        pos.y = 5.1f;
        pos.z = 255.01f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 302.27f;
        pos.y = 5.1f;
        pos.z = 352.55f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));

        pos.x = 317.92f;
        pos.y = 5.1f;
        pos.z = 362.37f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 309.15f;
        pos.y = 5.1f;
        pos.z = 360.98f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 294.99f;
        pos.y = 5.1f;
        pos.z = 255.01f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 309.16f;
        pos.y = 5.1f;
        pos.z = 380.98f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 357.0f;
        pos.y = 5.1f;
        pos.z = 340.75f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/90_deg_track_detail Variant", pos, Quaternion.Euler(0f, 225f, 0f));

        pos.x = 389.87f;
        pos.y = 5.1f;
        pos.z = 220.03f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/90_deg_track_detail Variant", pos, Quaternion.Euler(0f, 135f, 0f));

        pos.x = 395.46f;
        pos.y = 4.5f;
        pos.z = 394.51f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_bend_down_10_deg Variant", pos, Quaternion.Euler(-12f, -45f, 0f));

        pos.x = 448.54f;
        pos.y = 4.85f;
        pos.z = 216.25f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_bend_down_10_deg Variant", pos, Quaternion.Euler(8f, 45f, 0f));

        pos.x = 402.11f;
        pos.y = 1.15f;
        pos.z = 387.86f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_bend_up_10_deg Variant", pos, Quaternion.Euler(-17f, -45f, 0f));

        pos.x = 458.61f;
        pos.y = 0.45f;
        pos.z = 226.32f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_bend_up_10_deg Variant", pos, Quaternion.Euler(8.7f, 45f, 0f));

        pos.x = 331.14f;
        pos.y = 5.1f;
        pos.z = 209.97f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -67.5f, 0f));

        pos.x = 464.89f;
        pos.y = 0.1f;
        pos.z = 323.71f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 157.5f, 0f));

        pos.x = 295.97f;
        pos.y = 5.1f;
        pos.z = 344.88f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 22.5f, 0f));

        pos.x = 295.97f;
        pos.y = 5.1f;
        pos.z = 245.14f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -22.5f, 0f));

        pos.x = 310.12f;
        pos.y = 5.1f;
        pos.z = 390.85f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 22.5f, 0f));

        pos.x = 428.75f;
        pos.y = 5.1f;
        pos.z = 215.36f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 112.33f, 0f));

        pos.x = 333.83f;
        pos.y = 5.1f;
        pos.z = 381.29f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -112.5f, 0f));

        pos.x = 324.09f;
        pos.y = 5.1f;
        pos.z = 381.29f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -67.5f, 0f));

        pos.x = 340.72f;
        pos.y = 5.1f;
        pos.z = 388.18f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -157.5f, 0f));

        pos.x = 340.79f;
        pos.y = 5.1f;
        pos.z = 397.64f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -202.5f, 0f));

        pos.x = 308.41f;
        pos.y = 5.1f;
        pos.z = 360.28f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 200.64f, 0f));

        pos.x = 379.7f;
        pos.y = 5.1f;
        pos.z = 342.24f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -21.5f, 0f));

        pos.x = 317.2f;
        pos.y = 5.1f;
        pos.z = 388.18f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -22.5f, 0f));

        pos.x = 385.16f;
        pos.y = 5.1f;
        pos.z = 333.89f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 157.36f, 0f));

        pos.x = 370.89f;
        pos.y = 5.1f;
        pos.z = 345.51f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -112.46f, 0f));

        pos.x = 373.69f;
        pos.y = 5.1f;
        pos.z = 414.91f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, 112.46f, 0f));

        pos.x = 464.95f;
        pos.y = 0.125f;
        pos.z = 233.97f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -158.06f, 0f));

        pos.x = 437.04f;
        pos.y = 5.1f;
        pos.z = 209.88f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, 135f, 0f));

        pos.x = 530.96f;
        pos.y = 5.1f;
        pos.z = 209.87f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_right_track_detail Variant", pos, Quaternion.Euler(0f, -112.5f, 0f));

        pos.x = 595.78f;
        pos.y = 5.1f;
        pos.z = 271.95f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, -90f, 0f));

        pos.x = 361.01f;
        pos.y = 5.1f;
        pos.z = 208.99f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", pos, Quaternion.Euler(0f, -90f, 0f));

        pos.x = 309.33f;
        pos.y = 5.1f;
        pos.z = 230.4f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 378.75f;
        pos.y = 5.1f;
        pos.z = 352.18f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_right_track_detail Variant", pos, Quaternion.Euler(0f, 180f, 0f));

        pos.x = 309.16f;
        pos.y = 5.1f;
        pos.z = 370.98f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.Euler(0f, 180f, 0f));

        pos.x = 418.88f;
        pos.y = 5.1f;
        pos.z = 216.32f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.Euler(0f, -90f, 0f));

        pos.x = 411.01f;
        pos.y = 5.1f;
        pos.z = 208.99f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 441.01f;
        pos.y = 5.1f;
        pos.z = 208.99f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 316.42f;
        pos.y = 5.1f;
        pos.z = 398.52f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_fork_left_track_detail Variant", pos, Quaternion.Euler(0f, -135f, 0f));

        pos.x = 324.15f;
        pos.y = 5.1f;
        pos.z = 404.74f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, -87.5f, 0f));

        pos.x = 333.9f;
        pos.y = 5.1f;
        pos.z = 404.53f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 334.28f;
        pos.y = 5.1f;
        pos.z = 414.92f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, -90f, 0f));

        pos.x = 341.65f;
        pos.y = 5.1f;
        pos.z = 415.89f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/short_straight_track_detail Variant", pos, Quaternion.Euler(0f, 90f, 0f));

        pos.x = 355.31f;
        pos.y = 5.1f;
        pos.z = 437.31f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));

        pos.x = 341.17f;
        pos.y = 5.1f;
        pos.z = 423.27f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));

        pos.x = 327.03f;
        pos.y = 5.1f;
        pos.z = 409.13f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));

        pos.x = 366.11f;
        pos.y = 5.1f;
        pos.z = 463.38f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 366.11f;
        pos.y = 5.1f;
        pos.z = 483.38f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 366.11f;
        pos.y = 5.1f;
        pos.z = 495.88f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/short_straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 365.14f;
        pos.y = 5.1f;
        pos.z = 448.51f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, 45f, 0f));

        pos.x = 364.78f;
        pos.y = 5.1f;
        pos.z = 415.0f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, -45.872f, 0f));

        pos.x = 377.78f;
        pos.y = 5.1f;
        pos.z = 402.05f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/45_deg_left_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 371.48f;
        pos.y = 5.1f;
        pos.z = 409.72f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/straight_track_detail Variant", pos, Quaternion.Euler(0f, -45f, 0f));

        pos.x = 378.75f;
        pos.y = 5.1f;
        pos.z = 387.18f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));

        pos.x = 378.75f;
        pos.y = 5.1f;
        pos.z = 367.18f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Tracks/long_straight_track_detail Variant", pos, Quaternion.Euler(0f, 0f, 0f));


    }

    void SpawnBridges()
    {
        Vector3 pos;
        pos.x = 468.25f;
        pos.y = 4.9f;
        pos.z = 209.0f;

        PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/bridge_flat Variant", pos, Quaternion.Euler(0f, 90f, 0f));
    }

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

    void SpawnFences()
    {
        //Random.seed = (int)System.DateTime.Now.Ticks/(Random.Range(1,50000));
        //int brokenPartPos = Random.Range(10,30);
        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(270.0f, 6.5f, (193.0f + i * 5.0f));

            Vector3 fence_start = new Vector3(position.x - 2.5f, position.y, position.z);
            Vector3 fence_end = new Vector3(position.x + 2.5f, position.y, position.z);
            float height_start = Terrain.activeTerrain.SampleHeight(fence_start);
            float height_end = Terrain.activeTerrain.SampleHeight(fence_end);

            if (i == 10)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_broken_closed Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
            else
            {
                if (height_start > 8.0f || height_end > 8.0f)
                {
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                    position.y += 3;
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                    position.y += 3;
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                }
                else if (height_start > 5.0f || height_end > 5.0f)
                {
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                    position.y += 3;
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                }
                else
                {
                    PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                }
                
            }
        }

        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(500.0f, 6.5f, (193.0f + i * 5.0f));

            Vector3 fence_start = new Vector3(position.x - 2.5f, position.y, position.z);
            Vector3 fence_end = new Vector3(position.x + 2.5f, position.y, position.z);
            float height_start = Terrain.activeTerrain.SampleHeight(fence_start);
            float height_end = Terrain.activeTerrain.SampleHeight(fence_end);

            if (height_start > 8.0f || height_end > 8.0f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
            else if (height_start > 5.0f || height_end > 5.0f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
            else if (!(i > 4 || i < 2))
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 90f, 0f));
            }
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 190.5f);

            Vector3 fence_start = new Vector3(position.x - 2.5f, position.y, position.z);
            Vector3 fence_end = new Vector3(position.x + 2.5f, position.y, position.z);
            float height_start = Terrain.activeTerrain.SampleHeight(fence_start);
            float height_end = Terrain.activeTerrain.SampleHeight(fence_end);

            if (height_start < 4.99f || height_end < 4.99f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
            else
            { 
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));   
            }

        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 440.5f);

            Vector3 fence_start = new Vector3(position.x - 2.5f, position.y, position.z);
            Vector3 fence_end = new Vector3(position.x + 2.5f, position.y, position.z);
            float height_start = Terrain.activeTerrain.SampleHeight(fence_start);
            float height_end = Terrain.activeTerrain.SampleHeight(fence_end);

            if (height_start < 4.99f || height_end < 4.99f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
            else if (height_start > 8.0f || height_end > 8.0f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
            else if (height_start > 5.0f || height_end > 5.0f)
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple_bottom Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
                position.y += 3;
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
            else if (!(i > 15 && i < 20))
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/fence_simple Variant 1", position, Quaternion.Euler(0f, 0f, 0f));
            }
        }
        Debug.Log("Fences");
    }
}
