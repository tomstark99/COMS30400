 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.UI;
 public class AIController : MonoBehaviour
 {
 
     public Transform Player;
     public int MoveSpeed;
     public int MaxDist;
     public int MinDist;
 
 
 
 
     void Start()
     {
 
     }
 
     void Update()
     {
         transform.LookAt(Player);
 
         if (Vector3.Distance(transform.position, Player.position) >= MinDist)
         {
 
             transform.position += transform.forward * MoveSpeed * Time.deltaTime;
 
 
 
             //if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            // {
                 //Here Call any function U want Like Shoot at here or something
           //  }
 
         }
     }
 }