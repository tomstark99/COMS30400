using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TutorialManager : MonoBehaviourPunCallbacks
{
    [Header("WASD")]
    [SerializeField]
    private GameObject wSprite;
    [SerializeField]
    private GameObject aSprite;
    [SerializeField]
    private GameObject sSprite;
    [SerializeField]
    private GameObject dSprite;
    [SerializeField]
    private GameObject playerMovement;

    [Header("Mouse")]
    [SerializeField]
    private GameObject mouse;
    [SerializeField]
    private GameObject moveMouse;

    [Header("GuardDistraction")]
    [SerializeField]
    private GameObject throwRock;
    
    private GameObject WallLifts1;
    private Transform WallLifts1Transform;
    private GameObject WallLifts2;
    private Transform WallLifts2Transform;
    private GameObject WallLifts3;
    private GameObject WallLifts4;

    [Header("AdvanceToNextRoom")]
    [SerializeField]
    private GameObject advanceToNext;
    [SerializeField]
    private GameObject arrow;

    private GameObject cageGuard;
    private GameObject guardToDrag;

    [Header("Player")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject camera;

    private int tutorialCounter;
    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;
    private int keysPressed;
    private int cubesLookedAt;

    // Start is called before the first frame update
    void Start()
    {
        tutorialCounter = 0;
        keysPressed = 0;
        Debug.Log(GameObject.Find("Room1"));
        WallLifts1 = GameObject.Find("Room1").transform.GetChild(2).gameObject;
        WallLifts1Transform = WallLifts1.transform;
        WallLifts2 = GameObject.Find("Room2").transform.GetChild(1).gameObject;
        WallLifts2Transform = WallLifts2.transform;
        cageGuard = GameObject.Find("Guards").transform.GetChild(0).gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialCounter == 0)
        {
            if (Input.GetKeyDown(KeyCode.W) && !wPressed)
            {
                wPressed = true;
                keysPressed++;
                wSprite.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.A) && !aPressed)
            {
                aPressed = true;
                keysPressed++;
                aSprite.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.S) && !sPressed)
            {
                sPressed = true;
                keysPressed++;
                sSprite.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.D) && !dPressed)
            {
                dPressed = true;
                keysPressed++;
                dSprite.SetActive(true);
            }
            if (keysPressed == 4)
            {
                tutorialCounter++;
                Destroy(playerMovement);
                mouse.SetActive(true);
            }
        }
        else if (tutorialCounter == 1)
        {
            if (mouse.activeSelf == false)
            {
                tutorialCounter++;
                moveMouse.SetActive(true);
            }
        }
        else if (tutorialCounter == 2)
        {
            // shoots out a raycast to see if user looks at cube
            Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);

            if(hitInfo.collider != null)
                if (hitInfo.collider.gameObject.tag == "Cube")
                {
                    cubesLookedAt++;
                    Destroy(hitInfo.collider.gameObject);
                }

            if (cubesLookedAt == 2)
            {
                tutorialCounter++;
                Destroy(moveMouse);
            }
        } else if(tutorialCounter == 3) {

            if(transform.position.z < 93) 
            {
                if (advanceToNext.activeSelf == false)
                    advanceToNext.SetActive(true);

                WallLifts1.transform.position = new Vector3(WallLifts1.transform.position.x , WallLifts1.transform.position.y + 0.05f, WallLifts1.transform.position.z);
            }
            else 
            {
                advanceToNext.SetActive(false);
                WallLifts1.transform.position = new Vector3(WallLifts1Transform.position.x, 3, WallLifts1Transform.position.z);
                tutorialCounter++;
                throwRock.SetActive(true);
            }
        } else if (tutorialCounter == 4)
        {
            // once guard gets alerted by rock
            if (cageGuard.GetComponent<GuardAIPhoton>().GuardState == GuardAIPhoton.State.Alerted)
            {
                tutorialCounter++;
                Destroy(throwRock);
            }
        } else if (tutorialCounter == 5)
        {
            if (transform.position.z < 142)
            {
                if (advanceToNext.activeSelf == false)
                    advanceToNext.SetActive(true);

                WallLifts2.transform.position = new Vector3(WallLifts2.transform.position.x, WallLifts2.transform.position.y + 0.05f, WallLifts2.transform.position.z);
            }
            else
            {
                advanceToNext.SetActive(false);
                WallLifts2.transform.position = new Vector3(WallLifts2Transform.position.x, 3, WallLifts2Transform.position.z);
                tutorialCounter++;
            }
        } else if (tutorialCounter == 6)
        {
            //Debug.Log(GameObject.Find("Environment/Interactables/DeadGuards").GetComponentsInChildren<Transform>().Length);
            if (GameObject.Find("Environment/Interactables/DeadGuards").GetComponentsInChildren<Transform>().Length == 9)
            {
                int random = Random.Range(0, 4);
                guardToDrag = GameObject.Find("Environment/Interactables/DeadGuards").transform.GetChild(random).gameObject;
                Vector3 arrowPos = new Vector3(guardToDrag.transform.position.x, guardToDrag.transform.position.y + 5, guardToDrag.transform.position.z);
                Instantiate(arrow, arrowPos, arrow.transform.rotation);
                tutorialCounter++;
            }
        }


        //Debug.Log(tutorialCounter);
    }

}
