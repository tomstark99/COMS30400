using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : PickUpable
{

    public void ShootGun(Character character)
    {
        if (isPickedUp)
        {
            character.Shoot(this);
        }
    }
}
