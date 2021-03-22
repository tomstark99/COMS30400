using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Draggable : PickUpable
{
    public override void PrimaryInteraction(Character character)
    {
        if (!isPickedUp)
        {
            character.Drag(this);
        }
    }
}
