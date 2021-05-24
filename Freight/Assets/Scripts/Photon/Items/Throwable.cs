using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : PickUpable
{
    public override void SecondaryInteraction(Character character)
    {
        if (isPickedUp)
        {
            character.Drop(this);
        }
    }

    public void ThrowRock(Character character)
    {
        if (isPickedUp)
        {
            character.Throw(this);
        }
    }
}
