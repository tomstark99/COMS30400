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
    public GameObject instructions;
    public GameObject pingWarning;
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
            if (map.activeSelf || instructions.activeSelf)
            {
                map.SetActive(false);
                instructions.SetActive(false);
            }
            timetable.SetActive(!timetable.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (timetable.activeSelf || instructions.activeSelf)
            {
                timetable.SetActive(false);
                instructions.SetActive(false);
            }
            map.SetActive(!map.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (map.activeSelf || timetable.activeSelf)
            {
                map.SetActive(false);
                timetable.SetActive(false);
            }
            instructions.SetActive(!instructions.activeSelf);
        }

        int pingVal = PhotonNetwork.GetPing();

        if (pingVal > 120 && !pingWarning.activeSelf)
            pingWarning.SetActive(true);
        else if (pingVal <= 120 && pingWarning.activeSelf)
            pingWarning.SetActive(false);

        ping.text = "Ping: " + pingVal.ToString();
    }
}
