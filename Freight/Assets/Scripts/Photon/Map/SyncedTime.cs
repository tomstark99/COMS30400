﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedTime : MonoBehaviour
{
    private float timeToLeave;

    public float TimeToLeave
    {
        get { return timeToLeave;  }
    }

    // Start is called before the first frame update
    void Awake()
    {
        timeToLeave = 1000000f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
