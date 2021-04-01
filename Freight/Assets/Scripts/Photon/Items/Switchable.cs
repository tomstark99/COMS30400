using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Switchable : Interactable
{
    public bool isSwitchedOff = false;

    public override void PrimaryInteraction(Character character)
    {
        if (!isSwitchedOff)
        {
            character.SwitchOff(this);
        }
    }

    [PunRPC]
    void SetSwitchToOffRPC()
    {
        isSwitchedOff = true;
    }

    public void SetSwitchToOff()
    {
        GetComponent<PhotonView>().RPC(nameof(SetSwitchToOffRPC), RpcTarget.All);
    }
}
