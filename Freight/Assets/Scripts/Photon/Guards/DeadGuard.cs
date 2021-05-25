using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/*
    This class is placed on the dead guard  
*/
public class DeadGuard : MonoBehaviourPun
{
    // this RPC is used to tell all the players to make the guard the child of the DeadGuards game object
    [PunRPC]
    void Spawn()
    {
        transform.parent = GameObject.Find("/Environment/Interactables/DeadGuards").transform;
    }


    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("Spawn", RpcTarget.All);
    }

}
