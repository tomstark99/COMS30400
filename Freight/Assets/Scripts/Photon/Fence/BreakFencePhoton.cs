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
    private bool overlayDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
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

    void Update()
    {
        // if (isBroken)
        //  return;
        players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine) continue;
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            string gesture = player.GetComponent<PhotonPlayer>().gesture;
            bool pPressed = player.GetComponent<PhotonPlayer>().IsPressingP();
            
            if (tempDist <= 2.5f)
            {
                photonView.RPC("SetPressPToActive", player.GetComponent<PhotonView>().Owner);
                if (gesture.CompareTo("P") == 0 || pPressed) 
                {
                    Vector3 spawnPosition = transform.position;
                    
                    photonView.RPC("SetPressPToNotActive", player.GetComponent<PhotonView>().Owner);

                    photonView.RPC("DestroyFence", RpcTarget.MasterClient);
                    PhotonNetwork.Instantiate("PhotonPrefabs/fence_simple_broken_open Variant 1", spawnPosition, Quaternion.Euler(0f, 90f, 0f));
                    if (player.GetComponent<TutorialManager>() != null) {
                        FenceBroke();
                    }
                    isBroken = true;
                    break;
                }
            }
            else if (tempDist > 2.5f)
            {
                photonView.RPC("SetPressPToNotActive", player.GetComponent<PhotonView>().Owner);
            }
        }

        
    }
}
