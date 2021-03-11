using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : PickUpable
{
    public override void PrimaryInteractionOff(Character character)
    {
        if (isPickedUp)
        {
            character.Drop(this);
        }
    }

    public void ShootGun(Character character)
    {
        if (isPickedUp)
        {
            character.Shoot(this);
        }
    }
}
