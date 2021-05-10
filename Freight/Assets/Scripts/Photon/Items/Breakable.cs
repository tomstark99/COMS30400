using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Breakable : Interactable
{
    public event Action FenceBroke;

    [PunRPC]
    void FenceBrokeRPC() {
        FenceBroke();
    }
    public override void PrimaryInteraction(Character character)
    {
        if(transform.tag == "BrokenFence")
            view.RPC(nameof(FenceBrokeRPC), RpcTarget.All);
        character.Break(this);
    }
    // Sta
}
