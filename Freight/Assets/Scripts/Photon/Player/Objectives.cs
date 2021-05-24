using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


//Needs refactoring. Really messy. For some reason updating the position with a new vector3 did not work so i added the transform
//of an empty game object with transform.position.y = 20;
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
    private GameObject findBackpacksDescLaptop;
    [SerializeField]
    private GameObject findBackpacksLaptopDist;
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

    [SerializeField]
    private GameObject parentTing;
    private int bagsPickedUp;
    private int playerCount;

    public GameObject FindBackpacks{
        get { return findBackpacks; }
    }
    public GameObject FindTrain{
        get { return findTrain; }
    }
    
    public GameObject FindBackpacksDesc{
        get { return findBackpacksDesc; }
    }
    public GameObject FindTrainDesc{
        get { return findTrainDesc; }
    }

    // Start is called before the first frame update
    void Start()
    {
        bagsPickedUp = 0;
        GameObject.FindGameObjectWithTag("BrokenFence").transform.GetChild(4).GetComponent<BreakFencePhoton>().InRangeOfFence += SetBreakFenceToActive;
        GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<Breakable>().FenceBroke += SetFindBackpacksToActive;

        Invoke(nameof(SubscribeToBagEvents), 7f);

        GameObject[] trains = GameObject.FindGameObjectsWithTag("locomotive");
        foreach (var train in trains)
        {
            train.GetComponent<InRange>().InRangeOfTrain += SetEscapeToActive;
        }
        //GameObject.FindGameObjectWithTag("locomotive").GetComponent<InRange>().InRangeOfTrain += SetEscapeToActive;
        GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>().StartEndGame += SetObjectivesComplete;
        GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>().EndTheGame += ClearObjectives;

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    void SubscribeToBagEvents()
    {
        GameObject[] bags = GameObject.FindGameObjectsWithTag("Bag");
        foreach (var bag in bags)
        {
            bag.GetComponent<Grabbable>().BagPickedUp += SetFindTrainToActive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(findTrainDistance.activeSelf) 
        {
            float distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("locomotive").transform.position);
            findTrainDistance.GetComponent<TextMeshProUGUI>().text = "Nearest train: " + Math.Round(distance, 2) + "m";
        }
        if(findBackpacksLaptopDist.activeSelf)
        {
            float distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Laptop").transform.position);
            findBackpacksLaptopDist.GetComponent<TextMeshProUGUI>().text = "(" + Math.Round(distance, 2) + "m)";
            if (distance < 5.0f)
            {
                GetComponent<PlayerAudioClips>().LaptopFound();
                findBackpacksLaptopDist.SetActive(false);
                findBackpacksDescLaptop.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            }
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
        GameObject.FindGameObjectWithTag("BrokenFence").transform.GetChild(4).GetComponent<BreakFencePhoton>().InRangeOfFence -= SetBreakFenceToActive;
    }

    void SetFindBackpacksToActive()
    {
        breakFence.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        breakFenceDesc.SetActive(false);
        breakFenceBackground.SetActive(false);
        findBackpacks.SetActive(true);
        findBackpacks.GetComponent<TextMeshProUGUI>().text = "- Find the backpacks (" + bagsPickedUp + "/" + playerCount + ")";
        findBackpacksDesc.transform.localPosition = findBackpacksDesc.transform.localPosition - parentTing.transform.localPosition;
        findBackpacksDesc.SetActive(true);
        findBrokenFence.SetActive(false);
       
        findBackpacks.transform.localPosition = findBackpacks.transform.localPosition - parentTing.transform.localPosition;
        findBackpacksBackground.SetActive(true);
        breakFence.transform.localPosition = breakFence.transform.localPosition - parentTing.transform.localPosition;
        findBackpacksDescLaptop.transform.localPosition = findBackpacksDescLaptop.transform.localPosition - parentTing.transform.localPosition;
        findBackpacksDescLaptop.SetActive(true);

        findBackpacksLaptopDist.transform.transform.localPosition = findBackpacksLaptopDist.transform.localPosition - parentTing.transform.localPosition;
        findBackpacksLaptopDist.SetActive(true);
    }

    void SetFindTrainToActive() 
    {
        bagsPickedUp += 1;

       // Debug.Log("picked up bag");

        findBackpacks.GetComponent<TextMeshProUGUI>().text = "- Find the backpacks (" + bagsPickedUp + "/" + playerCount + ")";

        if (bagsPickedUp == playerCount)
        {
            breakFence.SetActive(false);
            findBackpacks.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findBackpacksDesc.SetActive(false);
            findBackpacksBackground.SetActive(false);
            findBackpacksDescLaptop.SetActive(false);
            findBackpacksLaptopDist.SetActive(false);


            findBackpacks.transform.localPosition = findBackpacks.transform.localPosition - parentTing.transform.localPosition;
            findTrain.transform.localPosition = findTrain.transform.localPosition - parentTing.transform.localPosition - parentTing.transform.localPosition ;
            findTrainDesc.transform.localPosition = findTrainDesc.transform.localPosition - parentTing.transform.localPosition - parentTing.transform.localPosition  ;
            findTrainDistance.transform.localPosition = findTrainDistance.transform.localPosition - parentTing.transform.localPosition - parentTing.transform.localPosition ;
            findTrain.SetActive(true);
            findTrainDesc.SetActive(true);
            findTrainBackground.SetActive(true);
            findTrainDistance.SetActive(true);
            GetComponent<PlayerAudioClips>().BagsCollected();
        }
    }

    void SetEscapeToActive()
    {
       // Debug.Log("IS BACKPACKS ACTIVE? " + findBackpacksDesc.activeSelf);
        if(!findBackpacksDesc.activeSelf && findBackpacks.activeSelf) 
        {
            findTrain.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            findTrainDesc.SetActive(false);
            findTrainDistance.SetActive(false);
            findTrainBackground.SetActive(false);
            findBackpacks.SetActive(false);
            findTrain.transform.localPosition = findTrain.transform.localPosition - parentTing.transform.localPosition;
            escapeOnTrain.transform.localPosition = escapeOnTrain.transform.localPosition - parentTing.transform.localPosition*2.3f;
            escapeOnTrainDesc.transform.localPosition = escapeOnTrainDesc.transform.localPosition - parentTing.transform.localPosition*2.1f;
            escapeOnTrain.SetActive(true);
            escapeOnTrainDesc.SetActive(true);
            escapeOnTrainBackground.SetActive(true);
            
        }
    }//escape train desc + 15 //escape train + 20

    void SetObjectivesComplete()
    {
        escapeOnTrain.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        escapeOnTrainDesc.SetActive(false);
       // escapeOnTrainBackground.SetActive(false);
        //escapeOnTrainCompleteBackground.SetActive(true);
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
