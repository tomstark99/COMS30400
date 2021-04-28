using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Droppable : Interactable
{
    public bool isDroppedOff = false;
    public event Action BagDropped;

    [PunRPC]
    void DroppedOffRPC()
    {
        isDroppedOff = true;
        BagDropped();
    }

    public override void PrimaryInteraction(Character character)
    {
        if (!isDroppedOff && !character.BagDroppedOff)
        {
            character.DropOff(this);
            GetComponent<PhotonView>().RPC(nameof(DroppedOffRPC), RpcTarget.All);
        }
    }
}
