using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
     Grabbable class applied to the bags you can pickup on the first level
*/
public class Grabbable : PickUpable
{
    public event Action BagPickedUp;

    // RPC call to all players to call event
    [PunRPC]
    void BagPickedUpRPC()
    {
        BagPickedUp();
    }

    public override void PrimaryInteraction(Character character)
    {
        if (!isPickedUp && !character.HoldingTheBag)
        {
            gameObject.GetComponent<PhotonView>().RPC(nameof(BagPickedUpRPC), RpcTarget.All);
            character.Grab(this);
        }
    }

    // empty function to override Pickupable
    public override void SecondaryInteraction(Character character)
    {

    }
}
