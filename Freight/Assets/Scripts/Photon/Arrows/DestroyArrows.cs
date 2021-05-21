using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//This script is placed on an empty game object inside the train station, just after the broken fence
//when the player enters the collider the tutorial arrows objejcs are destroyed
public class DestroyArrows : MonoBehaviourPun
{
    public GameObject arrows;

    void Start() 
    {

    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") {
            photonView.RPC(nameof(DeactivateArrowsGlowingRPC), other.GetComponent<PhotonView>().Owner, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void DeactivateArrowsGlowingRPC(int playerID)
    {
        Destroy(arrows);
        PhotonView player = PhotonView.Find(playerID).GetComponent<PhotonView>();
        Destroy(player.GetComponent<ArrowGlowing>());
    }
}
