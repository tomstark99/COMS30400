using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

public class BreakFencePhoton : MonoBehaviourPun
{
    public GameObject text;

    public GameObject LeftHand;
    public GameObject RightHand;

    private GameObject[] players;
    private bool isBroken;

    public event Action FenceBroke;
    public event Action InRangeOfFence;
    private bool overlayDisplayed = false;
    private bool walkedInRangeOfFence = false;

    public GameObject Arrows;
    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
        InRangeOfFence += setFenceOutline;
    }

    
    [PunRPC]
    void InRangeOfFenceRPC()
    {
        InRangeOfFence();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!walkedInRangeOfFence)
        {
            photonView.RPC(nameof(InRangeOfFenceRPC), RpcTarget.All);
        }
    }

    void setFenceOutline()
    {
        walkedInRangeOfFence = true;
        gameObject.GetComponent<Outline>().enabled = true;
    }
}
