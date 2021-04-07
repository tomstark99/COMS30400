﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI text;
    //public GameObject text;
    public float duration;
    string hour;
    string minute;
    string second;
    double startTime;
    bool startTimer;
    bool trainLeft;
    double timerIncrementer;
    DateTime currentTime;

    float timeToLeave;
    private float t = 0.0f;

    private ExitGames.Client.Photon.Hashtable CustomValue;


    // Start is called before the first frame update
    void Start()
    {
        //startTime = DateTime.Now;
        //InvokeRepeating("updateClock", 1f, 1f);

        if (PhotonNetwork.IsMasterClient)
        {
            CustomValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["StartTime"] != null)
            {
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }

        }
        timeToLeave = GameObject.FindGameObjectWithTag("time").GetComponent<SyncedTime>().TimeToLeave;
        trainLeft = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!startTimer) {
            if (PhotonNetwork.CurrentRoom.CustomProperties["StartTime"] != null)
            {
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }
            else
            {
                return;
            }
        } else if (!trainLeft)
        {
            double newTime = timeToLeave + (startTime - PhotonNetwork.Time);
            timerIncrementer = Math.Round(newTime, 4);
            text.text = "Time to leave: " + FormatTime(timerIncrementer);//timerIncrementer.ToString();
            if(newTime <= 60) {

                text.fontSize = Mathf.Lerp(16,20,t);
                if (t <= 1) t += Time.deltaTime/60.0f;
                if (newTime <= 0)
                {
                    trainLeft = true;
                    text.fontSize = 20;
                    text.text = "Train is leaving!";
                }
            }
        } 
    }

    private string FormatTime( double time ) {
        int min = (int) time / 60;
        int sec = (int) time - 60 * min;
        int mil = (int) (1000 * (time - min * 60 - sec));
        return time < 60 ? string.Format("{0:00}:{1:00}:{2:00}", min, sec, mil) : string.Format("{0:00}:{1:00}", min, sec);
    }

    //void updateClock()
    //{
    //    currentTime = DateTime.Now;
    //    TimeSpan difference = currentTime - startTime;
    //    gameTime = Convert.ToDateTime("01/01/2020 12:00:00");
    //    gameTime = gameTime.Add(difference);
    //    hour = leadingZero(gameTime.Hour);
    //    minute = leadingZero(gameTime.Minute);
    //    second = leadingZero(gameTime.Second);
    //    text.text = hour + ":" + minute + ":" + second;
    //}

    string leadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}
