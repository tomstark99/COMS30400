using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TutorialManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject player;

    private int tutorialCounter;

    // Start is called before the first frame update
    void Start()
    {
        tutorialCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialCounter == 0)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                tutorialCounter++;

            }
        }
    }

}
