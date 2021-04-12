using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DefaultButton : MonoBehaviour
{
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    public void SetSettingsToDefault()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();

        props.Add("sliderValue", 200);
        props.Add("sliderValueDiff", "Hard");

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
