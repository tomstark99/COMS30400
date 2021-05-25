using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
    This script is applied to a game object on the first level so the time to leave is the same among all users
*/
public class SyncedTime : MonoBehaviour
{
    private float timeToLeave;

    public float TimeToLeave
    {
        get { return timeToLeave;  }
    }

    // Start is called before the first frame update
    void Awake()
    {
        timeToLeave = (float) PhotonNetwork.CurrentRoom.CustomProperties["TimeToLeave"];
    }

    // Update is called once per frame
    void Update()
    {
            
    }
}
