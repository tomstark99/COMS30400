using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log(PhotonNetwork.PhotonServerSettings);
    }

    private string GetRandomName()
    {
        string name = "stefan";
        int random = Random.Range(1000, 9999);
        name += random.ToString();
        return name;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are connected to the " + PhotonNetwork.CloudRegion + " server");
        PhotonNetwork.LocalPlayer.NickName = GetRandomName();
        PhotonNetwork.JoinLobby();
    }
}
