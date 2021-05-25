using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class GameSetupControllerTutorial : MonoBehaviour
{
    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        //Debug.Log("Creating Player");
        PhotonNetwork.Instantiate("PhotonPrefabs/PhotonPlayerTutorialOld", new Vector3(241, 3, 68), Quaternion.identity);
    }
}
