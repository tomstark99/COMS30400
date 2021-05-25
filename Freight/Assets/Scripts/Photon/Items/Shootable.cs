using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : PickUpable
{
    // function that allows player to shoot gun
    public void ShootGun(Character character)
    {
        if (isPickedUp)
        {
            character.Shoot(this);
        }
    }
}
