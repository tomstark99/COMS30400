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
        endGameCamera.GetComponent<CinemachineVirtualCamera>().Priority = 101;
        winningText.SetActive(true);
        GameObject.FindGameObjectWithTag("Car").GetComponent<CarWheelAnimation>().IsSpinning = true;
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
