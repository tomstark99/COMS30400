using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spotlights : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        if (!(bool) PhotonNetwork.CurrentRoom.CustomProperties["SpotlightsActive"])
        {
            gameObject.SetActive(false);
        }
    }

}
