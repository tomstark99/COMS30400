using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class ObjectivesSecond : MonoBehaviour
{
    [SerializeField]
    private GameObject dropBags;
    [SerializeField]
    private GameObject dropBagsDistance;
    [SerializeField]
    private GameObject dropBagsDistance2;
    [SerializeField]
    private GameObject dropBagsDesc;
    [SerializeField]
    private GameObject dropBagsBackground;

    private bool twoPlayers;

    private GameObject drop1;
    private GameObject drop2;

    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        drop1 = GameObject.FindGameObjectWithTag("DropSpot");
        drop1.GetComponent<Droppable>().BagDropped += DropBags;
        counter = 1;

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            twoPlayers = true;
            dropBagsDistance2.SetActive(true);
            drop2 = GameObject.FindGameObjectWithTag("DropSpot2");
            drop2.GetComponent<Droppable>().BagDropped += DropBags;
            counter = 2;
        }

    }

    void DropBags()
    {
        counter -= 1;

        if (counter == 0)
        {
            dropBagsDistance.SetActive(false);
            dropBagsDistance2.SetActive(false);
            dropBagsDesc.SetActive(false);
        }
        else
        {
            if (drop1.GetComponent<Droppable>().isDroppedOff)
            {
                dropBagsDistance.GetComponent<TextMeshProUGUI>().text = "-Drop point 1 delivered";
                dropBagsDistance.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            } 
            else
            {
                dropBagsDistance2.GetComponent<TextMeshProUGUI>().text = "-Drop point 2 delivered";
                dropBagsDistance2.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dropBagsDesc.activeSelf)
        {
            if (!drop1.GetComponent<Droppable>().isDroppedOff)
            {
                float distance = Vector3.Distance(transform.position, drop1.transform.position);

                dropBagsDistance.GetComponent<TextMeshProUGUI>().text = "-Drop point 1 distance: " + Math.Round(distance, 2) + "m";
            }
            if (twoPlayers)
                if (!drop2.GetComponent<Droppable>().isDroppedOff)
                {
                    float distance = Vector3.Distance(transform.position, drop2.transform.position);

                    dropBagsDistance2.GetComponent<TextMeshProUGUI>().text = "-Drop point 2 distance: " + Math.Round(distance, 2) + "m";
                }
        }
    }
}
