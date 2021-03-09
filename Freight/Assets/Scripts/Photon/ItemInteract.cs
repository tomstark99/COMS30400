using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemInteract : MonoBehaviourPun
{
    public float maxInteractionDistance = 4f;
    [SerializeField] private Transform cameraTransform;
    private Character character;
    private bool interactableInRange = false;
    private PickUpable currentInteractable;

    public GameObject text;

    private GameObject rocks;

    private GameObject interactableRock;
    private GameObject interactables;
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
                PickUpable newInteractable = interactableRock.GetComponent<PickUpable>();

                /* If we are already interacting with something but we are now
                trying to interact with something new, then we need to disable
                the other interaction (turn off its glow).*/

                currentInteractable = newInteractable;
               // Debug.Log("current interactable is " + currentInteractable);
            if (currentInteractable != null) 
            {
                    // If we are able to interact with the new interactable then turn on its glow

                    // If we are pressing mouse down then do the interaction
                    //Debug.Log("current interactable has a pick up script");
                    if (Input.GetKeyDown(KeyCode.E)) 
                    {
                       // Debug.Log("F was pressed");
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
            if (Input.GetMouseButtonDown(0)) 
            {

              // Some item have a primary interaction off method, eg drop the
              // item after pickup. Therefore run this on mouse up.
              currentInteractable.PrimaryInteractionOff(character);
              currentInteractable = null;
            }
        }
    }

    [PunRPC]
    void SetPressEToActive()
    {
        text.SetActive(true);
    }

    [PunRPC]
    void SetPressEToNotActive()
    {
        text.SetActive(false);
    }
    private void FixedUpdate() 
    {

            rocks = GameObject.Find("Environment/Interactables");
            interactables = rocks.transform.GetChild(0).gameObject;
            float minimumDistanceToObject = float.MaxValue;
            foreach (Transform rock in interactables.transform)
            {
                float tempDist = Vector3.Distance(rock.transform.position, transform.position);
                if (tempDist <= 2.5f)
                {
                    photonView.RPC("SetPressEToActive", GetComponent<PhotonView>().Owner);
                    interactableInRange = true;

                    if(tempDist < minimumDistanceToObject) {
                        interactableRock = rock.gameObject;
                        minimumDistanceToObject = tempDist;
                    }
                }
                else if (tempDist > 2.5f)
                {
                    photonView.RPC("SetPressEToNotActive", GetComponent<PhotonView>().Owner);
                    interactableInRange = false;
                }
            }
            
           
    }
}