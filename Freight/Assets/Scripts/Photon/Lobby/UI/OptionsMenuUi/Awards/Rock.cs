using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rock : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMeshProUGUI text1;
    
    private void Start() {
        text1 = transform.parent.parent.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
        
    }
    // Start is called before the first frame update
    public void onMouseClick() {
        text1.SetText("<size=100%>                             Use nature:\n <size=70%>                 Distract a guard by throwing a rock near them");
    }
}
