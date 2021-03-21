using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BreakFencePhoton : MonoBehaviourPun
{
    public GameObject text;
    private GameObject[] players;
    private bool isBroken;

    public event Action FenceBroke;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
    }

    [PunRPC]
    void SetPressPToActive()
    {
        text.SetActive(true);
    }

    [PunRPC]
    void SetPressPToNotActive()
    {
        text.SetActive(false);
    }

    [PunRPC]
    void DestroyFence()
    {
        PhotonNetwork.Destroy(transform.gameObject);
    }

    void Update()
    {
        if (isBroken)
         return;
        players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (var player in players)
        {
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            string gesture = player.GetComponent<PhotonPlayer>().gesture;
            bool pPressed = player.GetComponent<PhotonPlayer>().IsPressingP();
            
            if (tempDist <= 2.5f)
            {
                photonView.RPC("SetPressPToActive", player.GetComponent<PhotonView>().Owner);
                if (gesture.CompareTo("P") == 0 || pPressed) 
                {
                    Vector3 spawnPosition = transform.position;

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
