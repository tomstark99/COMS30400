using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ItemInteract : MonoBehaviourPun
{
    public float maxInteractionDistance = 4f;
    [SerializeField] private Transform cameraTransform;
    private Character character;

    // Start is called before the first frame update
    void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(this);
        }    
     
     character = GetComponent<Character>();

    }

    // Update is called once per frame
    void Update()
    {
        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        bool canInteract = interactableInRange && !character.HasItem();
        if(canInteract) {
        }
    }
}
