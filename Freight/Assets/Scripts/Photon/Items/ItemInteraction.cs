using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

//Attached on the player prefab. Manages the behaviour of the interactables with the player
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
    
    //on trigger enter with the player
    void OnTriggerEnter(Collider other)
    {
        //try to get interacatable component for the collision if any
        Interactable collision = other.gameObject?.GetComponent<Interactable>();

        //try to get component pickabalbe
        if (other.gameObject?.GetComponent<PickUpable>())
        {

            //if there is a component pickable and the object has been picked up, return
            if (other.gameObject.GetComponent<PickUpable>().isPickedUp)
                return;
        }

        //if collision is an object with an interable component on it
        if (collision != null)
        {

            //if the gameObject is droppable
            if (collision.gameObject.GetComponent<Droppable>() != null)
            {
                //if the object is not dropped off
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
            
            //add gameObject to list of interables in range
            interactablesInRange.Add(collision.gameObject);
        }

        //handle the outline component (there is a second bigger collider on all interable object with the tab outline)
        if (other.tag == "Outline")
        {
            other.transform.parent.GetComponent<Outline>().enabled = true;
                
            if (other.transform.parent.gameObject?.GetComponent<PickUpable>())
            {
                if(other.transform.parent.gameObject.GetComponent<PickUpable>().isPickedUp)
                    other.transform.parent.GetComponent<Outline>().enabled = false;

                //show a tooltip on the first interactable object you encounter
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

    //on trigger exit 
    void OnTriggerExit(Collider other)
    {
        //if component interacble is different then null
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

            //remove interactable from list of interactables in range
            interactablesInRange.Remove(collision.gameObject);
        }
        //deactivate the outline
        if (other.tag == "Outline")
        {
            other.transform.parent.GetComponent<Outline>().enabled = false;
        }
    }

    //activate press E to pick up object on canvas
    void SetPressEToActive()
    {
        text.SetActive(true);
    }

    //deactivate press E to pick up object on canvas
    void SetPressEToNotActive()
    {
        text.SetActive(false);
    }

    //activate press E to drop object on canvas
    void SetPressDropToActive()
    {
        textDrop.SetActive(true);
    }

   //deactivate press E to drop object on canvas
    void SetPressDropToNotActive()
    {
        textDrop.SetActive(false);
    }

    //set the hands UI active for fence breaking
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

    //set the hands UI inactive for fence breaking
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

    //returns the closest interactable from the interactables in range list
    GameObject GetClosestInteractable()
    {
        GameObject closestInteractable = null;
        float lastDistance = float.MaxValue;

        bool nullFound = false;

        GameObject pickedUpItem = null;

        //loop trought the interactables

        foreach (var interactable in interactablesInRange)
        {
            // if object was destroyed
            if (interactable == null)
            {
                nullFound = true;
                continue;
            }

            //if component has pickable component and the item is pickedUp. Set pickedUpitem to the interactable (eg. some rock in hand)
            if (interactable?.GetComponent<PickUpable>())
            {
                if (interactable.GetComponent<PickUpable>().isPickedUp)
                {
                    pickedUpItem = interactable;
                    continue;
                }
            }

            //calculate the distance from interactable to the player
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
        //if the camera is not on the main virtual camera it means it is on an animation, so return 
        if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera != Camera.GetComponent<CinemachineVirtualCamera>())
            return;

        bool canInteract = (interactablesInRange.Count > 0) && !character.HasItem();

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


            //if the interactable object has an interactable component

            if (newInteractable != null)
            {
                //if E is pressed or P from the pose parser
                //if the object is breakable

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
                //if E is pressed close or open the door
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (newInteractable.GetComponent<Openable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                //if doing a opening pose - open
                if (PoseParser.GETGestureAsString().CompareTo("D") == 0)
                {
                    if (newInteractable.GetComponent<Openable>() != null && newInteractable.GetComponent<Openable>().isOpened == false)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }
                
                //if doing a closing pose - close
                if (PoseParser.GETGestureAsString().CompareTo("U") == 0)
                {
                    if (newInteractable.GetComponent<Openable>() != null && newInteractable.GetComponent<Openable>().isOpened == true)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                // if e or b on the pose
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
                currentInteractable.SecondaryInteraction(character);
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
