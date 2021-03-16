using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

public class BreakFencePhoton : MonoBehaviourPun
{
    public GameObject text;
    private GameObject[] players;
    private bool isBroken;
    private bool overlayDisplayed = false;
    
    [DllImport("__Internal")]
    private static extern void LoadOverlay(String relativePath);
    [DllImport("__Internal")]
    private static extern void ClearOverlay();

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
    }

    [PunRPC]
    void SetPressPToActive()
    {
        text.SetActive(true);                
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!overlayDisplayed) {
            LoadOverlay("overlays/pull_apart_fence.png");
            overlayDisplayed = true;
        }
#endif
    }

    [PunRPC]
    void SetPressPToNotActive()
    {
        text.SetActive(false);
        Debug.Log("clearText");
#if UNITY_WEBGL && !UNITY_EDITOR
        if (overlayDisplayed) {
            ClearOverlay();
            overlayDisplayed = false;
        }
#endif
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
