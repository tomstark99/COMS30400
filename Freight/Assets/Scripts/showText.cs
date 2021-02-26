using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Mirror;


public class ShowText : NetworkBehaviour
{

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    public Text txtMy;
    
    [ClientCallback]
    void Start()
    {
        if (!hasAuthority)
        {
            return;
        }
        txtMy.text = string.Join(",", PoseParser.GETPoseArray());
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        txtMy.text = PoseParser.GETGestureAsString() + string.Join(",", PoseParser.GETPoseArray());
    }
}
