using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class BackButtonMenu : MonoBehaviour
{
    public GameObject optionsCanvas;
    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    private void Update() {

         if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(roomsCanvases.CreateOrJoinRoomCanvas != null) {
                roomsCanvases.CreateOrJoinRoomCanvas.Show();
            }
        }
    } 
    // when click on create room, create a 2 player room
    public void OnClick_BackButton()
    {
        optionsCanvas.SetActive(false);
        //roomsCanvases.OptionsMenu.Hide();
        if(roomsCanvases.CreateOrJoinRoomCanvas != null) {
            roomsCanvases.CreateOrJoinRoomCanvas.Show();
        } else {
            transform.parent.transform.parent.GetComponent<PlayerUI>().closeorOpenMenu();
        }

        //roomsCanvases.CreateOrJoinRoomCanvas.Show();
    }
}
