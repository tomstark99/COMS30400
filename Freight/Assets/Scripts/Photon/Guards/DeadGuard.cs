using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadGuard : MonoBehaviourPun
{
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
