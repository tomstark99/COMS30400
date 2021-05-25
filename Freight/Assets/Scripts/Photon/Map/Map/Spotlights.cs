using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spotlights : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        // checks if the game settings difficulty keeps the spotlights active or not
        if (!(bool) PhotonNetwork.CurrentRoom.CustomProperties["SpotlightsActive"])
        {
            gameObject.SetActive(false);
        }
    }

}
