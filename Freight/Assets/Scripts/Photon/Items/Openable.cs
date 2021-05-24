using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Openable : Interactable
{
   public bool isOpened = false;
   public bool isMoving = false;
    
    void Awake() 
    {
        isOpened = false;
    }
    
    // RPC call to tell all players door is in process of moving
    [PunRPC]
    public void IsMoving() 
    {
        StartCoroutine(Moving());
    }

    // coroutine to prevent people from spam opening and closing the doors
    IEnumerator Moving() 
    {
        isMoving = true;
        yield return new WaitForSeconds(2);
        isMoving = false;
        isOpened = !isOpened;
    }

    public override void PrimaryInteraction(Character character)
    {
        // return early if door is animating
        if(isMoving == true) 
            return;

        photonView.RPC(nameof(IsMoving),RpcTarget.All);

        // open or close the door depending on its current state
        if(!isOpened)
            character.Open(this);
        else 
            character.Close(this);
        
         
    }
}
