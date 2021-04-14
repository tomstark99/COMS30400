using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class EndGame : MonoBehaviourPun
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
    public CinemachineVirtualCamera vcam;

    public HashSet<Collider> GetColliders() { return colliders; }

    void Start()
    {
        // subscribe event to function
        StartEndGame += HandleEndGame;
        EndTheGame += ShowEndScreen;
        gameEnding = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "locomotive")
        {
            colliders.Add(other);
            Debug.Log(other.gameObject);
            Debug.Log(colliders.Count);
            // 11 box colliders on the train so when all of them are in endgame, start endgame
            if (colliders.Count == 11)
            {
                StartEndGame();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
        Debug.Log(other.gameObject);
    }

    private void HandleEndGame()
    {
        gameEnding = true;
        timeToEnd = 0f;
    }

    private void ShowEndScreen()
    {
        endScreen = 0f;
        showingEndScreen = true;
    }

    [PunRPC]
    void SetActiveLevelCompleteRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        player.transform.GetChild(13).GetChild(0).gameObject.SetActive(true);
    }

    [PunRPC]
    void SetCutsceneCameraActiveRPC()
    {
        vcam.GetComponent<CinemachineVirtualCamera>().Priority = 99;
    }

    [PunRPC]
    void SetGameLostActiveRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        player.transform.GetChild(13).GetChild(14).gameObject.SetActive(true);
        player.transform.GetChild(13).GetChild(7).gameObject.SetActive(false);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (gameEnding)
            {
                timeToEnd += Time.deltaTime;
                if (timeToEnd > 5f)
                {
                    gameWon = true;
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var player in players)
                    {
                        Debug.Log(player.GetComponent<PlayerMovementPhoton>().OnTrain);
                        Debug.Log(player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Backpack/Backpack-20L_i").gameObject.activeSelf);
                        // if player not on train or if their backpack is not active, they lose 
                        if (!player.GetComponent<PlayerMovementPhoton>().OnTrain || !player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Backpack/Backpack-20L_i").gameObject.activeSelf)
                        {
                            gameWon = false;
                            break;
                        }
                    }
                    gameEnding = false;

                    if (gameWon == true)
                    {
                        Debug.Log("you won!");
                        foreach (var player in players)
                        {
                            photonView.RPC(nameof(SetActiveLevelCompleteRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                        }
                    }
                    else
                    {
                        bool switchCamera = false;
                        foreach (var player in players)
                            switchCamera = true;

                        if (switchCamera)
                        {
                            photonView.RPC(nameof(SetCutsceneCameraActiveRPC), RpcTarget.All);
                            Debug.Log("you lost...");
                            foreach (var player in players)
                            {
                                //player.transform.GetChild(13).GetChild(1).gameObject.SetActive(true);
                                //player.transform.GetChild(13).GetChild(14).gameObject.SetActive(true);
                                //player.transform.GetChild(13).GetChild(7).gameObject.SetActive(false);
                                photonView.RPC(nameof(SetGameLostActiveRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                            }
                        }
                        //uncomment for cinemachine transition

                    }
                    EndTheGame();
                }
            }
            else if (showingEndScreen)
            {
                endScreen += Time.deltaTime;
                if (endScreen > 6f)
                {
                    PhotonNetwork.LoadLevel(0);
                    showingEndScreen = false;
                }
            }
        }
    }
}
