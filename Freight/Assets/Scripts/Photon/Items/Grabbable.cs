using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grabbable : PickUpable
{
    public event Action BagPickedUp;

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

    public override void PrimaryInteractionOff(Character character)
    {

    }
}
