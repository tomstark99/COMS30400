using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class ItemInteract : MonoBehaviourPun
{
    public GameObject Camera;

    private CinemachineBrain cinemachineBrain;
    public float maxInteractionDistance = 4f;

    [SerializeField]
    private Transform cameraTransform;

    private Character character;
    private bool interactableInRange = false;
    private bool bagInRange = false;
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
    // Start is called before the first frame update
    void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(this);
        }
        cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
        character = GetComponent<Character>();
        tooltipCount = 0;
        tooltip = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera != Camera.GetComponent<CinemachineVirtualCamera>())
            return;

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
                //Debug.Log("Interactable is null");
            }
           

            //currentInteractable = newInteractable;

            if (newInteractable != null) 
            {

                //Debug.Log("current interactable has a pick up script");
                if(((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P")==0))) {
                    if(newInteractable.GetComponent<Breakable>() != null) {
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
                    if(newInteractable.GetComponent<Openable>() != null)
                    {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                } 

                if(PoseParser.GETGestureAsString().CompareTo("D") == 0) {
                    if(newInteractable.GetComponent<Openable>() != null  && newInteractable.GetComponent<Openable>().isOpened == false) {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                } 

                if(PoseParser.GETGestureAsString().CompareTo("U") == 0) {
                    if(newInteractable.GetComponent<Openable>() != null  && newInteractable.GetComponent<Openable>().isOpened == true) {
                        newInteractable.PrimaryInteraction(character);
                        return;
                    }
                }

                if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B")==0) && newInteractable.GetComponent<Breakable>() == null && newInteractable.GetComponent<Openable>() == null) 
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
                        currentInteractable = newInteractable;
                        currentInteractable.GetComponent<Outline>().enabled = false;
                        // Debug.Log("F was pressed");

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
                //Debug.Log("switch is null");
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
                //Debug.Log("breakable is null");
            }

            try 
            {
                openableObject = interactableObject.GetComponent<Openable>();

            }
            catch 
            {
                //Debug.Log("Opanable is null");
            }
            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("P")==0) && openableObject != null)
            {
                openableObject.PrimaryInteraction(character);
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B")==0) && newBag != null)
            {
                newBag.PrimaryInteraction(character);
            }

            if ((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B")==0) && newSwitch != null)
            {
                newSwitch.PrimaryInteraction(character);
            }

            if (Input.GetKeyDown(KeyCode.E) && dropBag != null)
            {
                dropBag.PrimaryInteraction(character);
                //photonView.RPC("SetPressDropToNotActive", GetComponent<PhotonView>().Owner);
                SetPressDropToNotActive();
            }

            if(Input.GetKeyDown(KeyCode.E) && breakableObject != null) {
                breakableObject.PrimaryInteraction(character);
                
            }
            // press G to drop/throw item
            if (Input.GetKeyDown(KeyCode.G)) 
            {
                // we drop/throw item and turn off its outline
                currentInteractable.PrimaryInteractionOff(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                currentInteractable = null;
            } else

            if ((Input.GetMouseButtonDown(0) || PoseParser.GETGestureAsString().CompareTo("R") ==0) && currentInteractable.GetComponent<Throwable>() != null) 
            {
                currentInteractable.GetComponent<Throwable>().ThrowRock(character);
                currentInteractable.GetComponent<Outline>().enabled = true;
                currentInteractable = null;
            } else
            // if item is shootable
            if (Input.GetMouseButtonDown(0) && currentInteractable.GetComponent<Shootable>() != null) 
            {
                //Debug.Log(currentInteractable);
                currentInteractable.GetComponent<Shootable>().ShootGun(character);
            }
        }
    }

    //[PunRPC]
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
            if(leftHand ?? false)
                leftHand.SetActive(true);
            if(rightHand ?? false)
                rightHand.SetActive(true);
            handsActive = true;
        }

    }

    //[PunRPC]
    void SetBreakHandsInactive() 
    {
        if (handsActive)
        {
            if(leftHand ?? false)
                leftHand.SetActive(false);
            if(rightHand ?? false)
                rightHand.SetActive(false);
            Destroy(leftHand);
            Destroy(rightHand);
            handsActive = false;
        }
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

                if (interact.GetComponent<Droppable>() != null)
                {
                    if (!interact.GetComponent<Droppable>().isDroppedOff)
                    {
                        if (tempDist <= 10f && interact.GetComponent<Droppable>() != null)
                        {
                            SetPressDropToActive();
                            interactableInRange = true;
                            if (tempDist < minimumDistanceToObject)
                            {
                                interactableObject = interact.gameObject;
                                minimumDistanceToObject = tempDist;
                            }
                            found = true;
                        }
                        else if (tempDist > 10f && found == false)
                        {
                            SetPressDropToNotActive();
                            interactableInRange = false;
                        }
                    }
                }
                else
                {
                    if (tempDist <= 20f && interact.GetComponent<Outline>() != null)
                    {
                        interact.GetComponent<Outline>().enabled = true;
                        if (tooltip)
                        {
                            Quaternion objRot = transform.rotation;
                            GameObject playerTooltip = Instantiate(tooltipObject, new Vector3(interact.position.x, interact.position.y + 5, interact.position.z), Quaternion.Euler(objRot.eulerAngles));
                            playerTooltip.GetComponent<Tooltip>().Player = gameObject;
                            tooltip = false;
                        }
                    }
                    else
                    {
                        if (interact.GetComponent<Outline>() != null && interact.GetComponent<Outline>().enabled == true)
                            interact.GetComponent<Outline>().enabled = false;
                    }

                    if (tempDist <= 6f)
                    {
                        if (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera == Camera.GetComponent<CinemachineVirtualCamera>())
                            SetPressEToActive();
                        if (interact.GetComponent<Breakable>() != null)
                            SetBreakHandsActive();
                        interactableInRange = true;

                        if (tempDist < minimumDistanceToObject)
                        {
                            interactableObject = interact.gameObject;
                            minimumDistanceToObject = tempDist;
                        }
                        found = true;

                    }
                    else if (tempDist > 6f && found == false)
                    {
                        if (interact.GetComponent<Breakable>() != null)
                            SetBreakHandsInactive();

                        SetPressEToNotActive();
                        interactableInRange = false;
                    }
                }
                
            }
        }
            
            
           
    }
}
