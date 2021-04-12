using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private int guardSightRange;
    private int guardAngle;
    private int speedChasing;
    private int speedPatrolling;
    private int timeToLeave;
    private float spotlightRotationSpeed;
    private int ammoAmount;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
