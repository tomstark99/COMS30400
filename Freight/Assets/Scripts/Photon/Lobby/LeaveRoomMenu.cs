using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeaveRoomMenu : MonoBehaviour
{
    private RoomsCanvases roomsCanvases;
    public void Initalise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    // when click on leave room, leaves the room and hides the current room canvas and shows the join room canvas again
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        roomsCanvases.CurrentRoomCanvas.Hide();
        roomsCanvases.CreateOrJoinRoomCanvas.Show();
    }
}
