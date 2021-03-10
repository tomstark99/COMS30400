using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : PickUpable
{
    public override void PrimaryInteractionOff(Character character)
    {
        Debug.Log("lololololool");
        if (isPickedUp)
        {
            character.Throw(this);
        }
    }
}
