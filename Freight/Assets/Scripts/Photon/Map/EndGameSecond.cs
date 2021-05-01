using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Cinemachine;

public class EndGameSecond : MonoBehaviourPun
{
    public event Action PlayerReadyToLeave;

    [SerializeField]
    private GameObject endGameCamera;
    [SerializeField]
    private GameObject winningText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        
        if (PlayerReadyToLeave != null)
            PlayerReadyToLeave();
    }

    [PunRPC]
    void EndTheGameRPC()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            player.transform.parent = car.transform;
            player.GetComponent<PlayerMovementPhoton>().GameEnding = true;
            player.GetComponent<Achievements>().FreightCompleted();
        }

        endGameCamera.GetComponent<CinemachineVirtualCamera>().Priority = 101;
        winningText.SetActive(true);
        car.GetComponent<CarWheelAnimation>().IsSpinning = true;
        car.GetComponent<SplineWalker>().enabled = true;
        GetComponent<Outline>().enabled = false;
        
    }

    public void EndTheGame()
    {
        photonView.RPC(nameof(EndTheGameRPC), RpcTarget.All);
    }

    [PunRPC]
    void CheckEndGameRPC()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (var player in players)
        {
            if (player.GetComponent<ObjectivesSecond>().readyToLeave)
                counter++;
        }

        if (counter == 2)
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
}
