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
        CreatePlayer();
    }
    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayer", new Vector3(222, 4, 289), Quaternion.identity);
        
    }
}
