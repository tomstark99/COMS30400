using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFences : MonoBehaviour
{
    public GameObject fence;
    public GameObject fenceBrokenFull;
    public GameObject fenceBrokenPartial;

    // Start is called before the first frame update
    void Start()
    {
        Random.seed = (int)System.DateTime.Now.Ticks/(Random.Range(1,50000));
        int brokenFullPos = Random.Range(1,10);
        int brokenPartPos = Random.Range(10,30);
        Debug.Log(brokenFullPos + "   sdf   " + brokenPartPos);
        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(270.0f, 6.5f, (193.0f + i*5.0f));
            if (i == brokenFullPos) {
                Instantiate(fenceBrokenFull, position, Quaternion.Euler(0f, 90f, 0f));
            } else if (i == brokenPartPos) {
                Instantiate(fenceBrokenPartial, position, Quaternion.Euler(0f, 90f, 0f));
            } else {
                Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
            }
        }

        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(500.0f, 6.5f, (193.0f + i * 5.0f));
            Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 190.5f);
            Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 440.5f);
            Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
        }
    }
}
