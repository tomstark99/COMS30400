using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

/*
    This class is used to check if the player is inrange of the fence for objective purposes
*/
public class BreakFencePhoton : MonoBehaviourPun
{
    public GameObject text;

    public event Action InRangeOfFence;

    private bool walkedInRangeOfFence = false;

    // event is called for all players, this event is subscribed to in the Objectives class
    [PunRPC]
    void InRangeOfFenceRPC()
    {
        InRangeOfFence?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        // we check if the thing that enters the trigger is a player
        if (!walkedInRangeOfFence && other.tag == "Player")
        {
            // send an RPC to all players to trigger event
            photonView.RPC(nameof(InRangeOfFenceRPC), RpcTarget.All);
        }
    }
}
