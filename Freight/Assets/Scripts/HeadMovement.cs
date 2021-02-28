using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HeadMovement : NetworkBehaviour
{
    public Transform cameraTransform;

    private Vector3 origionalNosePos = new Vector3(320, 240, 0);
    private float[] poseArr;
    private Vector3 currentNosePos;
    private Vector3 noseOffset;

    private Vector3 origionalTranfromPos;


    [ClientCallback]
    void Start()
    {
        if (!hasAuthority)
        {
            return;
        }
        origionalTranfromPos = cameraTransform.localPosition;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        poseArr = PoseParser.GETPoseArray();
        currentNosePos = new Vector3(poseArr[0], poseArr[1], 0);

        noseOffset = currentNosePos - origionalNosePos;
        noseOffset.Scale(new Vector3(1.0f / 240.0f, 1.0f / 240.0f, 1.0f / 240.0f));

        //Debug.Log(noseOffset.ToString());

        cameraTransform.localPosition = origionalTranfromPos + noseOffset;
    }
}

