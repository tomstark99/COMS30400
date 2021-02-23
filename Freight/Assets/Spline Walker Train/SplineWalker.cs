using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;

	private float progress;
	private bool goingForward = true;
    // private bool leaveStation = false;
    private float timeToLeave = 5.0f;

    private void Start () {
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
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
            }
    }

    // void timer_Tick(object sender, EventArgs e) {
    //     leaveStation = true;
    // }

	private void Update () {
        timeToLeave -= Time.deltaTime;
        if(timeToLeave < 0) {
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
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
	}
}
