using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class GuardAnimation : MonoBehaviourPun
{
    public NavMeshAgent guard;
    private Animator animator;
    private int isWalkingHash;
    private int isChasingHash;
    private int hasCaughtHash;
    public bool chasingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        guard = GetComponent<NavMeshAgent>();
        chasingPlayer = GetComponent<GuardAIPhoton>().getSpotted();

        // Setup animation variables
        animator = GetComponent<Animator>();

        // using StringToHash increases performance by nearly 50%
        isWalkingHash = Animator.StringToHash("isWalking");
        isChasingHash = Animator.StringToHash("isChasing");
        hasCaughtHash = Animator.StringToHash("hasCaught");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("SPEED ANIM " + guard.speed);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isChasing = animator.GetBool(isChasingHash);
        bool hasCaught = animator.GetBool(hasCaughtHash);

        if ((Mathf.Abs(guard.velocity.z) > 0.2f || Mathf.Abs(guard.velocity.x) > 0.2f) && !chasingPlayer) {
            animator.SetBool(isWalkingHash, true); 
            animator.SetBool(isChasingHash, false);
            animator.SetBool(hasCaughtHash, false);
        } else if ((isWalking || isChasing) && (Mathf.Abs(guard.velocity.z) <= 0.2f || Mathf.Abs(guard.velocity.x) <= 0.2f)) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isChasingHash, false);
            animator.SetBool(hasCaughtHash, true);
        }
        if (chasingPlayer && (Mathf.Abs(guard.velocity.z) > 0.2f || Mathf.Abs(guard.velocity.x) > 0.2f)) {
            animator.SetBool(isChasingHash, true);
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(hasCaughtHash, false);
        } else if (isChasing && !chasingPlayer) {
            animator.SetBool(isChasingHash, false);
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(hasCaughtHash, false);
        } else if (chasingPlayer && (Mathf.Abs(guard.velocity.z) <= 0.2f || Mathf.Abs(guard.velocity.x) <= 0.2f)) {
            animator.SetBool(isChasingHash, false);
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(hasCaughtHash, true);
        }
    }

    public bool getChasing() {
        return this.chasingPlayer;
    }
    public void setChasing(bool val) {
        this.chasingPlayer = val;
    }
}
