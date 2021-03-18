using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private HashSet<Collider> colliders = new HashSet<Collider>();

    public event Action StartEndGame;
    private bool gameEnding;
    private float timeToEnd;
    private bool gameWon;

    public HashSet<Collider> GetColliders() { return colliders; }

    void Start()
    {
        // subscribe event to function
        StartEndGame += HandleEndGame;
        gameEnding = false;
        
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "locomotive")
        {
            colliders.Add(other);
            Debug.Log(other.gameObject);
            Debug.Log(colliders.Count);
            // 6 box colliders on the train so when all of them are in endgame, start endgame
            if (colliders.Count == 6)
            {
                StartEndGame();
            }
        }
        
    }

    private void OnTriggerExit (Collider other)
    {
        colliders.Remove(other);
        Debug.Log(other.gameObject);
    }

    private void HandleEndGame()
    {
        gameEnding = true;
        timeToEnd = 0f;
    }

    void Update()
    {
        if (gameEnding)
        {
            timeToEnd += Time.deltaTime;
            if (timeToEnd > 5f)
            {
                gameWon = true;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (!player.GetComponent<PlayerMovementPhoton>().OnTrain)
                    {
                        gameWon = false;
                        break;
                    }
                }
                
                if (gameWon == true)
                {
                    Debug.Log("you won!");
                } 
                else
                {
                    Debug.Log("you lost...");
                }
                gameEnding = false;
            }
        }
    }
}
