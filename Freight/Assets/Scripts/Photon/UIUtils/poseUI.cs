using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class poseUI : MonoBehaviour
{

    public GameObject text;
    public GameObject TurnDownPose;
    public void OnMouseOver() {
        //Debug.Log("MOUSE OVER POSE");
        transform.parent.parent.parent.GetComponent<AudioSource>().Play();
        TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
        transform.GetComponent<Image>().enabled = true;
        TextMeshPros.color  = new Color32(0, 0, 0, 255);
        TurnDownPose.SetActive(true);
    }

    public void OnMouseExit() {
         TextMeshProUGUI TextMeshPros = text.GetComponent<TextMeshProUGUI>();
         TextMeshPros.color = new Color32(151, 158, 169, 255);
         transform.GetComponent<Image>().enabled = false;
         TurnDownPose.SetActive(false);
    }
    public void onClick()
    {
        //Debug.Log("turn off pose");
        PoseParser.turnOffPose();
    }
}