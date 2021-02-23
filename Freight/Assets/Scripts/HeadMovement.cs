using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{

    private Vector3 origionalNosePos = new Vector3(320,240,0);
    private float[] poseArr;
    private Vector3 currentNosePos;
    private Vector3 noseOffset;

    private Vector3 origionalTranfromPos;
    
    // Start is called before the first frame update
    void Start()
    {
        origionalTranfromPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        poseArr = PoseParser.GETPoseArray();
        currentNosePos = new Vector3(poseArr[0], poseArr[1], 0);

        noseOffset = currentNosePos - origionalNosePos;
        noseOffset.Scale(new Vector3(1.0f/240.0f, -1.0f/240.0f, 1.0f/240.0f));

        //Debug.Log(noseOffset.ToString());

        transform.localPosition = origionalTranfromPos + noseOffset;

    }
}
