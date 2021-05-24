using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class Overlay : MonoBehaviour
{
    
    [DllImport("__Internal")]
    private static extern void LoadOverlayPlugin(String relativePath);
    [DllImport("__Internal")]
    private static extern void ClearOverlayPlugin();

    public static void LoadOverlay(String path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            LoadOverlayPlugin(path);
#endif
    }

    public static void ClearOverlay()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ClearOverlayPlugin();
#endif
    }
    
    // // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
