using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Draggable : PickUpable
{   
    // does the primary interaction on draggable class
    public override void PrimaryInteraction(Character character)
    {
        // if item isn't picked up, drag it 
        if (!isPickedUp)
        {
            character.Drag(this);
        }
    }
}
