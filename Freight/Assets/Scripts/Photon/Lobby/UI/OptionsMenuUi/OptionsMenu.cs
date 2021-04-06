using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    // when click on create room, create a 2 player room
    public void OnClick_OptionsButton()
    {
        roomsCanvases.OptionsCanvas.Show();
        roomsCanvases.CreateOrJoinRoomCanvas.Hide();
    }
}

