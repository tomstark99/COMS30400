using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateOrJoinRoomCanvas : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private CreateRoomMenu createRoomMenu;
    [SerializeField]
    private RoomListingsMenu roomListingsMenu;
    [SerializeField]
    private OptionsMenu optionsMenu;
    [SerializeField]
    private TextMeshProUGUI name;

    private RoomsCanvases roomsCanvases;

    public override void OnConnectedToMaster()
    {
//        name.text = PhotonNetwork.LocalPlayer.NickName;
    }

    public void Initialise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
        createRoomMenu.Initialise(canvases);
        roomListingsMenu.Initalise(canvases);
        optionsMenu.Initialise(canvases);

    }

    // sets the create or join room canvas to active
    public void Show()
    {
        gameObject.SetActive(true);
        name.text = PhotonNetwork.LocalPlayer.NickName;
    }

    // hides the create or join room canvas
    public void Hide()
    {
        gameObject.SetActive(false);
        name.text = "";
    }

    public void StartTutorial()
    {
        PhotonNetwork.LoadLevel("Scenes/TutorialScene");
    }
}
