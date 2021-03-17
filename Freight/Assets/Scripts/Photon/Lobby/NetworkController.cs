using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = GetRandomName();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log(PhotonNetwork.PhotonServerSettings);
    }

    private string GetRandomName()
    {
        string name = "stefan";
        string name2 = "omar";
        int random = Random.Range(1,10);
        int random2 = Random.Range(1000, 9999);

        if(random > 5)
            return name + random2.ToString();
        else
            return name2 + random2.ToString();
    }

    public override void OnConnectedToMaster()
    {
        text.text = "Connected to " + PhotonNetwork.CloudRegion;
        Debug.Log("We are connected to the " + PhotonNetwork.CloudRegion + " server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
