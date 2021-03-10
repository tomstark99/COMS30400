using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private PlayerListing playerListing;
    private RoomsCanvases roomsCanvases;

    private List<PlayerListing> listings = new List<PlayerListing>();

    // gets all players in room an awake
    private void Awake()
    {
        GetCurrentRoomPlayers();
    }

    public void Initalise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    // when player leaves, destroys the player transform and its children
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        content.DestroyChildren();
    }

    // loops through the player key value pairs and adds them to List of player listings
    private void GetCurrentRoomPlayers()
    {
        foreach (KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
        
    }

    // populates list of players and sets their info
    private void AddPlayerListing(Photon.Realtime.Player player)
    {
        // creates listing and adds to list of rooms
        PlayerListing listing = Instantiate(playerListing, content);
        if (listing != null)
        {
            listing.SetPlayerInfo(player);
            listings.Add(listing);
        }
    }

    // when player enters room, add him to the List
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    // when player leaves the room, destroy player object and delete from list
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(listings[index].gameObject);
            listings.RemoveAt(index);
        }
    }
}
