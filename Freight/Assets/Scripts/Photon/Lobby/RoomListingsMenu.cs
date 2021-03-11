using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing roomListing;

    private List<RoomListing> listings = new List<RoomListing>();
    private RoomsCanvases roomsCanvases;

    public void Initalise(RoomsCanvases canvases)
    {
        roomsCanvases = canvases;
    }

    // when join room, show current room canvas, clear all the listings and destroy them so if you leave room, there aren't duplicates
    public override void OnJoinedRoom()
    {
        roomsCanvases.CurrentRoomCanvas.Show();
        roomsCanvases.CreateOrJoinRoomCanvas.Hide();
        listings.Clear();
        content.DestroyChildren();
    }

    // when new listing is added, add it to the List of listings and set its info
    // when listing is removed, destroy it and remove it from the List
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                // finds listing to remove and removes it from list of rooms and fromt he UI
                int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            } 
            else
            {
                int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index == -1)
                {
                    // creates listing and adds to list of rooms
                    RoomListing listing = Instantiate(roomListing, content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        listings.Add(listing);
                    }
                }
            }
            
        }
    }
}
