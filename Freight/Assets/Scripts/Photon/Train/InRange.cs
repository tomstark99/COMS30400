using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

public class InRange : MonoBehaviourPun
{
    private GameObject[] players;

    public event Action InRangeOfTrain;
    private bool overlayDisplayed = false;
    private bool walkedInRangeOfTrain = false;

    // Start is called before the first frame update
    void Start()
    {
        InRangeOfTrain += TrainOutlineOn;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    [PunRPC]
    void InRangeOfTrainRPC()
    {
        InRangeOfTrain();
    }

    [PunRPC]
    void WalkedOutOfTrain(){
        TrainOutlineOff();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            if(other.gameObject.GetComponent<Objectives>().FindBackpacks.activeSelf && !other.gameObject.GetComponent<Objectives>().FindBackpacksDesc.activeSelf) {
                if (!walkedInRangeOfTrain)
                {
                    photonView.RPC(nameof(InRangeOfTrainRPC), RpcTarget.All);
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (walkedInRangeOfTrain && other.gameObject.tag == "Player")
        {
            photonView.RPC(nameof(WalkedOutOfTrain), RpcTarget.All);
        }
    }

    void TrainOutlineOn()
    {
        walkedInRangeOfTrain = true;
        gameObject.GetComponent<Outline>().enabled = true;
    }

    void TrainOutlineOff()
    {
        walkedInRangeOfTrain = false;
        gameObject.GetComponent<Outline>().enabled = false;
    }
}
