using System.Collections;
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
            text.text = "Time to leave: " + timerIncrementer.ToString();
            if (newTime <= 0)
            {
                trainLeft = true;
                text.text = "Train is leaving!";
            }
        } 
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
