using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objectives : MonoBehaviour
{
    [SerializeField]
    public GameObject objectivesTitle;
    [SerializeField]
    public GameObject objectivesBackground;
    [SerializeField]
    private GameObject findBrokenFence;
    [SerializeField]
    private GameObject findBrokenFenceDesc;
    [SerializeField]
    private GameObject findBrokenFenceBackground;
    [SerializeField]
    private GameObject breakFence;
    [SerializeField]
    private GameObject breakFenceDesc;
    [SerializeField]
    private GameObject breakFenceBackground;
    [SerializeField]
    private GameObject findBackpacks;
    [SerializeField]
    private GameObject findBackpacksDesc;
    [SerializeField]
    private GameObject findBackpacksBackground;
    [SerializeField]
    private GameObject findTrain;
    [SerializeField]
    private GameObject findTrainDesc;
    [SerializeField]
    private GameObject findTrainBackground;
    [SerializeField]
    private GameObject findTrainDistance;
    [SerializeField]
    private GameObject escapeOnTrain;
    [SerializeField]
    private GameObject escapeOnTrainDesc;
    [SerializeField]
    private GameObject escapeOnTrainBackground;
    [SerializeField]
    private GameObject escapeOnTrainCompleteBackground;
    [SerializeField]
    private TextMeshProUGUI ping;

    private int bagsPickedUp;

    public GameObject FindBackpacks{
        get { return findBackpacks; }
    }
    public GameObject FindBackpacksDesc{
        get { return findBackpacksDesc; }
    }

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
        GameObject.FindGameObjectWithTag("locomotive").GetComponent<InRange>().InRangeOfTrain += SetEscapeToActive;
        GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>().StartEndGame += SetObjectivesComplete;
        GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>().EndTheGame += ClearObjectives;
    }

    // Update is called once per frame
    void Update()
    {
        if(findTrainDistance.activeSelf) {
            float distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("locomotive").transform.position);
            findTrainDistance.GetComponent<TextMeshProUGUI>().text = "Nearest train: " + Math.Round(distance, 2) + "m";
        }
    }

    void SetBreakFenceToActive()
    {
        findBrokenFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        findBrokenFenceDesc.SetActive(false);
        findBrokenFenceBackground.SetActive(false);
        breakFence.SetActive(true);
        breakFenceDesc.SetActive(true);
        breakFenceBackground.SetActive(true);
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence -= SetBreakFenceToActive;
    }

    void SetFindBackpacksToActive()
    {
        breakFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        breakFenceDesc.SetActive(false);
        breakFenceBackground.SetActive(false);
        findBackpacks.SetActive(true);
        findBackpacksDesc.SetActive(true);
        findBackpacksBackground.SetActive(true);
    }

    void SetFindTrainToActive() 
    {
        bagsPickedUp += 1;

        Debug.Log("picked up bag");

        findBackpacks.GetComponent<TextMeshProUGUI>().text = "- Find the backpacks (" + bagsPickedUp + "/2)";

        if (bagsPickedUp == 2)
        {
            findBackpacks.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findBackpacksDesc.SetActive(false);
            findBackpacksBackground.SetActive(false);
            findTrain.SetActive(true);
            findTrainDesc.SetActive(true);
            findTrainBackground.SetActive(true);
            findTrainDistance.SetActive(true);
        }
    }

    void SetEscapeToActive()
    {
        Debug.Log("IS BACKPACKS ACTIVE? " + findBackpacksDesc.activeSelf);
        if(!findBackpacksDesc.activeSelf && findBackpacks.activeSelf) {
            findTrain.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findTrainDesc.SetActive(false);
            findTrainDistance.SetActive(false);
            findTrainBackground.SetActive(false);
            escapeOnTrain.SetActive(true);
            escapeOnTrainDesc.SetActive(true);
            escapeOnTrainBackground.SetActive(true);
            //GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<BreakFencePhoton>().InRangeOfFence -= SetBreakFenceToActive;
        }
    }

    void SetObjectivesComplete()
    {
        escapeOnTrain.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        escapeOnTrainDesc.SetActive(false);
        escapeOnTrainBackground.SetActive(false);
        escapeOnTrainCompleteBackground.SetActive(true);
    }

    void ClearObjectives()
    {
        objectivesTitle.SetActive(false);
        objectivesBackground.SetActive(false);
        findBrokenFence.SetActive(false);
        breakFence.SetActive(false);
        findBackpacks.SetActive(false);
        findTrain.SetActive(false);
        escapeOnTrain.SetActive(false);
        escapeOnTrainBackground.SetActive(false);
    }

    void ClearPing()
    {
        ping.enabled = false;
    }
}
