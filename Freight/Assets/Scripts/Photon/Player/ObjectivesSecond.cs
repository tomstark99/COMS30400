using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class ObjectivesSecond : MonoBehaviourPun
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
    [SerializeField]
    private GameObject rendezvous;
    [SerializeField]
    private GameObject rendezvousDist;
    [SerializeField]
    private GameObject waitForOtherPlayer;

    private GameObject endGame;

    private bool twoPlayers;

    private GameObject drop1;
    private GameObject drop2;

    // used to check how many bags are left to drop
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        // subscribes to event on drop spot to notify when bags have been dropped
        drop1 = GameObject.FindGameObjectWithTag("DropSpot");
        drop1.GetComponent<Droppable>().BagDropped += DropBags;
        counter = 1;

        // if there are 2 players, other drop spot is activated and event is subscribed to
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            twoPlayers = true;
            dropBagsDistance2.SetActive(true);
            drop2 = GameObject.FindGameObjectWithTag("DropSpot2");
            drop2.GetComponent<Droppable>().BagDropped += DropBags;
            counter = 2;
        }

        endGame = GameObject.FindGameObjectWithTag("EndGame");
        
    }

    void DropBags()
    {
        // decrements counter
        counter -= 1;

        // once counter hits 0, indicates that there are no more bags to drop thus we can subscribe to end game checking event
        if (counter == 0)
        {
            GetComponent<PlayerAudioClipsSecond>().BagsDroppedOff();

            // disables gameobjects to do with drop spots
            dropBagsDistance.SetActive(false);
            dropBagsDistance2.SetActive(false);
            dropBagsDesc.SetActive(false);

            // activates distance to truck objective
            rendezvous.SetActive(true);
            rendezvousDist.SetActive(true);

            // subscribe to event that checks endgame
            endGame.GetComponent<EndGameSecond>().PlayerReadyToLeave += EndGameChecker;

            // outline back of the truck 
            endGame.GetComponent<Outline>().enabled = true;
        }
        else
        {
            // this is checking which drop point was the one that called the event, updates objectives so drop point is no longer having its distance calculated and its crossed out
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

    //[PunRPC]
    //void SetReadyToLeaveRPC()
    //{
    //    readyToLeave = true;
    //    endGame.GetComponent<EndGameSecond>().CheckEndGame();
    //} 

    // function is called when someone jumps in the back of the truck and triggers the collider
    public void EndGameChecker()
    {
        // if 2 players, disable the distance to truck and activate game object that tells you to wait for other player
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            rendezvousDist.SetActive(false);
            waitForOtherPlayer.SetActive(true);
            //GetComponent<PlayerMovementPhoton>().GameEnding();
            GetComponent<PlayerMovementPhoton>().enabled = false;
        }
        else
        {
            // if solo game, end the game
            endGame.GetComponent<EndGameSecond>().EndTheGame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // checks if the current objective is to drop the bag
        if (dropBagsDesc.activeSelf)
        {
            // update distances to drop points if they have not been dropped off yet
            if (!drop1.GetComponent<Droppable>().isDroppedOff)
            {
                float distance = Vector3.Distance(transform.position, drop1.transform.position);

                dropBagsDistance.GetComponent<TextMeshProUGUI>().text = "-Drop point 1 distance: " + Math.Round(distance, 2) + "m";
            }
            // if there are 2 players in the game, update second drop point
            if (twoPlayers)
                if (!drop2.GetComponent<Droppable>().isDroppedOff)
                {
                    float distance = Vector3.Distance(transform.position, drop2.transform.position);

                    dropBagsDistance2.GetComponent<TextMeshProUGUI>().text = "-Drop point 2 distance: " + Math.Round(distance, 2) + "m";
                }
        }
        // checks if current objective is the rendezvous point and updates distance to it
        else if (rendezvousDist.activeSelf)
        {
            float distance = Vector3.Distance(transform.position, endGame.transform.position);

            rendezvousDist.GetComponent<TextMeshProUGUI>().text = "-Distance: " + Math.Round(distance, 2) + "m";
        }
    }
}
