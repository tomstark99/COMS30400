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
        InRangeOfTrain += setTrainOutline;
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        Debug.Log(other);
        if (!walkedInRangeOfTrain && other.gameObject.tag == "Player")
        {
            photonView.RPC(nameof(InRangeOfTrainRPC), RpcTarget.All);
        }
    }

    void setTrainOutline()
    {
        walkedInRangeOfTrain = true;
        gameObject.GetComponent<Outline>().enabled = true;
    }
}
