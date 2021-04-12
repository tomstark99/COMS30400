using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script can be applied to sliders to get the value of the slider to show up as text on the UI
public class SliderToValue : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private bool difficulty;

    // Start is called before the first frame update
    void Start()
    {
        if (!difficulty)
            UpdateSliderValue();
        else
            UpdateSliderValueDifficulty();
    }

    public void UpdateSliderValue()
    {
        text.text = slider.value.ToString();
    }

    // special case function for easy/med/hard/impossible 
    public void UpdateSliderValueDifficulty()
    {
        int sliderValue = (int) slider.value;

        if (sliderValue == 0)
            text.text = "Easy";
        else if (sliderValue == 1)
            text.text = "Medium";
        else if (sliderValue == 2)
            text.text = "Hard";
        else if (sliderValue == 3)
            text.text = "Impossible";
    }
}
