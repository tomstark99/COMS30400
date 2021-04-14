using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DefaultButton : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Slider sliderDiff;

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

        slider.value = 200;
        sliderDiff.value = 2;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
