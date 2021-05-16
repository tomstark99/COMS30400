using System.Collections;
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
    [SerializeField]
    private GameObject cameraObject;
    [SerializeField]
    private GameObject spotlightObjects;

    private bool spawnCalled = false;

    private GameTracker gameTracker;

    void Start()
    {
        // https://forum.photonengine.com/discussion/7805/received-onserialization-for-view-id-xxxx-we-have-no-such-photon-view
        //photonView.RPC(nameof(PlayerLoaded), RpcTarget.AllBuffered);
        gameTracker = GameObject.FindGameObjectWithTag("GameTracker").GetComponent<GameTracker>();

        // Start method will only be called once scene is loaded, thus each player sends an RPC to confirm that they have loaded the scene and are ready to be spawned in
        gameTracker.PlayerLoadedFirstLevel();

    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        // master client checks GameTracker class for count of players spawned in and only spawns them in once it is equal to the amount of players in room
        if (!spawnCalled && gameTracker.PlayerCountFirst >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            spawnCalled = true;
            Invoke(nameof(StartCamera), 5f);
            Invoke(nameof(SpawnPlayers), 49.5f);
            //Invoke(nameof(SpawnPlayers), 6.5f);
        }
    }
   
    void StartCamera()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(StartCameraRPC), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void StartCameraRPC()
    {
        cameraObject.GetComponent<Animator>().enabled = true;
        cameraObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        cameraObject.GetComponent<AudioSource>().enabled = true;
        cameraObject.GetComponent<IntroSubbtitles>().enabled = true;
        environmentGameObject.SetActive(true);
        guardGameObject.SetActive(true);
        trainsGameObject.SetActive(true);
        bagSpawnerGameObject.SetActive(true);
        spotlightObjects.SetActive(true);
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
        Destroy(cameraObject);
       
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient != null)
            PhotonNetwork.LoadLevel(0);
    }
}
