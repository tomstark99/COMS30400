using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpable : Interactable
{
    private Transform pickupDestination;
    public bool isPickedUp = false;

    
    public override void PrimaryInteraction(Character character)
    {
        if (!isPickedUp)
        {
            character.PickUp(this);
        }
    }

    public override void SecondaryInteraction(Character character)
    {
        if (isPickedUp)
        {
            character.Drop(this);
        }
    }

    public bool CanInteract(Character character)
    {
        return !isPickedUp && !character.HasItem();
    }

    [PunRPC]
    public void ItemPickedUpRPC()
    {
        isPickedUp = true;
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.transform.rotation = gameObject.transform.parent.rotation;
    }

    public void ItemPickedUp()
    {
        GetComponent<PhotonView>().RPC("ItemPickedUpRPC", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    [PunRPC]
    public void ItemDroppedRPC()
    {
        isPickedUp = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void ItemDropped(Character character)
    {
        GetComponent<PhotonView>().RPC("ItemDroppedRPC", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

}
