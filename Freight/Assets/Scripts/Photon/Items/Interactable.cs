using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Interactable : MonoBehaviourPun
{
    protected PhotonView view;
    
    // interaction that makes you stop interacting with the object
    public virtual void SecondaryInteraction(Character character) {}

    // interaction that makes you interact with the object
    public virtual void PrimaryInteraction(Character character) {}
    
    public virtual void Start() 
    {
        view = GetComponent<PhotonView>();
    }
}
