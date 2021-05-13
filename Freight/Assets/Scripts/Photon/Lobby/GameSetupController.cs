﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class GameSetupController : MonoBehaviourPunCallbacks
{
    /*
     * All this script is doing in the end is creating the player object. The game is told to find this player object under a 
     * PhotonPrefabs folder and to look for the prefab named PhotonPlayer.
     * Everything is setting the starting position and rotation values.
     * */

    [SerializeField]
    private GameObject guardGameObject;
    [SerializeField]
    private GameObject bagSpawnerGameObject;
    [SerializeField]
    private GameObject environmentGameObject;
    [SerializeField]
    private GameObject trainsGameObject;

    private bool spawnCalled = false;

    private GameTracker gameTracker;

    void Start()
    {
        // https://forum.photonengine.com/discussion/7805/received-onserialization-for-view-id-xxxx-we-have-no-such-photon-view
        //photonView.RPC(nameof(PlayerLoaded), RpcTarget.AllBuffered);
        gameTracker = GameObject.FindGameObjectWithTag("GameTracker").GetComponent<GameTracker>();

        gameTracker.PlayerLoadedFirstLevel();
        //Invoke(nameof(SpawnPlayers), 5f);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!spawnCalled && gameTracker.PlayerCountFirst >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            spawnCalled = true;
            Invoke(nameof(SpawnPlayers), 3f);
        }
    }
   

    void SpawnPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void CreatePlayer()
    {
        int z = Random.Range(294,303);
        PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna", new Vector3(254, 10, z), Quaternion.identity);
        guardGameObject.SetActive(true);
        bagSpawnerGameObject.SetActive(true);
        environmentGameObject.SetActive(true);
        trainsGameObject.SetActive(true);

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient != null)
            PhotonNetwork.LoadLevel(0);
    }
}
