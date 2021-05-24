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
       
        // create room settings, setting the max players to 2 
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;

        // we create the room however if there is a room of the same name, we instead join it
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);
        
        roomsCanvases.CurrentRoomCanvas.SetRoomName(roomName.text);
    }

    // when room is created, show the current room and hide previous menu
    public override void OnCreatedRoom()
    {
        // if the initial room state is that it is invisible, we know that it is a tutorial room so we just instantly load the player into the tutorial level
        if (PhotonNetwork.CurrentRoom.IsVisible == false)
        {
            GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>().SetGameSettings();
            PhotonNetwork.LoadLevel("Scenes/TutorialScene");
            roomsCanvases.CurrentRoomCanvas.Hide();
            roomsCanvases.CreateOrJoinRoomCanvas.Hide();
        }
        // otherwise it is just a normal room so we show the current room canvas and disable the old canvas
        else
        {
            roomsCanvases.CurrentRoomCanvas.Show();
            roomsCanvases.CreateOrJoinRoomCanvas.Hide();
        }
    }

    // debug log if room creation failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Debug.Log("Room creation failed " + message);
    }
}
