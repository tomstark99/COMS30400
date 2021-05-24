using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

//this script counts down the stopwatch. The stop watch represents the time until the train is leaving
public class Clock : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject image;
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

    private Animator animator;

    private ExitGames.Client.Photon.Hashtable CustomValue;
    int flashHash;

    // Start is called before the first frame update
    void Start()
    {
       
        GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>().EndTheGame += ClearClock;

        // if client is master client, we take the server time of the PhotonNetwork and set it to be StartTime in the custom room properties
        if (PhotonNetwork.IsMasterClient)
        {
            CustomValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }
        // if client is second client, we check if this value has been set already and get the time the master client set, to synchronise 
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

        animator = image.GetComponent<Animator>();

        flashHash = Animator.StringToHash("flash");
    }

    // Update is called once per frame
    void Update()
    {
        // we run this in update in case the StartTime was not set in Start, so we keep checking until we can set it to something
        if (!startTimer) 
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["StartTime"] != null)
            {
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }
            else
            {
                return;
            }
        } 
        // if train has not yet left, we keep decrementing the counter
        else if (!trainLeft)
        {
            // to get synchronised time between clients we take the time to leave from game settings and add it to the initial PhotonNetwork time set by master client subtracted from the 
            // current PhotonNetwork time. 
            // this method ensures both clients have the exact same time displayed 
            double newTime = timeToLeave + (startTime - PhotonNetwork.Time);
            timerIncrementer = Math.Round(newTime, 4);
            text.text = "Time to leave: " + FormatTime(timerIncrementer);
            if(Math.Round(newTime) <= 60) 
            {
                bool flashing = animator.GetBool(flashHash);
                // on 30, 10, 5 seconds we have a short flashing animation
                if ((Mathf.Ceil((float) newTime) % 30 == 0 && !flashing) || 
                    (Mathf.Ceil((float) newTime) == 10 && !flashing) || 
                    (Mathf.Ceil((float) newTime) == 5 && !flashing)) 
                {
                    animator.SetBool(flashHash, true);
                } 
                // if previously was flashing, we set to not flashing
                else if (flashing) 
                {
                    animator.SetBool(flashHash,false);
                }
                text.fontSize = Mathf.Lerp(16,20,t);
                if (t <= 1) t += Time.deltaTime/60.0f;
                // if time reaches 0, we set train is leaving message
                if (newTime <= 0)
                {
                    trainLeft = true;
                    text.fontSize = 20;
                    text.text = "Train is leaving!";
                }
            }
        } 
    }

    private string FormatTime( double time ) 
    {
        int min = (int) time / 60;
        int sec = (int) time - 60 * min;
        int mil = (int) (1000 * (time - min * 60 - sec));
        return time < 60 ? string.Format("{0:00}:{1:00}:{2:00}", min, sec, Mathf.Floor(mil / 10)) : string.Format("{0:00}:{1:00}", min, sec);
    }

    void ClearClock()
    {
        image.SetActive(false);
        text.enabled = false;
    }
}
