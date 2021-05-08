using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poseUI : MonoBehaviour
{
    public void onClick()
    {
        Debug.Log("turn off pose");
        PoseParser.turnOffPose();
    }
}