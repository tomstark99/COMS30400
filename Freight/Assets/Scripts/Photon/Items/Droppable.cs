using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Droppable : Interactable
{
    public bool isDroppedOff = false;

    [PunRPC]
    void DroppedOffRPC()
    {
        isDroppedOff = true;
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
