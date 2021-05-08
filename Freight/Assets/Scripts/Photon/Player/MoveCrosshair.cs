using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveCrosshair : MonoBehaviourPun
{

    public GameObject crosshair;
    private Vector2 origionalPosition;
	private RectTransform crosshairTransform;

	// Start is called before the first frame update
    void Start()
    {
		crosshairTransform = crosshair.GetComponent<RectTransform>();
		origionalPosition = crosshairTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
		if (!photonView.IsMine)
			return;

	    Vector3 handPos = PoseParser.GETLeftHandPositionAsVector();
	    // if confidence is over 90%
	    if (handPos.z > 0.15)
	    {
		    // normalise
		    // TODO don't hard code values
		    handPos.Scale(new Vector3(1.0f/320.0f,1.0f/240.0f, 0));
		    // rescale onto screen (this is far from perfect due to odd UI scaling)
		    crosshairTransform.anchoredPosition = new Vector2((-2.0f * handPos.x * 240) + 580, (-2.0f * handPos.y * 240) + 300);
	    }
	    else
	    {
		    crosshairTransform.anchoredPosition = origionalPosition;
	    }
	    
	    // Debug.Log("CROSSHAIR = " + crosshairTransform.anchoredPosition.ToString());
    }

    public Vector3 GETCrosshairOffsetFromCentre()
    {
		if (!photonView.IsMine)
			return new Vector3(0,0,0);
		
		return new Vector3(crosshairTransform.anchoredPosition.x, crosshairTransform.anchoredPosition.y, 0);
    }
    
}
