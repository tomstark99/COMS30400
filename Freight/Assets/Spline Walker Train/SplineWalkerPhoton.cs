using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SplineWalkerPhoton : MonoBehaviourPun
{

    public BezierSpline spline;

    public float duration;

    public bool lookForward;

    public SplineWalkerMode mode;

    [SerializeField]
    private float progress = 0.25f;
    private bool goingForward = true;

    // if you want to change the time to leave, go to the SyncedTime script and change it there
    private float timeToLeave;

    public Vector3 trainPos;
    public Vector3 prevPos;

    public GameObject[] carriages;
    private Dictionary<GameObject, bool> carriageOnTrack = new Dictionary<GameObject, bool>();

    private bool animationComplete = false;
    private bool callEvent = false;
    public event Action TrainLeave;

    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        timeToLeave = GameObject.FindGameObjectWithTag("time").GetComponent<SyncedTime>().TimeToLeave;

        if (goingForward)
        {
            progress += (Time.deltaTime / duration);
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        prevPos = position;
        trainPos = position;
        transform.localPosition = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }

        foreach (var c in carriages) {
            carriageOnTrack[c] = true;
        }

        Debug.Log(spline);
    }

    // void timer_Tick(object sender, EventArgs e) {
    //     leaveStation = true;
    // }

    private void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

       // Debug.Log("time to leave:" + timeToLeave);
        timeToLeave -= Time.deltaTime;
        if (timeToLeave < 0)
        {
            if (!callEvent) {
                if (TrainLeave != null)
                {
                    TrainLeave();
                    callEvent = true;
                }
            }
            if (goingForward)
            {
                progress += (Time.deltaTime / duration);
                if (progress > 1f)
                {
                    if (mode == SplineWalkerMode.Once)
                    {
                        progress = 1f;
                        animationComplete = true;
                    }
                    else if (mode == SplineWalkerMode.Loop)
                    {
                        progress -= 1f;
                    }
                    else
                    {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }
            if (!animationComplete) {
                Vector3 position = spline.GetPoint(progress);
                prevPos = trainPos;
                trainPos = position;
                transform.localPosition = position;
                if (lookForward)
                {
                    transform.LookAt(position + spline.GetDirection(progress));
                }

                
            }
        }
        if (!animationComplete) {
            // carriages 
            int i = 1;
            foreach (var c in carriages) {
                // Debug.Log(((c.transform.position - spline.GetPoint(0.0f)).sqrMagnitude));
                if (((c.transform.position - spline.GetPoint(0.0f)).sqrMagnitude < 0.01f && !carriageOnTrack[c])) { // || (progress >= 0.05f && progress <= 0.06f)) {
                    Debug.Log("ONTRACK");
                    carriageOnTrack[c] = true;
                } else if (carriageOnTrack[c]) {
                    float min = float.PositiveInfinity;
                    float min_p = 0.0f;
                    // Debug.Log((progress - (0.1f * i)) + " : " + progress);
                    for (float t = ((progress - (0.1f * i)) <= 0) ? 0 : (progress - (0.1f * i)) ; t <= progress; t += 0.0005f) {
                        float dist = (c.transform.position - spline.GetPoint(t)).sqrMagnitude;
                        if (dist < min) {
                            min = dist;
                            min_p = t;
                        }
                    }
                    Vector3 carriagePosition = spline.GetPoint(min_p);
                    c.transform.localPosition = carriagePosition;
                    if (lookForward) {
                        c.transform.LookAt(carriagePosition + spline.GetDirection(min_p));
                    }
                }
                i++;
            }
        }
    }
}
