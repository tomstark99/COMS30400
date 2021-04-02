using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class GameSetupController : MonoBehaviour
{
    /*
     * All this script is doing in the end is creating the player object. The game is told to find this player object under a 
     * PhotonPrefabs folder and to look for the prefab named PhotonPlayer.
     * Everything is setting the starting position and rotation values.
     * */
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            CreatePlayer();
    }
    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        foreach (var p in PhotonNetwork.PlayerList)
        {
            GameObject player = PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayer", new Vector3(222, 4, 294), Quaternion.identity);
            if (p != PhotonNetwork.LocalPlayer)
            player.GetComponent<PhotonView>().TransferOwnership(p);
        }
        
        
    }
}
