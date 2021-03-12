using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ToggleUiPhoton : MonoBehaviour
{
    public GameObject timetable;
    public GameObject map;
    public TextMeshProUGUI ping;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (map.activeSelf)
            {
                map.SetActive(false);
            }
            timetable.SetActive(!timetable.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (timetable.activeSelf)
            {
                timetable.SetActive(false);
            }
            map.SetActive(!map.activeSelf);
        }

        ping.text = "Ping: " + PhotonNetwork.GetPing().ToString();
    }
}
