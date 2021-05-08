using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetDetected : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMeshProUGUI text1;
    
    private void Start() {
        text1 = transform.parent.parent.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
        
    }
    // Start is called before the first frame update
    public void onMouseClick() {
        text1.SetText("<size=100%>                       Learn the hard way:\n <size=70%>       Walk into the sight of one of the guards and get detected");
    }
}
