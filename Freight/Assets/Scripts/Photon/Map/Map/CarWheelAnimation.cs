using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject frontRight;
    [SerializeField]
    private GameObject frontLeft;
    [SerializeField]
    private GameObject backRight;
    [SerializeField]
    private GameObject backLeft;

    private bool isSpinning;

    public bool IsSpinning
    {
        set { isSpinning = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        isSpinning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // makes the wheels on the getaway vehicle appear to spin
        if (isSpinning)
        {
            frontRight.transform.Rotate(Vector3.back * 100f * Time.deltaTime);
            frontLeft.transform.Rotate(Vector3.back * 100f * Time.deltaTime);
            backRight.transform.Rotate(Vector3.back * 100f * Time.deltaTime);
            backLeft.transform.Rotate(Vector3.back * 100f * Time.deltaTime);
        }
    }
}
