using UnityEngine;
 public class VisibleByDistance : MonoBehaviour
 {
     public Transform PlayerObject;
     public float DistanceToView = 10f;
     private TextMesh myTextMesh;
     private string MyOriginalText;
 
     void Start()
     {
         myTextMesh = GetComponent<TextMesh>();
         MyOriginalText = myTextMesh.text;
     }
     void Update()
     {
        /* if (Vector3.Distance(PlayerObject.position, transform.position) <= DistanceToView)
         {
             myTextMesh.text = MyOriginalText;
         }
         else
         {
             myTextMesh.text = "";
         }*/
     }
 }