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

    private int bagsPickedUp;

    // Start is called before the first frame update
    void Start()
    {
        bagsPickedUp = 0;
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence += SetBreakFenceToActive;
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().FenceBroke += SetFindBackpacksToActive;
        GameObject[] bags = GameObject.FindGameObjectsWithTag("Bag");
        foreach (var bag in bags)
        {
            bag.GetComponent<Grabbable>().BagPickedUp += SetFindTrainToActive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetBreakFenceToActive()
    {
        findBrokenFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        breakFence.SetActive(true);
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence -= SetBreakFenceToActive;
    }

    void SetFindBackpacksToActive()
    {
        breakFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        findBackpacks.SetActive(true);
    }

    void SetFindTrainToActive() 
    {
        bagsPickedUp += 1;

        findBackpacks.GetComponent<TextMeshProUGUI>().text = "- Search the buildings for the backpacks (" + bagsPickedUp + "/2)";

        if (bagsPickedUp == 2)
        {
            findBackpacks.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findTrain.SetActive(true);
        }
    }
}
