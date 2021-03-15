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

    [Header("Player")]
    [SerializeField]
    private GameObject player;

    private int tutorialCounter;
    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;
    private int keysPressed;

    // Start is called before the first frame update
    void Start()
    {
        tutorialCounter = 0;
        keysPressed = 0;
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
            }
        }
    }

}
