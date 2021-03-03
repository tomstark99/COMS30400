using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Transform pickUpDestination;
    public PickUpable currentHeldItem;
    
    public void PickUp(PickUpable Item) 
    {
        currentHeldItem = Item;

        // An item can only be moved by a player if they are the owner.
        // Therefore, give ownership of the item to the local player before
        // moving it.
        PhotonView view = item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        item.SetItemPickupConditions();

        // Move to players pickup destination.
        item.transform.position = pickupDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        item.transform.parent = pickupDestination;
    }


    public void PutDown(PickUpable item) 
    {

        currentHeldItem = null;
        item.ResetItemConditions(this);

        item.transform.parent = GameObject.Find("/Environment/Interactables").transform;
    }

   public virtual Vector3 Velocity() 
   {
    return GetComponent<CharacterController>().velocity;
   }
}
