﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class ItemInteraction : MonoBehaviourPun
{
    public GameObject Camera;

    private CinemachineBrain cinemachineBrain;

    private Character character;
    [SerializeField]
    private Interactable currentInteractable;

    [SerializeField]
    private GameObject tooltipObject;

    public GameObject text;

    public GameObject leftHand;
    public GameObject rightHand;

    [SerializeField]
    private GameObject textDrop;

    private GameObject rocks;

    private GameObject interactableObject;
    private GameObject interactables;

    private int tooltipCount;

    private bool handsActive = false;

    private bool tooltip;

    private List<GameObject> interactablesInRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(this);
        }
        cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
        character = GetComponent<Character>();
        tooltipCount = 0;
        tooltip = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Interactable collision = other.gameObject?.GetComponent<Interactable>();
        if (collision != null)
        {
            if (collision.gameObject.GetComponent<Droppable>() != null)
            {
                if (!collision.gameObject.GetComponent<Droppable>().isDroppedOff)
                {
                    SetPressDropToActive();
                }
            }
            else
            {
                if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera == Camera.GetComponent<CinemachineVirtualCamera>() || currentInteractable == null)
                    SetPressEToActive();
                if (other.gameObject.GetComponent<Breakable>() != null)
                    SetBreakHandsActive();
            }

            interactablesInRange.Add(collision.gameObject);
        }
        if (other.tag == "Outline")
        {
            other.transform.parent.GetComponent<Outline>().enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Interactable collision = other.gameObject?.GetComponent<Interactable>();
        if (collision != null)
        {
            if (collision.gameObject.GetComponent<Droppable>() != null)
            {
                if (!collision.gameObject.GetComponent<Droppable>().isDroppedOff)
                {
                    SetPressDropToNotActive();
                }
            }
            else
            {
                
                SetPressEToNotActive();
                if (other.gameObject.GetComponent<Breakable>() != null)
                    SetBreakHandsInactive();
            }
           

            interactablesInRange.Remove(collision.gameObject);
        }
        if (other.tag == "Outline")
        {
            other.transform.parent.GetComponent<Outline>().enabled = false;
        }
    }

    void SetPressEToActive()
    {
        text.SetActive(true);
    }

    //[PunRPC]
    void SetPressEToNotActive()
    {
        text.SetActive(false);
    }

    //[PunRPC]
    void SetPressDropToActive()
    {
        textDrop.SetActive(true);
    }

    //[PunRPC]
    void SetPressDropToNotActive()
    {
        textDrop.SetActive(false);
    }

    //[PunRPC]
    void SetBreakHandsActive()
    {
        if (!handsActive)
        {
            if (leftHand ?? false)
                leftHand.SetActive(true);
            if (rightHand ?? false)
                rightHand.SetActive(true);
            handsActive = true;
        }

    }

    //[PunRPC]
    void SetBreakHandsInactive()
    {
        if (handsActive)
        {
            if (leftHand ?? false)
                leftHand.SetActive(false);
            if (rightHand ?? false)
                rightHand.SetActive(false);
            Destroy(leftHand);
            Destroy(rightHand);
            handsActive = false;
        }
    }

    GameObject GetClosestInteractable()
    {
        GameObject closestInteractable = null;
        float lastDistance = float.MaxValue;

        foreach (var interactable in interactablesInRange)
        {

            float tempDist = Vector3.Distance(interactable.transform.position, transform.position);

            if (closestInteractable == null || tempDist < lastDistance)
            {
                closestInteractable = interactable;
                lastDistance = tempDist;
            }

        }

        return closestInteractable;
    }

    // Update is called once per frame
    void Update()
    {
        if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera != Camera.GetComponent<CinemachineVirtualCamera>())
            return;

        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        bool canInteract = (interactablesInRange.Count > 0) && !character.HasItem();

        foreach (var inte in interactablesInRange)
        {
            Debug.Log(inte);
        }

        interactableObject = GetClosestInteractable();

        if (canInteract)
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
                if (((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P") == 0)))
                {
                    if (newInteractable.GetComponent<Breakable>() != null)
                    {
                        interactablesInRange.Remove(interactableObject);
                        //photonView.RPC("SetBreakHandsInactive", GetComponent<PhotonView>().Owner);
                        SetBreakHandsInactive();
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }
                //d is for opening 
                //u is closing

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (newInteractable.GetComponent<Openable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                if (PoseParser.GETGestureAsString().CompareTo("D") == 0)
                {
                    if (newInteractable.GetComponent<Openable>() != null && newInteractable.GetComponent<Openable>().isOpened == false)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                if (PoseParser.GETGestureAsString().CompareTo("U") == 0)
                {
                    if (newInteractable.GetComponent<Openable>() != null && newInteractable.GetComponent<Openable>().isOpened == true)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B") == 0) && newInteractable.GetComponent<Breakable>() == null && newInteractable.GetComponent<Openable>() == null)
                {
                    if (newInteractable.GetComponent<Switchable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                    }
                    else if (newInteractable.GetComponent<Droppable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                        //photonView.RPC("SetPressDropToNotActive", GetComponent<PhotonView>().Owner);
                        SetPressDropToNotActive();
                    }
                    else
                    {
                        SetPressEToNotActive();
                        currentInteractable = newInteractable;
                        currentInteractable.GetComponent<Outline>().enabled = false;

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
            Droppable dropBag = null;
            Breakable breakableObject = null;
            Openable openableObject = null;
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

            try
            {
                dropBag = interactableObject.GetComponent<Droppable>();
            }
            catch
            {
                Debug.Log("switch is null");
            }

            try
            {
                breakableObject = interactableObject.GetComponent<Breakable>();
            }
            catch
            {
                Debug.Log("breakable is null");
            }

            try
            {
                openableObject = interactableObject.GetComponent<Openable>();

            }
            catch
            {
                Debug.Log("Opanable is null");
            }
            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P") == 0) && openableObject != null)
            {
                openableObject.PrimaryInteraction(character);
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B") == 0) && newBag != null)
            {
                newBag.PrimaryInteraction(character);
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B") == 0) && newSwitch != null)
            {
                newSwitch.PrimaryInteraction(character);
            }

            if (Input.GetKeyDown(KeyCode.E) && dropBag != null)
            {
                dropBag.PrimaryInteraction(character);
                //photonView.RPC("SetPressDropToNotActive", GetComponent<PhotonView>().Owner);
                SetPressDropToNotActive();
            }

            if (Input.GetKeyDown(KeyCode.E) && breakableObject != null)
            {
                interactablesInRange.Remove(interactableObject);
                breakableObject.PrimaryInteraction(character);
                SetPressEToNotActive();
            }
            // press G to drop/throw item
            if (Input.GetKeyDown(KeyCode.G))
            {
                // we drop/throw item and turn off its outline
                currentInteractable.PrimaryInteractionOff(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                SetPressEToActive();
                currentInteractable = null;
            }
            else

            if ((Input.GetMouseButtonDown(0) || PoseParser.GETGestureAsString().CompareTo("R") == 0) && currentInteractable.GetComponent<Throwable>() != null)
            {
                currentInteractable.GetComponent<Throwable>().ThrowRock(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                currentInteractable = null;
            }
            else
            // if item is shootable
            if (Input.GetMouseButtonDown(0) && currentInteractable.GetComponent<Shootable>() != null)
            {
                Debug.Log(currentInteractable);
                currentInteractable.GetComponent<Shootable>().ShootGun(character);
            }
        }
    }
}