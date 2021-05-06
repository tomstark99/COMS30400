using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StartTutorialMenu : MonoBehaviourPunCallbacks
{
    public void OnClick_CreateTutorial()
    {
        if (!PhotonNetwork.IsConnected) return;

        //ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        //prop.Add("sliderValueDiff", "Tutorial");
        //if (PhotonNetwork.NetworkClientState != Photon.Realtime.ClientState.Leaving)
        //    PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

        RoomOptions options = new RoomOptions();
        options.IsVisible = false;
        options.MaxPlayers = 1;
        string roomName = "Tutorial" + PhotonNetwork.CountOfRooms;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);

    }

    public void OnMouseOver() {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }
    public void OnMouseExit() {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }

}
