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
        // sets the default button to not be interactable if player is not master client
        if (!PhotonNetwork.IsMasterClient)
        {
            gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    // this is called on button press, sets the properties of the room to go back to default (the values on room creation)
    public void SetSettingsToDefault()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();

        props.Add("sliderValue", 200f);
        props.Add("sliderValueDiff", "Hard");

        slider.value = 200;
        sliderDiff.value = 2;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
