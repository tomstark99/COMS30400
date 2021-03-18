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

        RoomOptions options = new RoomOptions();
        options.IsVisible = false;
        options.MaxPlayers = 1;
        string roomName = "Tutorial" + PhotonNetwork.CountOfRooms;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }
}
