using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Breakable : Interactable
{
    public event Action FenceBroke;

    // note to lead programmer: this is needed due to the delay in the rpc call if you spam E
    private bool isBroken;

    // RPC to all players to call event of fence being broken
    [PunRPC]
    void FenceBrokeRPC() 
    {
        FenceBroke();
    }  

    // calls the primary interaction with the breakable object
    public override void PrimaryInteraction(Character character)
    {
        // if fence has not yet been broken, call RPC to all players notifying them of the fence breaking and then actually break the fence through the Character script
        if(isBroken == false) {
            isBroken = true;
            if(transform.tag == "BrokenFence")
                view.RPC(nameof(FenceBrokeRPC), RpcTarget.All);
            character.Break(this);
        }
        
    }
}
