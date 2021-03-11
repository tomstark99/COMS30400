using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateRoomMenu createRoomMenu;
    [SerializeField]
    private RoomListingsMenu roomListingsMenu;

    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
        createRoomMenu.Initialise(canvases);
        roomListingsMenu.Initalise(canvases);
    }

    // sets the create or join room canvas to active
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // hides the create or join room canvas
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
