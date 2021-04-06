using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objectives : MonoBehaviour
{
    [SerializeField]
    private GameObject findBrokenFence;
    [SerializeField]
    private GameObject findBrokenFenceDesc;
    [SerializeField]
    private GameObject breakFence;
    [SerializeField]
    private GameObject breakFenceDesc;
    [SerializeField]
    private GameObject findBackpacks;
    [SerializeField]
    private GameObject findBackpacksDesc;
    [SerializeField]
    private GameObject findTrain;
    [SerializeField]
    private GameObject findTrainDesc;
    [SerializeField]
    private GameObject escapeOnTrain;
    [SerializeField]
    private GameObject escapeOnTrainDesc;

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
        findBrokenFenceDesc.SetActive(false);
        breakFence.SetActive(true);
        breakFenceDesc.SetActive(true);
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence -= SetBreakFenceToActive;
    }

    void SetFindBackpacksToActive()
    {
        breakFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        breakFenceDesc.SetActive(false);
        findBackpacks.SetActive(true);
        findBackpacksDesc.SetActive(true);
    }

    void SetFindTrainToActive() 
    {
        bagsPickedUp += 1;

        findBackpacks.GetComponent<TextMeshProUGUI>().text = "- Find the backpacks (" + bagsPickedUp + "/2)";

        if (bagsPickedUp == 2)
        {
            findBackpacks.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findBackpacksDesc.SetActive(false);
            findTrain.SetActive(true);
            findTrainDesc.SetActive(true);
        }
    }
}
