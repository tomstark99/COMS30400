using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Animator animator;
    public bool move;

    private int moveHash;
    // private bool moving;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        moveHash = Animator.StringToHash("move");

        animator.SetBool(moveHash, false);

    }
    // Update is called once per frame
    void Update()
    {
        
        if(move) {
            animator.SetBool(moveHash, true);
        } else {
            animator.SetBool(moveHash, false);
        }
    }
}
