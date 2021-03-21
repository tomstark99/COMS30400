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

    public override void PrimaryInteractionOff(Character character)
    {
        if (isPickedUp)
        {
            character.Drop(this);
        }
    }

    /// <summary> Checks if the item is in a pickup destination, if so it is
    /// picked up.  </summary>
    public bool CanInteract(Character character)
    {
        return !isPickedUp && !character.HasItem();
    }

    [PunRPC]
    public void SetItemPickupConditionsRPC()
    {
        isPickedUp = true;
        // Disable the box collider to prevent collisions whilst carrying item.
        // Also turn off gravity on item and freeze its Rigidbody.
       // GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.transform.rotation = gameObject.transform.parent.rotation;
    }

    public void SetItemPickupConditions()
    {
        GetComponent<PhotonView>().RPC("SetItemPickupConditionsRPC", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    [PunRPC]
    public void ResetItemConditionsRPC()
    {
        isPickedUp = false;
       // GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void ResetItemConditions(Character character)
    {
        GetComponent<PhotonView>().RPC("ResetItemConditionsRPC", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
      // Set velocity of box after it is putdown to the speed to the character moving it
    }

    void Update() 
    {

    }
}
