using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openable : Interactable
{
   public bool isOpened = false;

   public override void PrimaryInteraction(Character character)
    {
        isOpened = !isOpened;
        if(isOpened == false)
            character.Open(this);
        else character.Close(this);
        
        
    }
}
