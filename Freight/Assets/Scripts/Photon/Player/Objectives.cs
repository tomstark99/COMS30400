using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objectives : MonoBehaviour
{
    [SerializeField]
    private GameObject findBrokenFence;
    [SerializeField]
    private GameObject breakFence;
    [SerializeField]
    private GameObject findBackpacks;
    [SerializeField]
    private GameObject findTrain;
    [SerializeField]
    private GameObject escapeOnTrain;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence += setBreakFenceToActive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setBreakFenceToActive()
    {
        findBrokenFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        breakFence.SetActive(true);
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence -= setBreakFenceToActive;
    }
}
