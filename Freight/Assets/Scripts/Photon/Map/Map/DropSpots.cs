using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
    Script used to disable one of the drop points if only one player is in the game
*/
public class DropSpots : MonoBehaviourPun
{
    [SerializeField]
    private Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                ActivateDropSpot();
            }
        }
    }

    void ActivateDropSpot()
    {
        spawnPoints[1].gameObject.SetActive(false);
    }

}
