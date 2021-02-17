using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTrees : MonoBehaviour
{
    public GameObject tree;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 25; i++)
        {
            Vector3 position = new Vector3(Random.Range(250.0f, 270.0f), 5, Random.Range(200.0f, 420.0f));
            Instantiate(tree, position, Quaternion.identity);
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 position = new Vector3(Random.Range(275.0f, 450.0f), 5, Random.Range(185.0f, 165.0f));
            Instantiate(tree, position, Quaternion.identity);
        }

        for (int i = 0; i < 25; i++)
        {
            Vector3 position = new Vector3(Random.Range(510.0f, 530.0f), 5, Random.Range(200.0f, 400.0f));
            Instantiate(tree, position, Quaternion.identity);
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 position = new Vector3(Random.Range(275.0f, 450.0f), 5, Random.Range(445.0f, 465.0f));
            Instantiate(tree, position, Quaternion.identity);
        }
    }
}
