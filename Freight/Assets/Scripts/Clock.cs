using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Clock : MonoBehaviour
{
    public TextMeshPro text;
    public float duration;
    string hour;
    string minute;
    string second;
    DateTime startTime;
    DateTime currentTime;
    DateTime gameTime;


    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;
        InvokeRepeating("updateClock", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateClock()
    {
        currentTime = DateTime.Now;
        TimeSpan difference = currentTime - startTime;
        gameTime = Convert.ToDateTime("01/01/2020 12:00:00");
        gameTime = gameTime.Add(difference);
        hour = leadingZero(gameTime.Hour);
        minute = leadingZero(gameTime.Minute);
        second = leadingZero(gameTime.Second);
        text.text = hour + ":" + minute + ":" + second;
    }

    string leadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}
