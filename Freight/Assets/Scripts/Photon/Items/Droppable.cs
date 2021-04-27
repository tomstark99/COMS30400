using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Droppable : Interactable
{
    bool isDroppedOff = false;

    [PunRPC]
    void SetDroppedOffToTrue()
    {
        isDroppedOff = true;
    }

    public override void PrimaryInteraction(Character character)
    {
        if (!isDroppedOff)
        {
            character.DropOff(this);
            GetComponent<PhotonView>().RPC(nameof(SetDroppedOffToTrue), RpcTarget.All);
        }
    }
}
