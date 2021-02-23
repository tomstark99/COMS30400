using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Clock : MonoBehaviour
{
    public TextMeshPro text;
    string hour;
    string minute;
    string second;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateClock();
    }

    void updateClock()
    {
        DateTime currentTime = DateTime.Now;
        hour = currentTime.Hour.ToString();
        minute = currentTime.Minute.ToString();
        second = currentTime.Second.ToString();
        text.text = hour + ":" + minute + ":" + second;
    }
}
