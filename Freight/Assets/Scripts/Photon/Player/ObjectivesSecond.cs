using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectivesSecond : MonoBehaviour
{
    [SerializeField]
    private GameObject dropBags;
    [SerializeField]
    private GameObject dropBagsDistance;
    [SerializeField]
    private GameObject dropBagsDistance2;
    [SerializeField]
    private GameObject dropBagsDesc;
    [SerializeField]
    private GameObject dropBagsBackground;

    private bool twoPlayers;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            twoPlayers = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
