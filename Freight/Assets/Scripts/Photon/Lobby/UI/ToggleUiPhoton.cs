using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ToggleUiPhoton : MonoBehaviour
{
    public TextMeshProUGUI ping;
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

        int pingVal = PhotonNetwork.GetPing();

        if (pingVal > 120 && !pingWarning.activeSelf)
            pingWarning.SetActive(true);
        else if (pingVal <= 120 && pingWarning.activeSelf)
            pingWarning.SetActive(false);

        ping.text = "Ping: " + pingVal.ToString();
    }
}
