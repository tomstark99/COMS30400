using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//This is the main script the runs the tutorial 
public class TutorialManager : MonoBehaviourPunCallbacks
{

    [Header("Audio")]
    [SerializeField]
    private GameObject helloWelcome;
    [SerializeField]
    private GameObject wasdMovementSound;
    [SerializeField]
    private GameObject pointCrosshair;
    [SerializeField]
    private GameObject proceedToTheNextTrainingLevel;
    [SerializeField]
    private GameObject congratulationForCompletingTheFirstLevel;
    [SerializeField]
    private GameObject theGuardsGetDistracted;
    [SerializeField]
    private GameObject congratulationForCompletingTheSecondLevel;
    [SerializeField]
    private GameObject pleaseWalkUpToTheGunAndPickItUp;
    [SerializeField]
    private GameObject killAll4Guards;
    [SerializeField]
    private GameObject findTheGuardWithTheGreenArrow;
    [SerializeField]
    private GameObject guardsBecomeAlertedIfSeesDeadGuards;
    [SerializeField]
    private GameObject congratulationForCompletingTheThirdLevel;
    [SerializeField]
    private GameObject proceedToTheLadder;
    [SerializeField]
    private GameObject graduatedFreight;

    [Header("WASD")]

    [SerializeField]
    private GameObject shiftPressed;

    [SerializeField]
    private GameObject wSprite;
    [SerializeField]
    private GameObject aSprite;
    [SerializeField]
    private GameObject sSprite;
    [SerializeField]
    private GameObject dSprite;

    [SerializeField]
    private GameObject unPressedKeys;

    [SerializeField]
    private GameObject pressKeysText;


    [SerializeField]
    private GameObject playerMovement;

    [SerializeField]
    private GameObject spaceUnpressedSprite;

    [SerializeField]
    private GameObject spacePressed;

    [Header("Mouse")]
    [SerializeField]
    private GameObject mouse;
    [SerializeField]
    private GameObject moveMouse;

    [Header("GuardDistraction")]
    [SerializeField]
    private GameObject throwRock;
    
    [SerializeField]
    private GameObject pressGtoThrow;
    private GameObject WallLifts1;
    private GameObject WallLifts2;
    private GameObject WallLifts3;
    private GameObject WallLifts4;
    private GameObject fenceToBreak;

    [Header("AdvanceToNextRoom")]
    [SerializeField]
    private GameObject advanceToNext;
    
    [SerializeField]
    private GameObject advanceToNextWithGuard;
    [SerializeField]
    private GameObject arrow;

    [Header("Fence/Ladder")]
    [SerializeField]
    private GameObject breakFence;

    private GameObject cageGuard;
    private GameObject guardToDrag;

