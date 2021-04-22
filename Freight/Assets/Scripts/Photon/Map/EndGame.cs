﻿using System;
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
    private int totalBags;
    private int totalOnTrain;
    public CinemachineVirtualCamera vcam;

    public HashSet<Collider> GetColliders() { return colliders; }

    void Start()
    {
        // subscribe event to function
        StartEndGame += HandleEndGame;
        EndTheGame += ShowEndScreen;
        gameEnding = false;
        totalBags = 0;
        totalOnTrain = 0;

        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += ShowEndScreen;
        }

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
        //player.transform.GetChild(13).GetChild(14).gameObject.SetActive(true);
        player.transform.Find("UI 1/TrainLeftWithoutYouLost").gameObject.SetActive(true);
        //player.transform.GetChild(13).GetChild(7).gameObject.SetActive(false);
        player.transform.Find("UI 1/ObjectivesWrapper").gameObject.SetActive(false);
    }

    [PunRPC]
    void SetGameLostBagsActiveRPC(int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        //player.transform.GetChild(13).GetChild(15).gameObject.SetActive(true);
        player.transform.Find("UI 1/YouDidntCollectTheBags").gameObject.SetActive(true);
        //player.transform.GetChild(13).GetChild(7).gameObject.SetActive(false);
        player.transform.Find("UI 1/ObjectivesWrapper").gameObject.SetActive(false);
    }

    void CheckGameWinConditions(GameObject[] players)
    {
        
        foreach (var player in players)
        {
            Debug.Log(player.GetComponent<PlayerMovementPhoton>().OnTrain);
            Debug.Log(player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Backpack/Backpack-20L_i").gameObject.activeSelf);
            bool bagOnBack = player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Backpack/Backpack-20L_i").gameObject.activeSelf;
            if (player.GetComponent<PlayerMovementPhoton>().OnTrain)
                totalOnTrain += 1;

            if (bagOnBack)
                totalBags += 1;

            // if player not on train or if their backpack is not active, they lose 
            if (!player.GetComponent<PlayerMovementPhoton>().OnTrain || !bagOnBack)
            {
                gameWon = false;
                break;
            }
        }
    }
    
    void CheckIfGameOver()
    {
        endScreen += Time.deltaTime;
        if (endScreen > 6f)
        {
            PhotonNetwork.LoadLevel(0);
            showingEndScreen = false;
        }
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
                    CheckGameWinConditions(players);

                    gameEnding = false;

                    if (gameWon == true)
                    {
                        Debug.Log("you won!");
                        foreach (var player in players)
                        {
                            player.GetComponent<Achievements>().LikeANinjaCompleted();
                            GameObject[] deadGuards = GameObject.FindGameObjectsWithTag("DeadGuard");
                            // checks if there are no dead guards
                            if (deadGuards == null || deadGuards.Length == 0)
                                player.GetComponent<Achievements>().PeaceTreatyCompleted();

                            photonView.RPC(nameof(SetActiveLevelCompleteRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                        }
                    }
                    else
                    {
                        photonView.RPC(nameof(SetCutsceneCameraActiveRPC), RpcTarget.All);
                        Debug.Log("you lost...");
                        if (totalOnTrain < players.Length)
                        {
                            foreach (var player in players)
                            {
                                photonView.RPC(nameof(SetGameLostActiveRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                            }
                        } 
                        else if (totalBags < players.Length)
                        {
                            foreach (var player in players)
                            {
                                photonView.RPC(nameof(SetGameLostBagsActiveRPC), player.GetComponent<PhotonView>().Owner, player.GetComponent<PhotonView>().ViewID);
                            }
                        }

                        //uncomment for cinemachine transition

                    }
                    EndTheGame();
                }
            }
            else if (showingEndScreen)
            {
                CheckIfGameOver();
            }
        }
    }
}
