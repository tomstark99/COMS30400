using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI roomName;

    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    // when click on create room, create a 2 player room
    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
       

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);
        
        roomsCanvases.CurrentRoomCanvas.SetRoomName(roomName.text);
    }

    // when room is created, show the current room and hide previous menu
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created");
        if (PhotonNetwork.CurrentRoom.IsVisible == false)
        {
            PhotonNetwork.LoadLevel(2);
            roomsCanvases.CurrentRoomCanvas.Hide();
            roomsCanvases.CreateOrJoinRoomCanvas.Hide();
        }
        else
        {
            Debug.Log("1");
            roomsCanvases.CurrentRoomCanvas.Show();
            roomsCanvases.CreateOrJoinRoomCanvas.Hide();
        }
    }

    // debug log if room creation failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed " + message);
    }
}
