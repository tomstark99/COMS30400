using System;
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
    private bool bagInRange = false;
    [SerializeField]
    private Interactable currentInteractable;

    [SerializeField]
    private GameObject tooltipObject;

    public GameObject text;

    private GameObject rocks;

    private GameObject interactableObject;
    private GameObject interactables;

    private bool tooltip;
    // Start is called before the first frame update
    void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(this);
        }

        character = GetComponent<Character>();
        tooltip = false;
    }

    void DisplayTooltip()
    {

    }

    // Update is called once per frame
    void Update()
    {


        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        bool canInteract = interactableInRange && !character.HasItem();

        if(canInteract)
        {
            Interactable newInteractable = null;

            try
            {
                 newInteractable = interactableObject.GetComponent<Interactable>();
            }
            catch
            {
                Debug.Log("Interactable is null");
            }
           

            //currentInteractable = newInteractable;

            if (newInteractable != null) 
            {

                // If we are pressing mouse down then do the interaction
                //Debug.Log("current interactable has a pick up script");
                if (Input.GetKeyDown(KeyCode.E)) 
                {
                    if (newInteractable.GetComponent<Switchable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                    }
                    else
                    {
                        currentInteractable = newInteractable;
                        currentInteractable.GetComponent<Outline>().enabled = false;
                        // Debug.Log("F was pressed");
                        // Do whatever the primary interaction of this interactable is.
                        currentInteractable.PrimaryInteraction(character);
                    }

                }
            }
        }
        // Otherwise if we cant interact with anything but we were previously
        // interacting with something.
        else if (currentInteractable != null)
        {
            // check if there is a bag nearby as we can still pickup bags if we are holding an item
            Grabbable newBag = null;
            Switchable newSwitch = null;

            try
            {
                newBag = interactableObject.GetComponent<Grabbable>();
            }
            catch
            {
                Debug.Log("rock is null");
            }

            try
            {
                newSwitch = interactableObject.GetComponent<Switchable>();
            }
            catch
            {
                Debug.Log("switch is null");
            }

            if (Input.GetKeyDown(KeyCode.E) && newBag != null)
            {
                newBag.PrimaryInteraction(character);
            }

            if (Input.GetKeyDown(KeyCode.E) && newSwitch != null)
            {
                newSwitch.PrimaryInteraction(character);
            }

            // press G to drop/throw item
            if (Input.GetKeyDown(KeyCode.G)) 
            {
                // we drop/throw item and turn off its outline
                currentInteractable.PrimaryInteractionOff(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                currentInteractable = null;
            }

            // if item is shootable
            if (Input.GetMouseButtonDown(0) && currentInteractable.GetComponent<Shootable>() != null) 
            {
                Debug.Log(currentInteractable);
                currentInteractable.GetComponent<Shootable>().ShootGun(character);
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

        interactables = GameObject.Find("Environment/Interactables");

        if (interactables == null)
        {
            return;
        }
        
        //rocks = rocks.transform.GetChild(0).gameObject;
        float minimumDistanceToObject = float.MaxValue;
        bool found = false;
        foreach(Transform interactable in interactables.transform) 
        {
            foreach (Transform interact in interactable.transform)
            {
                
                float tempDist = Vector3.Distance(interact.transform.position, transform.position);
                if(tempDist <= 20f && interact.GetComponent<Outline>() != null) 
                {
                    interact.GetComponent<Outline>().enabled = true;
                    if (!tooltip)
                    {
                        Quaternion objRot = transform.rotation;
                        GameObject playerTooltip = Instantiate(tooltipObject, new Vector3(interact.position.x, interact.position.y + 5, interact.position.z), Quaternion.Euler(objRot.eulerAngles));
                        playerTooltip.GetComponent<Tooltip>().Player = gameObject;
                        tooltip = true;
                    }
                } 
                else 
                {
                    if(interact.GetComponent<Outline>().enabled == true)
                        interact.GetComponent<Outline>().enabled = false;
                }
                if (tempDist <= 2.5f)
                {
                    photonView.RPC("SetPressEToActive", GetComponent<PhotonView>().Owner);
                    interactableInRange = true;

                    if(tempDist < minimumDistanceToObject) {
                        interactableObject = interact.gameObject;
                        minimumDistanceToObject = tempDist;
                    }
                    found = true;

                }
                else if (tempDist > 2.5f && found == false)
                {
                    photonView.RPC("SetPressEToNotActive", GetComponent<PhotonView>().Owner);
                    interactableInRange = false;
                }

                
            }
        }
            
            
           
    }
}
