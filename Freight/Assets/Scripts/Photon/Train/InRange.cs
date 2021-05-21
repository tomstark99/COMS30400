using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;
//InRange is attached to the train that is about to leave. This script activates the train outline and it complets the objective 
// (find a way out of here) when the player is close enough to the train
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
