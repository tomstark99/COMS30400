using System.Collections;
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

    private GameObject interactableObject;

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
        tooltip = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Interactable collision = other.gameObject?.GetComponent<Interactable>();

        if (other.gameObject?.GetComponent<PickUpable>())
        {
            if (other.gameObject.GetComponent<PickUpable>().isPickedUp)
                return;
        }

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
                if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera == Camera.GetComponent<CinemachineVirtualCamera>())
                {
                    // if you don't have an interactable
                    if (currentInteractable == null)
                        SetPressEToActive();
                    // if you have an interactable and the new interactable is not a pickupable
                    else if (other.gameObject.GetComponent<PickUpable>() == null)
                        SetPressEToActive();
                }

                if (other.gameObject.GetComponent<Breakable>() != null)
                    SetBreakHandsActive();
            }

            interactablesInRange.Add(collision.gameObject);
        }
        if (other.tag == "Outline")
        {
            other.transform.parent.GetComponent<Outline>().enabled = true;
                
            if (other.transform.parent.gameObject?.GetComponent<PickUpable>())
            {
                if(other.transform.parent.gameObject.GetComponent<PickUpable>().isPickedUp)
                    other.transform.parent.GetComponent<Outline>().enabled = false;

                if (tooltip)
                {
                    Quaternion objRot = transform.rotation;
                    GameObject playerTooltip = Instantiate(tooltipObject, new Vector3(other.transform.position.x, other.transform.position.y + 5, other.transform.position.z), Quaternion.Euler(objRot.eulerAngles));
                    playerTooltip.GetComponent<Tooltip>().Player = gameObject;
                    tooltip = false;
                }
            }

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

        bool nullFound = false;

        GameObject pickedUpItem = null;

        foreach (var interactable in interactablesInRange)
        {
            // if object was destroyed
            if (interactable == null)
            {
                nullFound = true;
                continue;
            }
            if (interactable?.GetComponent<PickUpable>())
            {
                if (interactable.GetComponent<PickUpable>().isPickedUp)
                {
                    pickedUpItem = interactable;
                    continue;
                }
            }
            float tempDist = Vector3.Distance(interactable.transform.position, transform.position);

            if (closestInteractable == null || tempDist < lastDistance)
            {
                closestInteractable = interactable;
                lastDistance = tempDist;
            }

        }

        // fence broken
        if (nullFound)
        {
            interactablesInRange.RemoveAll(item => item == null);
            SetBreakHandsInactive();
            SetPressEToNotActive();
        }


        if (pickedUpItem != null)
        {
            pickedUpItem.GetComponent<Outline>().enabled = false;
            interactablesInRange.Remove(pickedUpItem);
        }
            

        return closestInteractable;
    }

    // Update is called once per frame
    void Update()
    {
        if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera != Camera.GetComponent<CinemachineVirtualCamera>())
            return;

        bool canInteract = (interactablesInRange.Count > 0) && !character.HasItem();

        foreach (var inte in interactablesInRange)
        {
           // Debug.Log(inte);
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
                //Debug.Log("Interactable is null");
            }


            //currentInteractable = newInteractable;

            if (newInteractable != null)
            {

                if (((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P") == 0)))
                {
                    if (newInteractable.GetComponent<Breakable>() != null)
                    {
                        //photonView.RPC("SetBreakHandsInactive", GetComponent<PhotonView>().Owner);
                        SetPressEToNotActive();
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
                        SetPressEToNotActive();
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
        // if we are holding something, we are limited to the possible interactions
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
                //Debug.Log("rock is null");
            }

            try
            {
                newSwitch = interactableObject.GetComponent<Switchable>();
            }
            catch
            {
               // Debug.Log("switch is null");
            }

            try
            {
                dropBag = interactableObject.GetComponent<Droppable>();
            }
            catch
            {
                //Debug.Log("switch is null");
            }

            try
            {
                breakableObject = interactableObject.GetComponent<Breakable>();
            }
            catch
            {
               // Debug.Log("breakable is null");
            }

            try
            {
                openableObject = interactableObject.GetComponent<Openable>();

            }
            catch
            {
                //Debug.Log("Opanable is null");
            }
            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P") == 0) && openableObject != null)
            {
                openableObject.PrimaryInteraction(character);
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B") == 0) && newBag != null)
            {
                newBag.PrimaryInteraction(character);
                SetPressEToNotActive();
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B") == 0) && newSwitch != null)
            {
                newSwitch.PrimaryInteraction(character);
                SetPressEToNotActive();
            }

            if (Input.GetKeyDown(KeyCode.E) && dropBag != null)
            {
                dropBag.PrimaryInteraction(character);
                SetPressDropToNotActive();
            }

            if (Input.GetKeyDown(KeyCode.E) && breakableObject != null)
            {
                breakableObject.PrimaryInteraction(character);
                SetPressEToNotActive();
                SetBreakHandsInactive();
            }
            // press G to drop/throw item
            if (Input.GetKeyDown(KeyCode.G))
            {
                // we drop/throw item and turn off its outline
                currentInteractable.PrimaryInteractionOff(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                SetPressEToActive();
                interactablesInRange.Add(currentInteractable.gameObject);
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
               // Debug.Log(currentInteractable);
                currentInteractable.GetComponent<Shootable>().ShootGun(character);
            }
        }
    }
}
