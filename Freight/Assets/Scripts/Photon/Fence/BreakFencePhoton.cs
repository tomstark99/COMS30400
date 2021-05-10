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
    void SetPressPToActive()
    {
        if (!overlayDisplayed) {
            text.SetActive(true);
            LeftHand.SetActive(true);
            RightHand.SetActive(true);
            
            Overlay.LoadOverlay("overlays/pull_apart_fence.png");
            overlayDisplayed = true;  
        }
    }

    [PunRPC]
    void SetPressPToNotActive()
    {
        if (overlayDisplayed) {
            text.SetActive(false);
            LeftHand.SetActive(false);
            RightHand.SetActive(false);
            
            Overlay.ClearOverlay();
            overlayDisplayed = false;
        }
    }

    [PunRPC]
    void DestroyFence()
    {
        PhotonNetwork.Destroy(transform.gameObject);
    }

    [PunRPC]
    void FenceBrokeRPC()
    {
        // event
        FenceBroke();
    }

    [PunRPC]
    void DestroyArrows() {
        Destroy(Arrows);
        players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine) continue;
            if (player.GetComponent<ArrowGlowing>())
                player.GetComponent<ArrowGlowing>().enabled = false;
        
        }
    }
    void Update()
    {
        if (isBroken)
         return;
        players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine) continue;
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            string gesture = player.GetComponent<PhotonPlayer>().gesture;
            bool pPressed = player.GetComponent<PhotonPlayer>().IsPressingP();
            
            if (tempDist <= 4f)
            {
                photonView.RPC("SetPressPToActive", player.GetComponent<PhotonView>().Owner);
                if (gesture.CompareTo("P") == 0 || pPressed) 
                {

                    photonView.RPC(nameof(FenceBrokeRPC), RpcTarget.All);
                    Vector3 spawnPosition = transform.position;
                    
                    photonView.RPC(nameof(SetPressPToNotActive), player.GetComponent<PhotonView>().Owner);

                    photonView.RPC(nameof(DestroyFence), RpcTarget.MasterClient);
                    photonView.RPC(nameof(DestroyArrows), RpcTarget.All);
                    PhotonNetwork.Instantiate("PhotonPrefabs/fence_simple_broken_open Variant 1", spawnPosition, Quaternion.Euler(0f, -45f, 0f));
                    isBroken = true;
                    break;
                }
            }
            else if (tempDist > 4f)
            {
                photonView.RPC("SetPressPToNotActive", player.GetComponent<PhotonView>().Owner);
            }
        }

        
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
