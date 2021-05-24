using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StartTutorialMenu : MonoBehaviourPunCallbacks
{
    public GameObject optionsCanvas;

    //start the tutorial scene
    public void OnClick_CreateTutorial()
    {
        if (!PhotonNetwork.IsConnected) return;

        RoomOptions options = new RoomOptions();
        options.IsVisible = false;
        options.MaxPlayers = 1;
        string roomName = "Tutorial" + PhotonNetwork.CountOfRooms;
        optionsCanvas.SetActive(false);
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);

    }

    public void OnMouseOver() {
        //If your mouse hovers over the GameObject with the script attached, output this message
       // Debug.Log("Mouse is over GameObject.");
    }
    public void OnMouseExit() {
        //The mouse is no longer hovering over the GameObject so output this message each frame
       // Debug.Log("Mouse is no longer on GameObject.");
    }

}
