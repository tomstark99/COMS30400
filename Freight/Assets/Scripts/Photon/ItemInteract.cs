using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemInteract : MonoBehaviourPun
{
    public float maxInteractionDistance = 4f;
    [SerializeField] private Transform cameraTransform;
    private Character character;
    private RaycastHit raycastFocus;
    private bool interactableInRange = false;
    private Interactable currentInteractable;

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

        if(canInteract) 
        {
                Interactable newInteractable = raycastFocus.collider.transform.GetComponent<Interactable>();

                /* If we are already interacting with something but we are now
                trying to interact with something new, then we need to disable
                the other interaction (turn off its glow).*/

                currentInteractable = newInteractable;

            if (currentInteractable != null && currentInteractable.CanInteract(character)) 
            {
                    // If we are able to interact with the new interactable then turn on its glow

                    // If we are pressing mouse down then do the interaction
                    if (Input.GetKeyDown(KeyCode.F)) 
                    {
                    // Do whatever the primary interaction of this interactable is.
                    currentInteractable.PrimaryInteraction(character);
                    }
            }
        }
     // Otherwise if we cant interact with anything but we were previously
        // interacting with something.
        else if (currentInteractable != null) 
        {
            // Then turn off the glow of that thing
        
        // And if bring the mouse button up
            if (Input.GetKeyDown(KeyCode.G)) 
            {

              // Some item have a primary interaction off method, eg drop the
              // item after pickup. Therefore run this on mouse up.
              currentInteractable.PrimaryInteractionOff(character);
              currentInteractable = null;
            }
        }
    }

        private void FixedUpdate() 
        {
                Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

                // Is interactable object in fron the player

                // Is interactable object detected in front of player?
            if (
            // Fire a ray out and see if we hit anything within a max distance
                Physics.Raycast(ray, out raycastFocus, maxInteractionDistance) 
            // If we hit something that is not interactalbe then it doesnt count 
            &&  raycastFocus.collider.transform.GetComponent<Interactable>() != null
            // If we hit ourselves then it also doesnt count 
            &&  raycastFocus.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()
            ) 

            {
                interactableInRange = true;
            }
            else 
            {
                interactableInRange = false;
            }
        }
}