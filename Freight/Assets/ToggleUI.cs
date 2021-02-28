using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ToggleUI : NetworkBehaviour
{
    public GameObject timetable;
    public GameObject map;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ClientCallback]
    // Update is called once per frame
    void Update()
    {
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
    }
}
