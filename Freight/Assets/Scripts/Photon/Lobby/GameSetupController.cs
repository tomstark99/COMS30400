using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class GameSetupController : MonoBehaviourPun
{
    /*
     * All this script is doing in the end is creating the player object. The game is told to find this player object under a 
     * PhotonPrefabs folder and to look for the prefab named PhotonPlayer.
     * Everything is setting the starting position and rotation values.
     * */
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(CreatePlayer), RpcTarget.All);
    }

    [PunRPC]
    private void CreatePlayer()
    {
        //Debug.Log("Creating Player");
        //int i = 0;
        //foreach (var p in PhotonNetwork.PlayerList)
        //{
        //    Debug.Log(p);
        //    GameObject player = PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayer", new Vector3(222, 4, 294 + i), Quaternion.identity);
        //    if (p != PhotonNetwork.LocalPlayer)
        //        player.GetComponent<PhotonView>().TransferOwnership(p);
        //    else
        //    {
        //        player.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Fixed;
        //        player.GetComponent<PlayerMovementPhoton>().enabled = true;
        //        player.GetComponent<PhotonPlayer>().enabled = true;
        //        player.GetComponent<MouseLookPhoton>().enabled = true;
        //        player.GetComponent<HeadMovementPhoton>().enabled = true;
        //        player.GetComponent<IkBehaviour>().enabled = true;
        //    }

        //    i += 3;
        //}
        PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayer", new Vector3(222, 4, 295), Quaternion.identity);
    }

    //public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player previousOwner)
    //{

    //}

    //public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    //{
    //    if (targetView.gameObject.GetComponent<CharacterController>() != null)
    //    {
    //        Debug.Log("UNITY IS IARRIRIRR");
    //        targetView.OwnershipTransfer = OwnershipOption.Fixed;
    //        targetView.gameObject.GetComponent<PlayerMovementPhoton>().enabled = true;
    //        targetView.gameObject.GetComponent<PhotonPlayer>().enabled = true;
    //        targetView.gameObject.GetComponent<MouseLookPhoton>().enabled = true;
    //        targetView.gameObject.GetComponent<HeadMovementPhoton>().enabled = true;
    //        targetView.gameObject.GetComponent<IkBehaviour>().enabled = true;
    //    }

    //}
}
