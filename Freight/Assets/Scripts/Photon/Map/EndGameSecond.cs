using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Cinemachine;

public class EndGameSecond : MonoBehaviourPunCallbacks
{
    public event Action PlayerReadyToLeave;
    public event Action EndTheGameSecond;

    [SerializeField]
    private GameObject endGameCamera;
    [SerializeField]
    private GameObject winningText;

    private int playersToLeave;

    private bool gameOver;
    private float endScreen;

    private List<PhotonView> playersInCollider = new List<PhotonView>();

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        playersToLeave = 0;

        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += GameLost;
        }
    }

    void Update()
    {
        if (gameOver)
        {
            endScreen += Time.deltaTime;
            //Debug.Log("endScreen time: " + endScreen);

            if (endScreen > 6f)
            {
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
                prop.Add("levelToLoad", "Assets/Scenes/MenuSceneNew.unity");
                //Destroy(GameObject.FindGameObjectWithTag("GameTracker"));
                PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

                gameOver = false;

            }
        }
    }

    void GameLost()
    {
        gameOver = true;
        endScreen = 0f;
    }

    //void CheckIfGameOver()
    //{
    //    endScreen += Time.deltaTime;
    //    Debug.Log("Time Left: " + endScreen);
    //    if (endScreen > 6f)
    //    {
    //        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
    //        prop.Add("levelToLoad", "Assets/Scenes/MenuSceneNew.unity");
    //        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

    //        gameOver = false;

    //    }
    //}

    // RPC call only to the master, increases the players to leave
    [PunRPC]
    void IncreasePlayerToLeave()
    {
        playersToLeave++;
        CheckEndGame();
    }

    // calls the event only on the client that jumps on the truck
    [PunRPC]
    void CallPlayerReadyToLeave(int playerID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;
        //PlayerReadyToLeave();
        player.GetComponent<ObjectivesSecond>().EndGameChecker();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.gameObject.tag == "Player")
            {
                // checks if player is ready to leave as this event is only subscribed to once both bags have been delivered, masterclient increments player ready to leave count
                if (PlayerReadyToLeave != null && !playersInCollider.Contains(other.gameObject.GetComponent<PhotonView>()))
                {
                    playersInCollider.Add(other.gameObject.GetComponent<PhotonView>());
                    photonView.RPC(nameof(CallPlayerReadyToLeave), other.gameObject.GetComponent<PhotonView>().Owner, other.gameObject.GetComponent<PhotonView>().ViewID);
                    //EndTheGame();

                    photonView.RPC(nameof(IncreasePlayerToLeave), RpcTarget.MasterClient);
                }

            }
        }
    }

    [PunRPC]
    void EndTheGameRPC()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //// gets each player and sets their movement to inactive and checks if they completed the achievement
        foreach (var player in players)
        {
            player.transform.parent = car.transform;
            if (player.GetComponent<PlayerMovementPhoton>())
                player.GetComponent<PlayerMovementPhoton>().GameEnding();

            player.GetComponent<PlayerAudioClipsSecond>().GameFinished();

            player.GetComponent<Achievements>().FreightCompleted();
        }

        gameOver = true;
        endScreen = 0f;

        // sets cinemachine camera active
        endGameCamera.GetComponent<CinemachineVirtualCamera>().Priority = 101;

        // winning UI text
        winningText.SetActive(true);

        // starts moving the car
        car.GetComponent<CarWheelAnimation>().IsSpinning = true;
        car.GetComponent<SplineWalker>().enabled = true;
        GetComponent<Outline>().enabled = false;
    }

    public void EndTheGame()
    {
        photonView.RPC(nameof(EndTheGameRPC), RpcTarget.All);
        EndTheGameSecond?.Invoke();
    }

    // checks if both players have jumped on the back of the truck
    [PunRPC]
    void CheckEndGameRPC()
    {
        if (playersToLeave == 2)
        {
            EndTheGame();
        }
    }

    public void CheckEndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(CheckEndGameRPC), RpcTarget.MasterClient);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // loads scene once properties have changed
        if (propertiesThatChanged.ContainsKey("levelToLoad"))
        {
            PhotonNetwork.LoadLevel("Scenes/LoadingScreen");
        }
    }
}
