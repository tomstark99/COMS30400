using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Breakable : Interactable
{
    public event Action FenceBroke;

    //note to lead programmer: this is needed due to the delay in the rpc call if you spam E
    private bool isBroken;

    [PunRPC]
    void FenceBrokeRPC() {
        FenceBroke();
    }  


    public override void PrimaryInteraction(Character character)
    {
        if(isBroken == false) {
            isBroken = true;
            if(transform.tag == "BrokenFence")
                view.RPC(nameof(FenceBrokeRPC), RpcTarget.All);
            character.Break(this);
        }
        
    }
    // Sta
}
