using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
