using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CurrentRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roomText;
    [SerializeField]
    private PlayerListingMenu playerListingMenu;
    [SerializeField]
    private LeaveRoomMenu leaveRoomMenu;

    private RoomsCanvases roomsCanvases;

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
        playerListingMenu.Initalise(canvases);
        leaveRoomMenu.Initalise(canvases);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetRoomName(string name)
    {
        roomText.text = "Room: " + name;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(1);
        }
    }
}
