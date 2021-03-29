using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateLight : MonoBehaviour
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;

    // Start is called before the first frame update
    void Start()
    {
        positiveRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * 0.25f);
            if (spotlight.transform.rotation.y > 0.35f)
            {
                positiveRotation = false;
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -0.25f);
            if (spotlight.transform.rotation.y < -0.7f)
            {
                positiveRotation = true;
            }
        }
        
    }
}
