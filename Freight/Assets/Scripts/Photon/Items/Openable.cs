using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Openable : Interactable
{
   public bool isOpened = false;
   public bool isMoving = false;
    
    [PunRPC]
    public void IsMoving() {
        StartCoroutine(Moving());
    }

    IEnumerator Moving() {
        isMoving = true;
        yield return new WaitForSeconds(2);
        isMoving = false;
    }
   public override void PrimaryInteraction(Character character)
    {
        if(isMoving == true) 
            return;
        photonView.RPC(nameof(IsMoving),RpcTarget.All);
        isOpened = !isOpened;
        if(isOpened == false)
            character.Open(this);
        else character.Close(this);
        
        
    }
}
