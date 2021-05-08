using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnTrain : MonoBehaviour
{
    [SerializeField]
    private bool onTrain;

    public bool OnTrain
    {
        get { return onTrain; }
        set { onTrain = value;  }
    }
    


}
