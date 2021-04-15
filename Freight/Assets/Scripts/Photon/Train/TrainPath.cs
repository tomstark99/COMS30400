using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrainPath : MonoBehaviourPun
{
    public GameObject train;
    public Transform[] points;
    public GameObject[] carriages;

    [Range(0,1)]
    public float position;
    private float timeToLeave;
    private bool left = false;
    private Dictionary<GameObject, bool> carriageOnTrack = new Dictionary<GameObject, bool>();

    // Start is called before the first frame update
    void Start()
    {
        timeToLeave = GameObject.FindGameObjectWithTag("time").GetComponent<SyncedTime>().TimeToLeave;
        // StartAnimation();
        foreach (var c in carriages) {
            carriageOnTrack[c] = false;
        }
    }

    void Update()
    {
        if(PhotonNetwork.IsMasterClient) {
            timeToLeave -= Time.deltaTime;
            if (timeToLeave < 0 && !left) StartAnimation();
            if (left) {
                foreach (var c in carriages) {
                    float min = float.PositiveInfinity;
                    float min_p = 0.0f;
                    for (float t = 0; t <= 1; t += 0.0005f) {
                        float dist = (c.transform.position - iTween.PointOnPath(points, t)).sqrMagnitude;
                        if (dist < min) {
                            min = dist;
                            min_p = t;
                        }
                    }
                    if (min < 0.001f && !carriageOnTrack[c]) {
                        Debug.Log("ONTRACK");
                        carriageOnTrack[c] = true;
                    } else if (carriageOnTrack[c]) {
                        iTween.PutOnPath(c, points, min_p);
                    }
                }
            }
        }
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
