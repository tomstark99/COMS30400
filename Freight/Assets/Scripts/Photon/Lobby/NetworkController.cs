using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using FrostweepGames.Plugins.Native;
using VoiceChatClass;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        var voice = GameObject.Find("[PeerJS]VoiceChat");
        if (voice != null)
        {
            Destroy(voice);
        }

        var microphone = GameObject.Find("[FG]Microphone");
        if (microphone != null)
        {
            if(CustomMicrophone.IsRecording(CustomMicrophone.devices[0]))
            {
                CustomMicrophone.End(CustomMicrophone.devices[0]);
            }
            Destroy(microphone);
        }



        // request microphone permissions at the start of the menu
        if (!CustomMicrophone.HasMicrophonePermission())
        {
            CustomMicrophone.RequestMicrophonePermission();
        }
        if (!CustomMicrophone.HasConnectedMicrophoneDevices())
        {
            CustomMicrophone.RefreshMicrophoneDevices();
        }
        //Debug.Log(CustomMicrophone.devices.Length + " microphone devices found");

        // destroys game tracker from previous game 
        if (GameObject.FindGameObjectWithTag("GameTracker") != null)
            Destroy(GameObject.FindGameObjectWithTag("GameTracker"));

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
        PhotonNetwork.ConnectUsingSettings();
        //Debug.Log(PhotonNetwork.PhotonServerSettings);
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
        //Debug.Log("We are connected to the " + PhotonNetwork.CloudRegion + " server");
        if (!PhotonNetwork.InLobby)
        {
           // Debug.Log(PhotonNetwork.CurrentRoom);
            if (PhotonNetwork.LocalPlayer.NickName == "")
                PhotonNetwork.LocalPlayer.NickName = GetRandomName();

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.JoinLobby();
        }
    }
}
