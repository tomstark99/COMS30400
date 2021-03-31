using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateLight : MonoBehaviour
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;
    private float rotationMultiplier = 0.25f;
    public bool spinningLeft;
    public float rotationUpperLimit;
    public float rotationLowerLimit;

    // Start is called before the first frame update
    void Start()
    {
        positiveRotation = true;
        if (spinningLeft == false)
        {
            rotationMultiplier *= -1;
            positiveRotation = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(spotlight.transform.rotation.y);
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * rotationMultiplier);
            if (spotlight.transform.rotation.y > rotationUpperLimit)
            {
                positiveRotation = false;
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -rotationMultiplier);
            if (spotlight.transform.rotation.y < rotationLowerLimit)
            {
                positiveRotation = true;
            }
        }
        
    }
}
