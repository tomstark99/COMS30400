using UnityEngine;
using Photon.Pun;

public class SplineWalker : MonoBehaviourPun
{

	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;

	private float progress;
	private bool goingForward = true;

    public Vector3 trainPos;
    public Vector3 prevPos;

    public bool leaving;

    private void Start () {
        if (!PhotonNetwork.IsMasterClient)
            return;

        leaving = true;

        if (goingForward) {
                progress += (Time.deltaTime / duration);
                if (progress > 1f) {
                    if (mode == SplineWalkerMode.Once) {
                        progress = 1f;
                    }
                    else if (mode == SplineWalkerMode.Loop) {
                        progress -= 1f;
                    }
                    else {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else {
                progress -= Time.deltaTime / duration;
                if (progress < 0f) {
                    progress = - progress;
                    goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            prevPos = position;
            trainPos = position;
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
                transform.Rotate(0, -90, 0);
            }

            Debug.Log(spline);
    }

	private void Update () 
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if(leaving) {
            if (goingForward) {
                progress += (Time.deltaTime / duration);
                if (progress > 1f) {
                    if (mode == SplineWalkerMode.Once) {
                        progress = 1f;
                    }
                    else if (mode == SplineWalkerMode.Loop) {
                        progress -= 1f;
                    }
                    else {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else {
                progress -= Time.deltaTime / duration;
                if (progress < 0f) {
                    progress = - progress;
                    goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            prevPos = trainPos;
            trainPos = position;
            transform.localPosition = position;
            if (lookForward) 
            {
                transform.LookAt(position + spline.GetDirection(progress));
                transform.Rotate(0, -90, 0);
            }
        }
	}
}
