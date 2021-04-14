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
    }

    void Update()
    {
        timeToLeave -= Time.deltaTime;
        if (timeToLeave < 0 && !left) StartAnimation();
    }

    private void OnDrawGizmos() {
        iTween.DrawPath(points);
    }

    private void StartAnimation() {
        iTween.MoveTo(train, iTween.Hash("path", points, "time", 50.0f, "orientToPath", true, "easetype", iTween.EaseType.easeInOutSine, "axis", "y"));
        left = true;
    }
}
