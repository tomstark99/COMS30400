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
    private bool objectiveActive = false;
    private bool foundTrain = false;
    public GameObject[] carriages;

    // Start is called before the first frame update
    void Start()
    {
        InRangeOfTrain += TrainOutlineOn;
        if (gameObject.GetComponent<SplineWalkerPhoton>() != null) {
            
        }
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
        if(other.gameObject.tag == "Player") 
        {
            if(other.gameObject.GetComponent<Objectives>().FindBackpacks.activeSelf && !other.gameObject.GetComponent<Objectives>().FindBackpacksDesc.activeSelf) {
                if (!walkedInRangeOfTrain)
                {
                    objectiveActive = true;
                    photonView.RPC(nameof(InRangeOfTrainRPC), RpcTarget.All);
                }
            } 
            // else if(other.gameObject.GetComponent<Objectives>().FindTrain.activeSelf && !other.gameObject.GetComponent<Objectives>().FindTrainDesc.activeSelf) {
            //     if (walkedInRangeOfTrain) {
            //         foundTrain = true;
            //         photonView.RPC(nameof(WalkedOutOfTrain), RpcTarget.All);
            //     }
            // }
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if (!objectiveActive) 
        {
            if (walkedInRangeOfTrain && other.gameObject.tag == "Player")
            {
                photonView.RPC(nameof(InRangeOfTrainRPC), RpcTarget.All);
            }
        } 
        // else if (!foundTrain) {
            
        //     if (!walkedInRangeOfTrain && other.gameObject.tag == "Player" && foundTrain) {
        //         photonView.RPC(nameof(InRangeOfTrainRPC), RpcTarget.All)
        //     }
        // }
    }

    void TrainOutlineOn()
    {
        walkedInRangeOfTrain = true;
        gameObject.GetComponent<Outline>().enabled = true;
        foreach (var c in carriages) {
            c.GetComponent<Outline>().enabled = true;
        }
    }

    void TrainOutlineOff()
    {
        walkedInRangeOfTrain = false;
        gameObject.GetComponent<Outline>().enabled = false;
        foreach (var c in carriages) {
            c.GetComponent<Outline>().enabled = false;
        }
    }
}
