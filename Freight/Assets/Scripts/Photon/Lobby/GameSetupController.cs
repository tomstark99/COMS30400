using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

/*
    This class is on the first level, it uses GameTracker to see when both players load into the level and then spawns them in as well as setting all game objects with photon views on them to active
*/
public class GameSetupController : MonoBehaviourPunCallbacks
{

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
        
        // finds the GameTracker object in the scene
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
            // we add an invoke to start the camera as WebGL can be a bit laggy with loading the level, causing freezing issues
            // the invoke is set to 5 seconds to give us some time to be sure the player has fully loaded the level and has every asset in the level loaded to not give us null reference errors
            Invoke(nameof(StartCamera), 5f);
            // we spawn the players after this time as this is how long it takes the cutscene to finish
            Invoke(nameof(SpawnPlayers), 49.5f);
        }
    }
   
    void StartCamera()
    {
        // tells all players to start the initial cutscene
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(StartCameraRPC), RpcTarget.AllBufferedViaServer);
    }

    // RPC call that sets the camera animator to active and the waiting text to false
    [PunRPC]
    void StartCameraRPC()
    {
        cameraObject.GetComponent<Animator>().enabled = true;
        cameraObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        // sets the voiceover to active as well as the subtitles
        cameraObject.GetComponent<AudioSource>().enabled = true;
        cameraObject.GetComponent<IntroSubbtitles>().enabled = true;

        // we set all the game objects with PhotonViews on them to inactive on scene load and set them to active once the camera animation starts
        // this is because we were getting issues with receving RPCs before the scene had been loaded, thus freezing the game as we could not find the PhotonViews sending these RPCs yet
        environmentGameObject.SetActive(true);
        guardGameObject.SetActive(true);
        trainsGameObject.SetActive(true);
        bagSpawnerGameObject.SetActive(true);
        spotlightObjects.SetActive(true);
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
        // gets a random z value so players don't spawn in each other
        int z = Random.Range(294,303);
        // spawn player in
        PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerPruna", new Vector3(254, 10, z), Quaternion.identity);
        // destroy cutscene camera object so we don't have two cameras in the level
        Destroy(cameraObject);
       
    }

    // if the master client leaves the game, we return back to lobby
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient != null)
            PhotonNetwork.LoadLevel(0);
    }
}
