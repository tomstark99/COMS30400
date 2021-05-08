using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]

public class IkBehaviour : MonoBehaviourPun
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform handObj = null;
    public Transform lookObj = null;

    public Transform rightArm;
    public Transform rightForeArm;
    public Transform rightHand;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    void AnimateRPC(int playerID)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, handObj.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handObj.rotation);
    }

    [PunRPC]
    void UnAnimateRPC()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
    }

    void OnAnimatorIK() {
        if(animator) {
            if(ikActive) {
                if (lookObj != null)
                {
                    // animator.SetLookAtWeight(1);
                    // animator.SetLookAtPosition(lookObj.position);
                }
                if (handObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, handObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, handObj.rotation);
                    
                    //photonView.RPC(nameof(AnimateRPC), RpcTarget.Others, GetComponent<PhotonView>().ViewID);
                }
            } 
            else {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                // animator.SetLookAtWeight(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
