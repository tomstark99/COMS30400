using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/*
    This class is applied to the drop points in the second level
*/
public class Droppable : Interactable
{
    public bool isDroppedOff = false;
    public event Action BagDropped;

    // RPC call to all players letting them know that a bag has been dropped off at the drop point 
    [PunRPC]
    void DroppedOffRPC()
    {
        isDroppedOff = true;
        BagDropped();
    }

    public override void PrimaryInteraction(Character character)
    {
        // check if the drop point is empty and check if the player has a bag to drop off
        if (!isDroppedOff && !character.BagDroppedOff)
        {
            // drop off the bag onto this drop point
            character.DropOff(this);
            GetComponent<PhotonView>().RPC(nameof(DroppedOffRPC), RpcTarget.All);
        }
    }
}
