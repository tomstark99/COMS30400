using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Openable : Interactable
{
   public bool isOpened = false;
   public bool isMoving = false;
    
    void Awake() {
        isOpened = false;
    }
    
    [PunRPC]
    public void IsMoving() {
        StartCoroutine(Moving());
    }

    IEnumerator Moving() {
        isMoving = true;
        yield return new WaitForSeconds(2);
        isMoving = false;
        isOpened = !isOpened;
    }
   public override void PrimaryInteraction(Character character)
    {
        //Debug.Log("is opened is" +  isOpened);
        if(isMoving == true) 
            return;
        photonView.RPC(nameof(IsMoving),RpcTarget.All);

        if(isOpened == false)
            character.Open(this);

        else character.Close(this);
        
         
    }
}
