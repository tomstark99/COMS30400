using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

// This script can be applied to sliders to get the value of the slider to show up as text on the UI
public class SliderToValue : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Slider sliderDiff;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private TextMeshProUGUI textDiff;

    [SerializeField]
    private bool difficulty;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!difficulty)
                UpdateSliderValue();
            else
                UpdateSliderValueDifficulty();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            if (!difficulty)
                InitialiseSliderValue();
            else
                InitialiseSliderValueDifficulty();

            slider.interactable = false;
            sliderDiff.interactable = false;
        }
    }

    void InitialiseSliderValue()
    {
        text.text = PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"].ToString();
    }

    void InitialiseSliderValueDifficulty()
    {
        textDiff.text = PhotonNetwork.CurrentRoom.CustomProperties["sliderValueDiff"].ToString();
    }

    public void UpdateSliderValue()
    {
        text.text = slider.value.ToString();
        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        prop.Add("sliderValue", slider.value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
    }

    // special case function for easy/med/hard/impossible 
    public void UpdateSliderValueDifficulty()
    {
        int sliderValue = (int) sliderDiff.value;

        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();

        if (sliderValue == 0)
            //text.text = "Easy";
            prop.Add("sliderValueDiff", "Easy");
        else if (sliderValue == 1)
            //text.text = "Medium";
            prop.Add("sliderValueDiff", "Medium");
        else if (sliderValue == 2)
            //text.text = "Hard";
            prop.Add("sliderValueDiff", "Hard");
        else if (sliderValue == 3)
            //text.text = "Impossible";
            prop.Add("sliderValueDiff", "Impossible");

        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("sliderValue"))
        {
            text.text = propertiesThatChanged["sliderValue"].ToString();
        }
        if (propertiesThatChanged.ContainsKey("sliderValueDiff"))
        {
            textDiff.text = propertiesThatChanged["sliderValueDiff"].ToString();
        }
    }
}
