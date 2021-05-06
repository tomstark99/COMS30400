using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

public class OpenDoorPhoton : MonoBehaviourPun
{
    public GameObject text;

    public GameObject LeftHand;
    public GameObject RightHand;

    private List <GameObject> playersInRange = new List<GameObject>();
    private bool isBroken;

    public event Action InRangeOfDoor;
    private bool overlayDisplayed = false;
    private bool walkedInRangeOfDoor = false;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
        InRangeOfDoor += setDoorOutline;
    }

    [PunRPC]
    void SetPressPToActive()
    {
        
            text.SetActive(true);
            LeftHand.SetActive(true);
            RightHand.SetActive(true);
            
            Overlay.LoadOverlay("overlays/pull_apart_fence.png");
            overlayDisplayed = true;  
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
    void DestroyDoor()
    {
        PhotonNetwork.Destroy(transform.gameObject);
    }

    void Update()
    {
        if (isBroken)
         return;
        //Debug.Log(playersInRange.Count);
        foreach (GameObject player in playersInRange)
        {
            if (!player.GetPhotonView().IsMine) continue;
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            
            if (tempDist <= 3.60f)
            {
                string gesture = player.GetComponent<PhotonPlayer>().gesture;
                bool pPressed = player.GetComponent<PhotonPlayer>().IsPressingP();
                if (!overlayDisplayed){
                    photonView.RPC(nameof(SetPressPToActive), player.GetComponent<PhotonView>().Owner);
                }
                if (gesture.CompareTo("P") == 0 || pPressed) 
                {
                    Vector3 spawnPosition = transform.position;
                    photonView.RPC(nameof(SetPressPToNotActive), player.GetComponent<PhotonView>().Owner);
                    photonView.RPC(nameof(DestroyDoor), RpcTarget.MasterClient);
                    isBroken = true;
                }
            }
            else if (overlayDisplayed) 
            {
                photonView.RPC(nameof(SetPressPToNotActive), player.GetComponent<PhotonView>().Owner);
            }
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playersInRange.Add(other.gameObject);
            if (!walkedInRangeOfDoor)
            {
                photonView.RPC(nameof(InRangeOfDoorRPC), RpcTarget.All);
            }
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "Player")
        {
            if(overlayDisplayed){
                photonView.RPC(nameof(SetPressPToNotActive), other.gameObject.GetComponent<PhotonView>().Owner);
            }
            playersInRange.Remove(other.gameObject);
        }
    }

    [PunRPC]
    void InRangeOfDoorRPC()
    {
        InRangeOfDoor?.Invoke();
    }

    void setDoorOutline()
    {
        walkedInRangeOfDoor = true;
        gameObject.GetComponent<Outline>().enabled = true;
    }
}
