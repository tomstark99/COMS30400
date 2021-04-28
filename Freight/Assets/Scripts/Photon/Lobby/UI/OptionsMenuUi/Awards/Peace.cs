using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Peace : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMeshProUGUI text1;
    
    private void Start() {
        text1 = transform.parent.parent.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
        
    }
    // Start is called before the first frame update
    public void onMouseClick() {
        text1.SetText("<size=100%>                            Peace Treaty:\n <size=70%>                Complete the level without killing any guards");
    }
}
