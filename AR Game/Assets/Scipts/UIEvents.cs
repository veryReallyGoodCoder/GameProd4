using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIEvents : MonoBehaviour
{
    public RawImage textDisp;
    public TMP_Text text;
    
   public void Welcome()
    {
        text.SetText("Point Towards a Flat Surface, Like a Table or Floor");
        Destroy(textDisp, 3);
    }

}
