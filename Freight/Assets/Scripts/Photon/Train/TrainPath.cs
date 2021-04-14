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

    // Start is called before the first frame update
    void Start()
    {
        iTween.MoveTo(train, iTween.Hash("path", points, "speed", 5.0f, "orientToPath", true, "easetype", iTween.EaseType.easeInOutSine, "axis", "y", "lookahead", 0.001f));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (position < 1) {
            position += Time.deltaTime / 10;
        }
        // iTween.PutOnPath(train, points, position);

    }

    private void OnDrawGizmos() {
        iTween.DrawPath(points);
    }
}
