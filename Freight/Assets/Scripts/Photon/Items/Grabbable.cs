using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grabbable : PickUpable
{
    public override void PrimaryInteraction(Character character)
    {
        if (!isPickedUp)
        {
            character.Grab(this);
        }
    }

    public override void PrimaryInteractionOff(Character character)
    {

    }
}
