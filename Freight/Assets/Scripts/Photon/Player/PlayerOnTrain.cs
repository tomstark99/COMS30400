using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script attached to the player. Returns if the player is on the train
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
