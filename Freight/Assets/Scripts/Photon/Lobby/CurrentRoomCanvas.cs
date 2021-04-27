using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviourPunCallbacks
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

    // shows current room canvas
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // displays the name of the room
    public void SetRoomName(string name)
    {
        roomText.text = "Room: " + name;
    }

    // hides current room canvas
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // if master client presses on start game, game is started
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameObject.transform.GetChild(3).GetComponent<Button>().interactable = false;
            Debug.Log("Starting Game");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
            prop.Add("levelToLoad", "Assets/Scenes/TrainStationPun.unity");
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

        if (propertiesThatChanged.ContainsKey("levelToLoad"))
        {
            PhotonNetwork.LoadLevel("Scenes/LoadingScreen");
        }
    }
}
