using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
    Class applied onto the laptop that turns off the lights 
*/
public class Switchable : Interactable
{
    public bool isSwitchedOff = false;

    // used to switch the lights on and off
    public override void PrimaryInteraction(Character character)
    {
        character.SwitchOff(this);
    }
}
