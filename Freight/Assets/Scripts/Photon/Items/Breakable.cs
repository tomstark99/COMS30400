using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Breakable : Interactable
{
    public override void PrimaryInteraction(Character character)
    {
        character.Break(this);
    }
    // Sta
}
