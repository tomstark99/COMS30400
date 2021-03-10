using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerUI;
    private GameObject uiRef;
    public string gesture;


    // Start is called before the first frame update
    void Start()
    {
        //uiRef = Instantiate(playerUI);
        //uiRef.transform.parent = gameObject.transform;
        gesture = PoseParser.GETGestureAsString();
        if (photonView.IsMine)
        {
            transform.Find("UI 1").gameObject.SetActive(true);
        }
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            Debug.Log(" DISABLE CONTROLER ");
            transform.Find("UI 1").gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        string tempGesture = PoseParser.GETGestureAsString();
        if (tempGesture != gesture)
        {
            gesture = tempGesture;
        }
        //Debug.Log(gesture);
    }

    public bool IsPressingP()
    {
         return Input.GetKeyDown(KeyCode.P);
    }

    public bool IsPressingE()
    {
         return Input.GetKeyDown(KeyCode.E);
    }
}
