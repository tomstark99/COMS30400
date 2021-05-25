using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

/*
    This class is on the second level, it uses GameTracker to see when both players load into the level and then spawns them in as well as setting all game objects with photon views on them to active
*/
public class GameSetupControllerSecond : MonoBehaviourPunCallbacks
{
    private bool spawnCalled = false;

    private GameTracker gameTracker;

    [SerializeField]
    private GameObject guardGameObject;

    void Start()
    {
        // https://forum.photonengine.com/discussion/7805/received-onserialization-for-view-id-xxxx-we-have-no-such-photon-view

        // finds the GameTracker object in the scene
        gameTracker = GameObject.FindGameObjectWithTag("GameTracker").GetComponent<GameTracker>();

        // Start method will only be called once scene is loaded, thus each player sends an RPC to confirm that they have loaded the scene and are ready to be spawned in
        gameTracker.PlayerLoadedSecondLevel();
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // master client checks GameTracker class for count of players spawned in and only spawns them in once it is equal to the amount of players in room
        if (!spawnCalled && gameTracker.PlayerCountSecond >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            spawnCalled = true;
            Invoke(nameof(SpawnPlayers), 4f);
        }
    }

    void SpawnPlayers()
    {
        // RPC sent to each player to spawn themselves into the level
        if (PhotonNetwork.IsMasterClient) 
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void CreatePlayer()
    {
        // spawn players in different locations, master client spawns in the first carriage while the other player spawns in the second carriage
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna2", new Vector3(141f, 3.2f, 106f), Quaternion.identity);
        else
            PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna2", new Vector3(141f, 3.2f, 115f), Quaternion.identity);

        // set guards to active
        guardGameObject.SetActive(true);
    }

    // if the master client leaves the game, we return back to lobby
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient != null)
            PhotonNetwork.LoadLevel(0);
    }
}
