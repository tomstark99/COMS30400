using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Switchable : Interactable
{
    public bool isSwitchedOff = false;

    public override void PrimaryInteraction(Character character)
    {
        character.SwitchOff(this);
    }
}
