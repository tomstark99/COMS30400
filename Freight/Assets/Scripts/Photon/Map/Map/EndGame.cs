using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

/*
    This script is used in the first level to check endgame conditions and display to the players whether they have won or lost 
*/
public class EndGame : MonoBehaviourPunCallbacks
{
    public GameObject leavingTrain;
    private HashSet<Collider> colliders = new HashSet<Collider>();

    public event Action StartEndGame;
    public event Action EndTheGame;
    private bool gameEnding;
    private float timeToEnd;
    private bool gameWon;
    private bool showingEndScreen;
    private float endScreen;
    private int totalBags;
    private int totalOnTrain; 
    public CinemachineVirtualCamera vcam;

    private bool trainsEnteredColliders;

    public HashSet<Collider> GetColliders() { return colliders; }

    void Start()
    {
        // subscribe event to function
        StartEndGame += HandleEndGame;
        EndTheGame += ShowEndScreen;
        gameEnding = false;
        totalBags = 0;
        totalOnTrain = 0;
        trainsEnteredColliders = false;

        Invoke(nameof(SubscribeToGuardEvent), 6f);
    }

    void SubscribeToGuardEvent()
    {
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += ShowEndScreen;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // only adds collider to list of colliders if it is tagged as a locomotive, meaning its the getaway train
        if (other.gameObject.tag == "locomotive")
        {
            colliders.Add(other);

            // 11 box colliders on the train so when all of them are in endgame, start endgame
            if (colliders.Count == 11 && !trainsEnteredColliders)
            {
                trainsEnteredColliders = true;
                StartEndGame();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
    
    // this is called when enough colliders have entered the endgame collider
    private void HandleEndGame()
    {
        gameEnding = true;
        timeToEnd = 0f;
    }

    // this is called when we have checked for endgame conditions and can now tell the player if they have lost or won
    private void ShowEndScreen()
    {
        endScreen = 0f;
        showingEndScreen = true;
    }

    // RPC call to all players telling them to display 'level complete' screen
    [PunRPC]
    void SetActiveLevelCompleteRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        player.transform.GetChild(13).GetChild(0).gameObject.SetActive(true);
    }

    // RPC call to all players telling them to activate the cinemachine camera on the train to show them that they lost
    [PunRPC]
    void SetCutsceneCameraActiveRPC()
    {
        vcam.GetComponent<CinemachineVirtualCamera>().Priority = 99;
    }

    // RPC call to all players to show text telling them that the train left without them
    [PunRPC]
    void SetGameLostActiveRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;

        player.transform.Find("UI 1/TrainLeftWithoutYouLost").gameObject.SetActive(true);

        player.transform.Find("UI 1/ObjectivesWrapper").gameObject.SetActive(false);
    }

    // RPC call to all players to show text telling them that they left without collecting all the bags
    [PunRPC]
    void SetGameLostBagsActiveRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;

        player.transform.Find("UI 1/YouDidntCollectTheBags").gameObject.SetActive(true);

        player.transform.Find("UI 1/ObjectivesWrapper").gameObject.SetActive(false);
    }

    // this function is used to check if the players have satisfied all conditions to win the game
    void CheckGameWinConditions(GameObject[] players)
    {
        // loop through all players
        foreach (var player in players)
        {
            // we check if the bag on the player's back is set to active or not, if it is it means they have collected it
            bool bagOnBack = player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Backpack/Backpack-20L_i").gameObject.activeSelf;
            
            // checks if the player has entered the train collider 
            if (player.GetComponent<PlayerOnTrain>().OnTrain)
                totalOnTrain += 1;

            if (bagOnBack)
                totalBags += 1;

            // if player not on train or if their backpack is not active, they lose 
            if (!player.GetComponent<PlayerOnTrain>().OnTrain || !bagOnBack)
            {
                gameWon = false;
                break;
            }
        }
    }
    
    // this function checks if the end screen has been shown for long enough and if it has, proceeds to the next scene  
    void CheckIfGameOver()
    {
        endScreen += Time.deltaTime;
        if (endScreen > 3f)
        {
            // if game has been won, we set the next scene to load to be the arrival train station
            if (gameWon)
            {
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
                prop.Add("levelToLoad", "Assets/Scenes/TrainStationArrive.unity");
                PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
            }
            // if game has been lost, we set the next scene to load to be the main menu scene
            else
            {
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
                prop.Add("levelToLoad", "Assets/Scenes/MenuSceneNew.unity");
                PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
            }


            showingEndScreen = false;
        }
    }

    // RPC call to award player with like a ninja achievement
    [PunRPC]
    void CheckLikeANinjaRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        player.GetComponent<Achievements>().LikeANinjaCompleted();
    }

    // RPC call to award player with peace treaty achievement
    [PunRPC]
    void CheckPeaceTreatyRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        player.GetComponent<Achievements>().PeaceTreatyCompleted();
    }

    // event call for game to end
    [PunRPC]
    void EndTheGameRPC() 
    {
        EndTheGame();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (gameEnding)
            {
                timeToEnd += Time.deltaTime;
                // after 5 seconds have passed, we check conditions for end game
                if (timeToEnd > 5f)
                {
                    // assume game is won, we will set this to false later if game has not been won
                    gameWon = true;

                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    // check if the conditions to win the game have been met
                    CheckGameWinConditions(players);

                    gameEnding = false;

                    if (gameWon)
                    {
                        Debug.Log("you won!");

                        GameObject[] deadGuards = GameObject.FindGameObjectsWithTag("DeadGuard");
                        foreach (var player in players)
                        {
                            photonView.RPC(nameof(CheckLikeANinjaRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);

                            // checks if there are no dead guards in the level so the player can complete PeaceTreaty
                            if (deadGuards == null || deadGuards.Length == 0)
                                photonView.RPC(nameof(CheckPeaceTreatyRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);

                            photonView.RPC(nameof(SetActiveLevelCompleteRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                        }
                    }
                    else
                    {
                        // set train's cinemachine camera to active 
                        photonView.RPC(nameof(SetCutsceneCameraActiveRPC), RpcTarget.All);
                        Debug.Log("you lost...");

                        // if the total players on the train is less than the players in the game, we display "Train Left Without You" message
                        if (totalOnTrain < players.Length)
                        {
                            foreach (var player in players)
                            {
                                photonView.RPC(nameof(SetGameLostActiveRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                            }
                        } 
                        // if all players on train but total bags are less than the players in the room, we display "You Didn't Collect The Bags" message
                        else if (totalBags < players.Length)
                        {
                            foreach (var player in players)
                            {
                                photonView.RPC(nameof(SetGameLostBagsActiveRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                            }
                        }

                    }
                    photonView.RPC(nameof(EndTheGameRPC), RpcTarget.All);
                }
            }
            else if (showingEndScreen)
            {
                CheckIfGameOver();
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // loads loading screen once properties have changed
        if (propertiesThatChanged.ContainsKey("levelToLoad"))
        {
            PhotonNetwork.LoadLevel("Scenes/LoadingScreen");
        }
    }
}
