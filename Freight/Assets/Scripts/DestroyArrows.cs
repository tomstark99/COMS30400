using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DestroyArrows : MonoBehaviourPun
{
    public GameObject arrows;

    void Start() {
        if(!PhotonView.IsMine)
            Destroy(this);
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") {
            
            photonView.RPC(nameof(DeactivateArrowsGlowingRPC), photonView.Owner, other.GetComponent<PhotonView>().ViewID);
        }
    }

    void DeactivateArrowsGlowingRPC(int playerID)
    {
        Destroy(arrows);
        PhotonView player = PhotonView.Find(playerID).GetComponent<PhotonView>();
        Destroy(player.GetComponent<ArrowGlowing>());
    }
}
