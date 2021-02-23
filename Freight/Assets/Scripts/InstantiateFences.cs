using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFences : MonoBehaviour
{
    public GameObject fence;
    public GameObject bottomFence;
    public GameObject fenceBrokenPartial;

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = (int)System.DateTime.Now.Ticks/(Random.Range(1,50000));
        //int brokenPartPos = Random.Range(10,30);
        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(270.0f, 6.5f, (193.0f + i*5.0f));
            if (i == 10) {
                Instantiate(fenceBrokenPartial, position, Quaternion.Euler(0f, 90f, 0f));
            } else {
                Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
            }
        }

        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(500.0f, 6.5f, (193.0f + i * 5.0f));
            if (i > 4 || i < 2)
            {
                Instantiate(fence, position, Quaternion.Euler(0f, 90f, 0f));
            }
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 190.5f);
            if (i > 36 && i < 42)
            {
                Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
            }
            else
            {
                Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
            }
            
        }

        for (int i = 0; i < 46; i++)
        {
            Vector3 position = new Vector3((272.5f + i * 5.0f), 6.5f, 440.5f);
            if (i > 27 && i < 34)
            {
                Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
                position.y -= 3;
                Instantiate(bottomFence, position, Quaternion.Euler(0f, 0f, 0f));
            }
            else
            {
                Instantiate(fence, position, Quaternion.Euler(0f, 0f, 0f));
            }
        }
        Debug.Log("Fences");
    }
}
