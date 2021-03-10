using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviour
{
    public Transform pickUpDestination;
    public PickUpable currentHeldItem;
    
    public bool HasItem()
    {
        return currentHeldItem != null;
    }

    public void PickUp(PickUpable Item) 
    {
        currentHeldItem = Item;

        // An item can only be moved by a player if they are the owner.
        // Therefore, give ownership of the item to the local player before
        // moving it.
        PhotonView view = Item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        Item.SetItemPickupConditions();

        // Move to players pickup destination.
        Item.transform.position = pickUpDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestination;
    }


    public void Throw(Throwable Item) 
    {
        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;

        currentHeldItem = null;
        Item.ResetItemConditions(this);
        Item.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }

    public void Drop(PickUpable Item) 
    {
        currentHeldItem = null;
        Item.ResetItemConditions(this);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }
   
}
