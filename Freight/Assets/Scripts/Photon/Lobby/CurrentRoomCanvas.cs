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
        if(name.Length > 1)
            roomText.text = "Room: " + name;
        else 
            roomText.text = "";
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
            PhotonNetwork.Instantiate("PhotonPrefabs/GameTracker", new Vector3(0,0,0), Quaternion.identity);

            gameObject.transform.GetChild(4).GetChild(1).GetComponent<Button>().interactable = false;
           // Debug.Log("Starting Game");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
            prop.Add("levelToLoad", "Assets/Scenes/TrainStationPun.unity");
            //prop.Add("levelToLoad", "Assets/Scenes/TrainStationArrive.unity");
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // loads scene once properties have changed
        if (propertiesThatChanged.ContainsKey("levelToLoad"))
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel("Scenes/LoadingScreen");
        }
    }
}