    [Header("Player")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private GameObject pressGtoDropYourGun;

    [SerializeField]
    private GameObject pressClickToShoot;

    [SerializeField]
    private GameObject pickUpTheGunAndKillTheGuards;

    [SerializeField]
    private GameObject trainingOver;
    private int tutorialCounter;
    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;
    private int keysPressed;
    private int cubesLookedAt;

    [SerializeField]
    private GameObject pickUpGuard;

    // Start is called before the first frame update
    void Start()
    {
        //tutorial counter represents the stage the player is in completing the tutorial
        tutorialCounter = -1;
        keysPressed = 0;

        //find the walls that need to be lifted if a player complets one room of the tutorial
        WallLifts1 = GameObject.Find("Room1").transform.GetChild(2).gameObject;
        WallLifts2 = GameObject.Find("Room2").transform.GetChild(1).gameObject;
        WallLifts3 = GameObject.Find("Room3").transform.GetChild(1).gameObject;
        cageGuard = GameObject.Find("Guards").transform.GetChild(0).gameObject;

        //subscribe the fence broke event 
        fenceToBreak = GameObject.FindGameObjectWithTag("BrokenFence");
        fenceToBreak.GetComponent<Breakable>().FenceBroke += HandleBrokenFence;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tutorialCounter == -1)
        {
            //play welcome audioSource
            if (helloWelcome.activeSelf == false)
                helloWelcome.SetActive(true);

            if(helloWelcome.GetComponent<AudioSource>().isPlaying == false)
                tutorialCounter++;
        }
        // checks for when you press all of W A S D
        if (tutorialCounter == 0)
        {
            shiftPressed.SetActive(true);
            unPressedKeys.SetActive(true);
            pressKeysText.SetActive(true);
            if(wasdMovementSound.activeSelf == false) {
                wasdMovementSound.SetActive(true);
            }
            // checks each keycode and sets the bool to true + increases keysPressed counter
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

            // once 4 keys are pressed, we destory the tooltip form the UI and move onto the next tutorial part
            if (keysPressed == 4)
            {
                if(wasdMovementSound.GetComponent<AudioSource>().isPlaying == false)
                    tutorialCounter++;
                shiftPressed.SetActive(false);
                wSprite.SetActive(false);
                aSprite.SetActive(false);
                sSprite.SetActive(false);
                dSprite.SetActive(false);
                pressKeysText.SetActive(false);
                unPressedKeys.SetActive(false);
                // set active the mouse AI tooltip
                if (mouse.activeSelf == false)
                {
                    Debug.Log("oiyo");
                    mouse.SetActive(true);
                }
                
            }
        }
        // short animation to tell you to use the mouse to move the camera
        else if (tutorialCounter == 1)
        {
            // UI element has a script on itself that does a short animation and then deactivates, once it is deactivated we move onto next tutorial part
            if (mouse.activeSelf == false)
            {

                tutorialCounter++;
                // sets active the move mouse to red cubes tooltip
                moveMouse.SetActive(true);
                pointCrosshair.SetActive(true);
            }

        } 
        // checks if if the player is looking at the 2 cubes in the scene
        else if (tutorialCounter == 2)
        {
            // shoots out a raycast to see if user looks at cube
            Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);

            // checks if the thing the raycast has collided with is the cube
            if(hitInfo.collider != null)
                if (hitInfo.collider.gameObject.tag == "Cube")
                {
                    // if it is the cube, we increase how many cubes we have looked at and destroy the red cube, revealing a green cube
                    cubesLookedAt++;
                    Destroy(hitInfo.collider.gameObject);
                }

            // if looked at both cubes, destroy the UI tooltip and move onto next part
            if (cubesLookedAt == 2)
            {
                proceedToTheNextTrainingLevel.SetActive(true);
                if(proceedToTheNextTrainingLevel.GetComponent<AudioSource>().isPlaying && pointCrosshair.GetComponent<AudioSource>().isPlaying)
                {
                    pointCrosshair.GetComponent<AudioSource>().Stop();
                    tutorialCounter++;
                } else
                {
                    tutorialCounter++;
                }
                     
                Destroy(moveMouse);
            }
        }
        // this part moves the wall up and allows the user to proceed onto the next part
        else if (tutorialCounter == 3)
        {

            // if the user is still in the old room, wall continues to rise
            if (transform.position.z < 93) 
            {
                if (advanceToNext.activeSelf == false)
                    advanceToNext.SetActive(true);

                WallLifts1.transform.position = new Vector3(WallLifts1.transform.position.x , WallLifts1.transform.position.y + 0.05f, WallLifts1.transform.position.z);
            }
            // if user is in the new room, wall comes back and we move onto next tutorial
            else 
            {
                advanceToNext.SetActive(false);
                WallLifts1.transform.position = new Vector3(WallLifts1.transform.position.x, 3, WallLifts1.transform.position.z);
                tutorialCounter++;
                throwRock.SetActive(true);
                if (proceedToTheNextTrainingLevel.GetComponent<AudioSource>().isPlaying)
                {
                    proceedToTheNextTrainingLevel.GetComponent<AudioSource>().Stop();
                    
                }
                congratulationForCompletingTheFirstLevel.SetActive(true);

            }
        }
        // checks if guard gets alerted by rock
        else if (tutorialCounter == 4)
        {
            Debug.Log(camera.transform.GetChild(0).childCount);
                if(camera.transform.GetChild(1).childCount != 0) {
                     throwRock.SetActive(false);
                     pressGtoThrow.SetActive(true);
                     //Destroy(throwRock);
                }
            // once guard gets alerted by rock we can move onto the next part
            if (cageGuard.GetComponent<GuardAIPhoton>().GuardState == GuardAIPhoton.State.Alerted)
            {
                if (congratulationForCompletingTheFirstLevel.GetComponent<AudioSource>().isPlaying)
                {
                    congratulationForCompletingTheFirstLevel.GetComponent<AudioSource>().Stop();
                }
                theGuardsGetDistracted.SetActive(true);
                tutorialCounter++;
                Destroy(throwRock);
                Destroy(pressGtoThrow);
            }
       
        }
        // this part moves the wall up and allows the user to proceed onto the next part
        else if (tutorialCounter == 5)
        {
            // if the user is still in the old room, wall continues to rise
            if (transform.position.z < 142)
            {
                if (advanceToNext.activeSelf == false)
                    advanceToNext.SetActive(true);

                WallLifts2.transform.position = new Vector3(WallLifts2.transform.position.x, WallLifts2.transform.position.y + 0.05f, WallLifts2.transform.position.z);
            }
            // if user is in the new room, wall comes back and we move onto next tutorial
            else
            {
                advanceToNext.SetActive(false);
                WallLifts2.transform.position = new Vector3(WallLifts2.transform.position.x, 3, WallLifts2.transform.position.z);
                tutorialCounter++;
                if (theGuardsGetDistracted.GetComponent<AudioSource>().isPlaying)
                {
                    theGuardsGetDistracted.GetComponent<AudioSource>().Stop();

                }
                pleaseWalkUpToTheGunAndPickItUp.SetActive(true);
            }
       
        }
        // checks if all the shooting range guards have been killed
        else if (tutorialCounter == 6)
        {
            if(camera.transform.GetChild(0).childCount == 0) {
                
                if(pickUpTheGunAndKillTheGuards != null)
                    pickUpTheGunAndKillTheGuards.SetActive(true);
                    
                     
            } else {
                if (pleaseWalkUpToTheGunAndPickItUp.GetComponent<AudioSource>().isPlaying)
                {
                    pleaseWalkUpToTheGunAndPickItUp.GetComponent<AudioSource>().Stop();
                }
                Destroy(pickUpTheGunAndKillTheGuards);
                    
                if(pressClickToShoot != null)
                {
                    pressClickToShoot.SetActive(true);
                    killAll4Guards.SetActive(true);
                }
                    
                if(Input.GetMouseButton(0)) 
                {
                    Destroy(pressClickToShoot);
                }
            }
          
            // once all guards have been found to be dead
            // its 329 because of all the transforms in the guard prefab
            if (GameObject.Find("Environment/Interactables/DeadGuards").GetComponentsInChildren<Transform>().Length == 329)
            {
                // get a random number and set the guard to drag to be that random guard
                int random = Random.Range(0, 4);
                guardToDrag = GameObject.Find("Environment/Interactables/DeadGuards").transform.GetChild(random).gameObject;
                guardToDrag.GetComponent<Rigidbody>().isKinematic = true;
                // spawn an arrow above that guard and set the arrow's position to be slightly above the guard
                Vector3 arrowPos = new Vector3(guardToDrag.transform.position.x, guardToDrag.transform.position.y + 5, guardToDrag.transform.position.z);
                Instantiate(arrow, arrowPos, arrow.transform.rotation);
                tutorialCounter++;

                if (killAll4Guards.GetComponent<AudioSource>().isPlaying)
                {
                    killAll4Guards.GetComponent<AudioSource>().Stop();
                }
                findTheGuardWithTheGreenArrow.SetActive(true);
            }
        } else if(tutorialCounter == 7) {
                spaceUnpressedSprite.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Space)) 
                {
                    spaceUnpressedSprite.SetActive(false);
                    spacePressed.SetActive(true);
                    tutorialCounter++;
                    Destroy(playerMovement);
                }
        }
        // this part moves the wall up and allows the user to proceed onto the next part
        else if (tutorialCounter == 8)
        {
            // if the user and dead guard are still in the old room, wall continues to rise
            if (transform.position.x < 273 || guardToDrag.transform.position.x < 273)
            {
                if(transform.Find("Drag").childCount == 0) 
                {
           
                    if(Vector3.Distance(transform.position, guardToDrag.transform.position) < 5.0f) 
                    {
                        if(Input.GetKeyDown(KeyCode.E)) 
                        {
                            pressGtoDropYourGun.SetActive(true);
                            pickUpGuard.SetActive(false);
                        }
                    } else 
                       {
                           pickUpGuard.SetActive(true);
                           pressGtoDropYourGun.SetActive(false);
                       }
                } else 
                { 
                    pressGtoDropYourGun.SetActive(false);
                    advanceToNextWithGuard.SetActive(true); 
                    pickUpGuard.SetActive(false);
                }
                    
                WallLifts3.transform.position = new Vector3(WallLifts3.transform.position.x, WallLifts3.transform.position.y + 0.05f, WallLifts3.transform.position.z);
            }
            // if user and dead guard are in the new room, wall comes back and we move onto next tutorial
            else
            {
                advanceToNextWithGuard.SetActive(false);
                advanceToNext.SetActive(false);
                WallLifts3.transform.position = new Vector3(WallLifts3.transform.position.x, 3, WallLifts3.transform.position.z);
                breakFence.SetActive(true);
                pickUpGuard.SetActive(false);
                tutorialCounter++;

                if (findTheGuardWithTheGreenArrow.GetComponent<AudioSource>().isPlaying)
                {
                    findTheGuardWithTheGreenArrow.GetComponent<AudioSource>().Stop();
                }

                guardsBecomeAlertedIfSeesDeadGuards.SetActive(true);
            }
        }
        // this part waits for the user to break the fence
        else if (tutorialCounter == 9)
        {
            // subscribed to this in Start function
            //fenceToBreak.GetComponent<BreakFencePhoton>().FenceBroke -= HandleBrokenFence;

            if (!guardsBecomeAlertedIfSeesDeadGuards.GetComponent<AudioSource>().isPlaying)
            {
                congratulationForCompletingTheThirdLevel.SetActive(true);
            }

        } 
        else if (tutorialCounter == 10) 
        {
            if (transform.position.x > 307)
            {
                trainingOver.SetActive(true);
                if (proceedToTheLadder.GetComponent<AudioSource>().isPlaying)
                {
                    proceedToTheLadder.GetComponent<AudioSource>().Stop();
                }
                graduatedFreight.SetActive(true);
                tutorialCounter++;
            }
        }
        else if (tutorialCounter == 11)
        {
            if (!graduatedFreight.GetComponent<AudioSource>().isPlaying)
            {
                PhotonNetwork.LoadLevel(0);
                //PhotonNetwork.Disconnect();
                tutorialCounter++;
            }
        }


        //Debug.Log(tutorialCounter);
    }

    //if the fence is broken play sound 
    void HandleBrokenFence()
    {
        tutorialCounter++;
        breakFence.SetActive(false);
        if (congratulationForCompletingTheThirdLevel.GetComponent<AudioSource>().isPlaying || guardsBecomeAlertedIfSeesDeadGuards.GetComponent<AudioSource>().isPlaying)
        {
            congratulationForCompletingTheThirdLevel.GetComponent<AudioSource>().Stop();
            guardsBecomeAlertedIfSeesDeadGuards.GetComponent<AudioSource>().Stop();
        }
        proceedToTheLadder.SetActive(true);
    }

}
