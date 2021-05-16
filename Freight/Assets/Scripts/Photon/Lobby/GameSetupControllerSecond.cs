using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class GameSetupControllerSecond : MonoBehaviourPunCallbacks
{
    /*
     * All this script is doing in the end is creating the player object. The game is told to find this player object under a 
     * PhotonPrefabs folder and to look for the prefab named PhotonPlayer.
     * Everything is setting the starting position and rotation values.
     * */

    private bool spawnCalled = false;

    private GameTracker gameTracker;

    [SerializeField]
    private GameObject guardGameObject;

    void Start()
    {
        // https://forum.photonengine.com/discussion/7805/received-onserialization-for-view-id-xxxx-we-have-no-such-photon-view
        //Invoke(nameof(SpawnPlayers), 3f);

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
        if (PhotonNetwork.IsMasterClient) 
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void CreatePlayer()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna2", new Vector3(141f, 3.2f, 106f), Quaternion.identity);
        else
            PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna2", new Vector3(141f, 3.2f, 115f), Quaternion.identity);

        guardGameObject.SetActive(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient != null)
            PhotonNetwork.LoadLevel(0);
    }
}
