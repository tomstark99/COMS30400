using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrainPath : MonoBehaviourPun
{
    public GameObject train;
    public Transform[] points;

    [Range(0,1)]
    public float position;
    private float timeToLeave;
    private bool left = false;

    // Start is called before the first frame update
    void Start()
    {
        timeToLeave = GameObject.FindGameObjectWithTag("time").GetComponent<SyncedTime>().TimeToLeave;
        StartAnimation();
    }

    void Update()
    {
        // timeToLeave -= Time.deltaTime;
        // if (timeToLeave < 0 && !left) StartAnimation();
    }

    private void OnDrawGizmos() {
        iTween.DrawPath(points);
    }

    private void StartAnimation() {
        /*  switch between for different movement
            - iTween.EaseType.easeInOutSine
            - iTween.EaseType.linear
        */
        Debug.Log("mans animating");
        iTween.MoveTo(train, iTween.Hash("path", points, "speed", 7.0f, "orientToPath", true, "easetype", iTween.EaseType.linear, "lookahead", 0.005f));
        left = true;
    }
}
