using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;


public class showText : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    private Text txtMy;
    
    // Start is called before the first frame update
    void Start()
    {
        txtMy = GameObject.Find("Canvas/Text").GetComponent<Text>();
        txtMy.text = "This text";
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            
            string pose = StringReturnValueFunction();

            txtMy.text = pose;

            // GetComponent(TextMesh).text = pose;
        }
    }
}
